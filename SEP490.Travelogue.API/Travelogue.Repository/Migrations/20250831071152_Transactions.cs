using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Transactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("3ab54eb2-cb40-4329-866e-a568cfe6b554"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("556efefa-cda2-4214-a399-9095ed419bea"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("a7e580e5-8942-436f-b1b7-8fc8f5b788ea"));

            migrationBuilder.DropColumn(
                name: "estimated_end_time",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "estimated_start_time",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "planned_end_time",
                table: "tour_plan_location_workshops");

            migrationBuilder.DropColumn(
                name: "planned_start_time",
                table: "tour_plan_location_workshops");

            migrationBuilder.AddColumn<int>(
                name: "channel",
                table: "transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "transactions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "system_kind",
                table: "transactions",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("db4f808d-6588-4d6f-abe2-5b8f9c4376c2"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2273), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2275), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("fc9e85ee-0603-4037-9f35-d0e2cc0db804"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2249), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2258), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("39c8465b-083a-49d3-af37-bfd3eab3f3ef"), null, new DateTimeOffset(new DateTime(2025, 8, 31, 14, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(1500), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 31, 14, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(1565), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("db4f808d-6588-4d6f-abe2-5b8f9c4376c2"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("fc9e85ee-0603-4037-9f35-d0e2cc0db804"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("39c8465b-083a-49d3-af37-bfd3eab3f3ef"));

            migrationBuilder.DropColumn(
                name: "channel",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "system_kind",
                table: "transactions");

            migrationBuilder.AddColumn<float>(
                name: "estimated_end_time",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "estimated_start_time",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "planned_end_time",
                table: "tour_plan_location_workshops",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "planned_start_time",
                table: "tour_plan_location_workshops",
                type: "time(6)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("3ab54eb2-cb40-4329-866e-a568cfe6b554"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 3, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(5289), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 3, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(5290), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("556efefa-cda2-4214-a399-9095ed419bea"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 3, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(5274), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 3, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(5281), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("a7e580e5-8942-436f-b1b7-8fc8f5b788ea"), null, new DateTimeOffset(new DateTime(2025, 8, 31, 10, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(4823), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 31, 10, 59, 11, 432, DateTimeKind.Unspecified).AddTicks(4879), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
