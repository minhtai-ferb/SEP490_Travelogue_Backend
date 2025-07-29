using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class BookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_open_to_join",
                table: "bookings");

            migrationBuilder.AddColumn<string>(
                name: "promotion_code",
                table: "promotions",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_promotion_code",
                table: "promotions",
                column: "promotion_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_promotions_promotion_code",
                table: "promotions");

            migrationBuilder.DropColumn(
                name: "promotion_code",
                table: "promotions");

            migrationBuilder.AddColumn<bool>(
                name: "is_open_to_join",
                table: "bookings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
