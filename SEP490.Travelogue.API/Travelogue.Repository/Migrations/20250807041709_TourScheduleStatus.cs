using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TourScheduleStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_trip_plan_version_id",
                table: "trip_plans");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "trip_plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "tour_schedules",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "tour_schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "trip_plans");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "tour_schedules");

            migrationBuilder.DropColumn(
                name: "status",
                table: "tour_schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "user_trip_plan_version_id",
                table: "trip_plans",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }
    }
}
