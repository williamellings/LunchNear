using LunchNear.API.Services;
using LunchNear.API.Data;
using LunchNear.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=lunchnear.db";
builder.Services.AddDbContext<LunchNearDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishRatingService, DishRatingService>();
builder.Services.AddScoped<IStudentDiscountService, StudentDiscountService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
