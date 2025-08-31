using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class LocationPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("b6c77e60-bc7a-4c0c-b11a-df1ffdb41273"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("ce13ce40-3593-4982-8892-da0fb35a6ab3"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("08affc58-3282-4cab-bc7f-736bae9493d0"));

            migrationBuilder.AddColumn<bool>(
                name: "is_tour_workshop",
                table: "tours",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "max_participants",
                table: "tour_guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "max_price",
                table: "locations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "min_price",
                table: "locations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "is_tour_workshop",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "max_participants",
                table: "tour_guides");

            migrationBuilder.DropColumn(
                name: "max_price",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "min_price",
                table: "locations");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("b6c77e60-bc7a-4c0c-b11a-df1ffdb41273"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(5145), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(5146), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("ce13ce40-3593-4982-8892-da0fb35a6ab3"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(5128), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(5136), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("08affc58-3282-4cab-bc7f-736bae9493d0"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(4672), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 50, 41, 445, DateTimeKind.Unspecified).AddTicks(4721), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
