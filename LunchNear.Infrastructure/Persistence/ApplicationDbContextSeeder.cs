namespace LunchNear.Infrastructure.Persistence;

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using LunchNear.Domain.Entities;
using LunchNear.Domain.Enums;
using LunchNear.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Seeds demo data through the domain's own factory methods and aggregate behaviour
/// (Restaurant.AddStudentDiscount/AddLunchDeal/AddRating, Dish.AddRating) instead of EF Core's
/// HasData() - HasData() only supports plain object-initializer snapshots, which cannot call
/// into private constructors/invariants, so it is a poor fit for a rich domain model.
///
/// Restaurant names/addresses/coordinates are fetched once from OpenStreetMap's Overpass API
/// (real places in Gothenburg, Sweden) so the app shows genuine venues instead of invented ones.
/// OSM has no notion of student discounts, lunch deals, dishes or ratings - those are LunchNear's
/// own features, so they are generated deterministically per restaurant (seeded by its OSM id)
/// on top of the real place data. If the API is unreachable, a small hardcoded fallback list is
/// used instead so the app still works offline.
/// </summary>
public sealed class ApplicationDbContextSeeder
{
    private const string OverpassEndpoint = "https://overpass-api.de/api/interpreter";

    // Bounding box roughly covering central Gothenburg (south, west, north, east).
    private const string GothenburgBoundingBox = "57.685,11.94,57.715,11.99";

    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApplicationDbContextSeeder> _logger;

    public ApplicationDbContextSeeder(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<ApplicationDbContextSeeder> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);

        if (await _context.Restaurants.AnyAsync(cancellationToken))
        {
            return;
        }

        _logger.LogInformation("Database is empty, seeding demo data.");

        var specs = await GetSeedSpecsAsync(cancellationToken);

        var restaurants = specs
            .Select(spec => CreateRestaurant(spec))
            .ToList();

        _context.Restaurants.AddRange(restaurants);
        await _context.SaveChangesAsync(cancellationToken);

        var dishes = new List<Dish>();
        for (var i = 0; i < restaurants.Count; i++)
        {
            var restaurant = restaurants[i];
            foreach (var (dishName, price) in specs[i].Dishes)
            {
                dishes.Add(Dish.Create(restaurant.Id, dishName, description: null, price));
            }
        }

        _context.Dishes.AddRange(dishes);
        await _context.SaveChangesAsync(cancellationToken);

