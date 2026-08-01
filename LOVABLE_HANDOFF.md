# LunchNear — Lovable Design Handoff

## What to build

Redesign the frontend of **LunchNear**, a mobile-first restaurant discovery app for students in Gothenburg, Sweden.

Build a polished, production-looking **React + TypeScript** prototype that can later be ported back to an existing **Blazor WebAssembly** frontend. Focus on layout, components, responsive behavior, transitions, loading/empty/error states, and accessibility.

Do not build a new backend, authentication system, or database. Use the attached `lovable-sample-data.json` as mock data.

## Product idea

LunchNear helps students:

- find restaurants near their current location;
- discover student discounts;
- see lunch deals;
- rate restaurants;
- rate each individual dish independently.

The ability to rate dishes separately from restaurants is the main product feature.

## Language and locale

- UI language: English only.
- City: Gothenburg, Sweden.
- Currency: SEK, displayed as `kr`.
- Distances: kilometres.
- Mobile-first, but desktop should show a centred app surface with a sensible maximum width.

## Required routes

### `/` — Home

Include:

- LunchNear brand;
- concise tagline: `Find lunch nearby with student discounts`;
- three clear value propositions:
  - Restaurants near you
  - Student discounts and lunch deals
  - Rate restaurants and every dish
- primary CTA: `Browse restaurants`;
- bottom navigation with Home and Restaurants.

The home screen should feel like a real consumer product, not a generic landing-page template.

### `/restaurants` — Restaurant discovery

Include:

- heading: `Restaurants nearby`;
- supporting text: `Student discounts, lunch deals and dish ratings`;
- search by restaurant name;
- filter chips:
  - Nearby
  - Discounts
  - Lunch
- restaurant cards showing:
  - name;
  - address;
  - distance;
  - average rating;
  - price range (`Cheap`, `Medium`, `Expensive`);
  - student-discount badge when applicable;
  - lunch-deal badge when applicable.

Behavior:

- Search updates immediately while typing.
- Filters can be combined.
- Nearby results are sorted by distance.
- Clicking a restaurant opens `/restaurant/:id`.
- Provide skeleton loading, empty-results, location-permission, and network-error states.

### `/restaurant/:id` — Restaurant details

Include:

- back navigation;
- restaurant name, address, distance, price level, rating and number of ratings;
- a compact section for submitting a restaurant rating:
  - interactive 1–5 star picker;
  - optional review;
  - submit button;
  - pending, success and error feedback;
- student-discount section, only when discounts exist;
- lunch-deal cards with name, price, description, hours and days;
- dishes section.

Each dish must show:

- name;
- optional description;
- price in `kr`;
- its own average rating and rating count;
- independent 1–5 star picker;
- Rate button and submission feedback.

Restaurant and dish ratings must look clearly separate in the UI.

## Navigation

Use a mobile bottom navigation bar with:

- Home
- Restaurants

It should account for mobile safe-area insets and remain easy to reach with one hand.

## Visual direction

Create a warm, modern Scandinavian food-discovery aesthetic:

- inviting rather than corporate;
- clean typography and generous spacing;
- strong visual hierarchy;
- light neutral background;
- warm coral/orange primary accent;
- restrained teal/green accent for discounts and distance;
- amber/gold for ratings;
- soft surfaces, subtle borders and shadows;
- rounded cards and controls without making everything excessively pill-shaped;
- tasteful micro-interactions;
- accessible contrast and visible focus states.

Avoid:

- stock dashboard styling;
- large desktop sidebars;
- excessive gradients or glass effects;
- random placeholder charts;
- fake social feeds;
- making the interface look like a food-delivery checkout app.

Restaurant photos may be represented by tasteful gradient/image placeholders because the current API does not return image URLs. Keep the layout usable when images are absent.

## Data rules

The real .NET API returns the following shapes.

```ts
type Restaurant = {
  id: number;
  name: string;
  address: string;
  priceRange: "Cheap" | "Medium" | "Expensive";
  latitude: number;
  longitude: number;
  averageRating: number;
  ratingsCount: number;
  hasStudentDiscount: boolean;
  hasLunchDeals: boolean;
};

type NearbyRestaurant = {
  restaurant: Restaurant;
  distanceKm: number;
};

type Dish = {
  id: number;
  restaurantId: number;
  name: string;
  description: string | null;
  price: number;
  averageRating: number;
  ratingsCount: number;
};

type StudentDiscount = {
  id: number;
  restaurantId: number;
  description: string;
  discountPercentage: number;
};

type LunchDeal = {
  id: number;
  restaurantId: number;
  name: string;
  description: string | null;
  price: number;
  startTime: string;
  endTime: string;
  daysOfWeek: string;
};

type SubmitRatingRequest = {
  userId: string;
  rating: number;
  review?: string | null;
};
```

## Existing API routes

Do not call these endpoints in the Lovable preview unless an API base URL is explicitly configured. Create a small data-access layer so mock data can later be replaced with these calls.

```text
GET  /api/restaurants
GET  /api/restaurants/:id
GET  /api/restaurants/search?query=...
GET  /api/restaurants/nearby?latitude=...&longitude=...&radiusKm=10
POST /api/restaurants/filter

GET  /api/restaurants/:id/studentdiscounts
GET  /api/restaurants/:id/lunchdeals
GET  /api/restaurants/:id/dishes

GET  /api/restaurants/:id/ratings/average
POST /api/restaurants/:id/ratings

GET  /api/dishes/:id/ratings/average
POST /api/dishes/:id/ratings
```

## Implementation constraints

- Keep business/data logic out of visual components.
- Put data access behind one adapter/service so it is easy to port.
- Components should be small and reusable.
- Preserve the route structure and field names above.
- Provide responsive states for approximately 360 px, 390 px, 768 px, and desktop widths.
- Support keyboard interaction for stars, filters, links and buttons.
- Do not invent login, payments, ordering, delivery or reservations.
- Do not present generated discounts, lunch deals or dish data as verified restaurant information.

## Expected output

Deliver a complete frontend prototype with:

- all three routes;
- reusable components;
- mock data wired through a data adapter;
- polished loading, error and empty states;
- a short README explaining how to run it;
- no backend implementation.

After the design is exported, it will be given to another developer to translate the visual components into Blazor/Razor while keeping the existing .NET API.
