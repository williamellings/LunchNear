using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunchNear.API.Migrations
{
    /// <inheritdoc />
    public partial class TestRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StudentDiscounts_RestaurantId",
                table: "StudentDiscounts",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_DishRatings_DishId",
                table: "DishRatings",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId",
                table: "Dishes",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Restaurants_RestaurantId",
                table: "Dishes",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRatings_Dishes_DishId",
                table: "DishRatings",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDiscounts_Restaurants_RestaurantId",
                table: "StudentDiscounts",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Restaurants_RestaurantId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRatings_Dishes_DishId",
                table: "DishRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentDiscounts_Restaurants_RestaurantId",
                table: "StudentDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_StudentDiscounts_RestaurantId",
                table: "StudentDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_DishRatings_DishId",
                table: "DishRatings");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId",
                table: "Dishes");
        }
    }
}