        SeedSampleRatings(restaurants[0], dishes[0]);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seeding completed: {RestaurantCount} restaurants, {DishCount} dishes.",
            restaurants.Count,
            dishes.Count);
    }

    private void SeedSampleRatings(Restaurant restaurant, Dish dish)
    {
        restaurant.AddRating("demo-user-1", RatingValue.Create(5), "Great atmosphere and quick service!");
        restaurant.AddRating("demo-user-2", RatingValue.Create(4), "Good value for the price.");

        dish.AddRating("demo-user-1", RatingValue.Create(5), "Best dish on the menu.");
    }

    private static Restaurant CreateRestaurant(RestaurantSeedSpec spec)
    {
        var restaurant = Restaurant.Create(
            spec.Name,
            spec.Address,
            spec.PriceRange,
            GeoLocation.Create(spec.Latitude, spec.Longitude));

        if (spec.StudentDiscountPercentage is int discountPercentage)
        {
            restaurant.AddStudentDiscount("Student discount with a valid student ID", discountPercentage);
        }

        if (spec.LunchDeal is { } deal)
        {
            restaurant.AddLunchDeal(deal.Name, "Daily lunch offer", deal.Price, deal.Start, deal.End, deal.Days);
        }

        return restaurant;
    }

    /// <summary>
    /// Tries to fetch real restaurants in Gothenburg from OpenStreetMap. Falls back to a small
    /// hardcoded list of real Gothenburg addresses if the API call fails for any reason (offline,
    /// timeout, rate limit, unexpected response shape) - seeding must never crash the app.
    /// </summary>
    private async Task<List<RestaurantSeedSpec>> GetSeedSpecsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var real = await FetchRealGothenburgRestaurantsAsync(cancellationToken);
            if (real.Count > 0)
            {
                _logger.LogInformation("Fetched {Count} real restaurants from OpenStreetMap for seeding.", real.Count);
                return real;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not fetch real restaurant data from OpenStreetMap, falling back to built-in demo list.");
        }

        return GetFallbackSeedSpecs();
    }

    private async Task<List<RestaurantSeedSpec>> FetchRealGothenburgRestaurantsAsync(CancellationToken cancellationToken)
    {
        var http = _httpClientFactory.CreateClient("Overpass");

        var query = $"[out:json][timeout:25];(node[\"amenity\"=\"restaurant\"]({GothenburgBoundingBox}););out body;";
        using var request = new HttpRequestMessage(HttpMethod.Post, OverpassEndpoint)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["data"] = query })
        };

        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OverpassResponse>(cancellationToken: cancellationToken);
        var elements = payload?.Elements ?? [];

        var candidates = elements
            .Where(e => e.Tags is not null && e.Tags.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(name))
            .Where(e => e.Tags!.ContainsKey("addr:street"))
            .DistinctBy(e => e.Tags!["name"])
            .OrderBy(e => e.Id)
            .Take(12)
            .ToList();

        return candidates.Select(BuildSpecFromOsm).ToList();
    }

    private static RestaurantSeedSpec BuildSpecFromOsm(OverpassElement element)
    {
        var tags = element.Tags!;
        var name = tags["name"];
        var street = tags["addr:street"];
        var houseNumber = tags.GetValueOrDefault("addr:housenumber");
        var address = string.IsNullOrWhiteSpace(houseNumber) ? street : $"{street} {houseNumber}";
        var cuisine = tags.GetValueOrDefault("cuisine", "");

        // Deterministic per-restaurant randomness (seeded by the stable OSM node id) so the
        // generated LunchNear-only extras (price range, discount, lunch deal) don't change
        // between app restarts even though we re-fetch from OSM each time the DB is empty.
        var rng = new Random(unchecked((int)element.Id));

        var priceRange = rng.Next(100) switch
        {
            < 30 => PriceRange.Cheap,
            < 80 => PriceRange.Medium,
            _ => PriceRange.Expensive
        };

        int? studentDiscount = rng.Next(100) < 40 ? rng.Next(10, 21) : null;

        LunchDealSeedSpec? lunchDeal = rng.Next(100) < 55
            ? new LunchDealSeedSpec(
                "Lunch Deal",
                priceRange == PriceRange.Cheap ? rng.Next(95, 121) : rng.Next(115, 165),
                new TimeOnly(11, 0),
                new TimeOnly(rng.Next(14, 16), rng.Next(0, 2) * 30),
                rng.Next(100) < 20 ? "Mon-Sat" : "Mon-Fri")
            : null;

        return new RestaurantSeedSpec(
            name,
            address,
            priceRange,
            element.Lat,
            element.Lon,
            studentDiscount,
            lunchDeal,
            BuildDishesForCuisine(cuisine, rng));
    }

    private static (string Name, decimal Price)[] BuildDishesForCuisine(string cuisine, Random rng)
    {
        var menu = cuisine.ToLowerInvariant() switch
        {
            var c when c.Contains("pizza") || c.Contains("italian") =>
                new[] { ("Margherita", 145m), ("Carbonara", 165m), ("Tiramisu", 79m) },
            var c when c.Contains("sushi") || c.Contains("japanese") =>
                new[] { ("California Roll", 119m), ("Miso Soup", 49m), ("Ramen", 139m) },
            var c when c.Contains("chinese") || c.Contains("asian") =>
                new[] { ("Kung Pao Chicken", 139m), ("Spring Rolls", 69m), ("Fried Rice", 99m) },
            var c when c.Contains("thai") =>
                new[] { ("Pad Thai", 135m), ("Tom Yum Soup", 89m), ("Mango Sticky Rice", 69m) },
            var c when c.Contains("greek") =>
                new[] { ("Gyros", 129m), ("Greek Salad", 95m), ("Baklava", 55m) },
            var c when c.Contains("indian") =>
                new[] { ("Chicken Tikka Masala", 145m), ("Garlic Naan", 45m), ("Mango Lassi", 39m) },
            var c when c.Contains("burger") =>
                new[] { ("Classic Burger", 129m), ("Sweet Potato Fries", 55m), ("Milkshake", 49m) },
            var c when c.Contains("vegan") || c.Contains("vegetarian") =>
                new[] { ("Buddha Bowl", 119m), ("Falafel Wrap", 89m), ("Smoothie", 55m) },
            var c when c.Contains("seafood") || c.Contains("fish") =>
                new[] { ("Fish & Chips", 149m), ("West Coast Stew", 165m), ("Toast Skagen", 135m) },
            var c when c.Contains("kebab") =>
                new[] { ("Kebab Plate", 99m), ("Falafel", 69m), ("Ayran", 29m) },
            var c when c.Contains("cafe") || c.Contains("coffee") =>
                new[] { ("Avocado Toast", 89m), ("Cinnamon Bun", 39m), ("Latte", 45m) },
            _ =>
                new[] { ("Meatballs with Mash", 129m), ("Pea Soup", 89m), ("Lingonberry Cake", 55m) }
        };

        // Small deterministic price jitter so identical cuisines don't all cost exactly the same.
        return menu.Select(d => (d.Item1, d.Item2 + rng.Next(-10, 11))).ToArray();
    }

    private static List<RestaurantSeedSpec> GetFallbackSeedSpecs() =>
    [
        new(
            "Green Courtyard",
            "Kungsportsavenyn 14",
            PriceRange.Cheap,
            57.7008, 11.9765,
            StudentDiscountPercentage: 15,
            LunchDeal: new LunchDealSeedSpec("Business Lunch", 99m, new TimeOnly(11, 0), new TimeOnly(14, 0), "Mon-Fri"),
            Dishes: [("Meatballs with mash", 129m), ("Pea soup", 89m), ("Lingonberry cordial", 25m)]),
        new(
            "Pasta & Basta",
            "Haga Nygata 9",
            PriceRange.Medium,
            57.6975, 11.9611,
            StudentDiscountPercentage: 10,
            LunchDeal: new LunchDealSeedSpec("Lunch Set", 135m, new TimeOnly(11, 0), new TimeOnly(15, 0), "Mon-Fri"),
            Dishes: [("Carbonara", 165m), ("Margherita", 145m), ("Tiramisu", 79m)]),
        new(
            "Sakura Sushi",
            "Linnégatan 21",
            PriceRange.Medium,
            57.6949, 11.9560,
            StudentDiscountPercentage: null,
            LunchDeal: new LunchDealSeedSpec("Sushi Lunch", 149m, new TimeOnly(11, 30), new TimeOnly(15, 0), "Mon-Sun"),
            Dishes: [("California Roll", 119m), ("Miso Soup", 49m), ("Ramen", 139m)]),
        new(
            "Steak House Prime",
            "Vasagatan 33",
            PriceRange.Expensive,
            57.7010, 11.9660,
            StudentDiscountPercentage: null,
            LunchDeal: null,
            Dishes: [("Ribeye Steak", 349m), ("Caesar Salad", 155m), ("Cheesecake", 95m)]),
        new(
            "Vegan Corner",
            "Järntorget 4",
            PriceRange.Cheap,
            57.6989, 11.9553,
            StudentDiscountPercentage: 20,
            LunchDeal: new LunchDealSeedSpec("Green Lunch", 109m, new TimeOnly(11, 0), new TimeOnly(14, 30), "Mon-Fri"),
            Dishes: [("Buddha Bowl", 119m), ("Falafel Wrap", 89m), ("Smoothie", 55m)]),
        new(
            "Majorna Bistro",
            "Karl Johansgatan 61",
            PriceRange.Medium,
            57.6960, 11.9330,
            StudentDiscountPercentage: 12,
            LunchDeal: new LunchDealSeedSpec("Lunch Special", 119m, new TimeOnly(11, 0), new TimeOnly(15, 0), "Mon-Sat"),
            Dishes: [("Fish & chips", 149m), ("West coast stew", 165m), ("Toast Skagen", 135m)]),
    ];

    private sealed record RestaurantSeedSpec(
        string Name,
        string Address,
        PriceRange PriceRange,
        double Latitude,
        double Longitude,
        int? StudentDiscountPercentage,
        LunchDealSeedSpec? LunchDeal,
        (string Name, decimal Price)[] Dishes);

    private sealed record LunchDealSeedSpec(string Name, decimal Price, TimeOnly Start, TimeOnly End, string Days);

    private sealed record OverpassResponse([property: JsonPropertyName("elements")] List<OverpassElement> Elements);

    private sealed record OverpassElement(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lon")] double Lon,
        [property: JsonPropertyName("tags")] Dictionary<string, string>? Tags);
}
