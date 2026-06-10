using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LunchNear.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RestaurantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DishId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Review = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishRatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                    PriceRange = table.Column<string>(type: "TEXT", nullable: false),
                    HasLunchBuffet = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasStudentDiscount = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountPercentage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDiscounts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Description", "Name", "Price", "Rating", "RestaurantId" },
                values: new object[,]
                {
                    { 1, "Classic flame-grilled burger", "Whopper", 8.99m, 0m, 1 },
                    { 2, "Crispy chicken sandwich", "Chicken Sandwich", 7.99m, 0m, 1 },
                    { 3, "Classic Italian pizza", "Margherita Pizza", 12.99m, 0m, 2 },
                    { 4, "Four cheese pizza", "Quattro Formaggi", 14.99m, 0m, 2 },
                    { 5, "Fresh salmon sushi roll", "Salmon Roll", 15.99m, 0m, 3 },
                    { 6, "Tempura shrimp and avocado", "Dragon Roll", 16.99m, 0m, 3 }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Address", "HasLunchBuffet", "HasStudentDiscount", "Name", "PriceRange", "Rating" },
                values: new object[,]
                {
                    { 1, "Avenyn, Gothenburg", false, true, "Burger King", "Cheap", 3.8m },
                    { 2, "Magasinsgatan, Gothenburg", true, true, "Pizzeria Marco", "Medium", 4.2m },
                    { 3, "Kungsgatan, Gothenburg", true, false, "Sushi Paradise", "Expensive", 4.5m },
                    { 4, "Nils Ericson Plats, Gothenburg", false, true, "Tacos El Amigo", "Cheap", 4.1m },
                    { 5, "Jarntorget, Gothenburg", true, true, "Thai Kitchen", "Medium", 4.3m },
                    { 6, "Storgatan, Gothenburg", false, false, "Steakhouse Premium", "Expensive", 4.7m },
                    { 7, "Heden, Gothenburg", false, true, "Kebab House", "Cheap", 3.9m },
                    { 8, "Vasaplatsen, Gothenburg", true, false, "Pasta Perfetto", "Medium", 4.4m }
                });

            migrationBuilder.InsertData(
                table: "StudentDiscounts",
                columns: new[] { "Id", "Description", "DiscountPercentage", "RestaurantId" },
                values: new object[,]
                {
                    { 1, "20% off with valid student ID", 20, 1 },
                    { 2, "Buy one pizza get second 50% off", 50, 2 },
                    { 3, "15% student discount with ID", 15, 4 },
                    { 4, "10% off for students every day", 10, 5 },
                    { 5, "Free drink with student discount", 5, 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "DishRatings");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropTable(
                name: "StudentDiscounts");
        }
    }
}
