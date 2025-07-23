using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitDb5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "distance_from_prev",
                table: "trip_plan_locations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "estimated_end_time",
                table: "trip_plan_locations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "estimated_start_time",
                table: "trip_plan_locations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "travel_time_from_prev",
                table: "trip_plan_locations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "locations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "distance_from_prev",
                table: "trip_plan_locations");

            migrationBuilder.DropColumn(
                name: "estimated_end_time",
                table: "trip_plan_locations");

            migrationBuilder.DropColumn(
                name: "estimated_start_time",
                table: "trip_plan_locations");

            migrationBuilder.DropColumn(
                name: "travel_time_from_prev",
                table: "trip_plan_locations");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "locations");
        }
    }
}
