using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Booking2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tours_tour_guides_tour_guide_id",
                table: "tours");

            migrationBuilder.DropIndex(
                name: "IX_tours_tour_guide_id",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "tour_guide_id",
                table: "tours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tour_guide_id",
                table: "tours",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_tours_tour_guide_id",
                table: "tours",
                column: "tour_guide_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tours_tour_guides_tour_guide_id",
                table: "tours",
                column: "tour_guide_id",
                principalTable: "tour_guides",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
