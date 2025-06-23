using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TripPlanVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "trip_plan_version_id",
                table: "trip_plans",
                newName: "user_trip_plan_version_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_from_tour_guide",
                table: "trip_plan_versions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_from_tour_guide",
                table: "trip_plan_versions");

            migrationBuilder.RenameColumn(
                name: "user_trip_plan_version_id",
                table: "trip_plans",
                newName: "trip_plan_version_id");
        }
    }
}
