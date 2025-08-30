using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TourPickUpAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("9d569f7b-3fb5-480a-9e72-29148e15898a"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("d7258da4-b955-442a-9daa-31ca4e1fcb6c"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("d971be9e-fad5-44e2-9562-f8a2729199d6"));

            migrationBuilder.AddColumn<string>(
                name: "pickup_address",
                table: "trip_plans",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "pickup_address",
                table: "tours",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "stay_info",
                table: "tours",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("561c28ef-8c23-4927-92a1-703dc14c6850"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2972), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2973), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("81b0d26e-9767-441f-8fd0-565f3a6fe29a"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2958), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2964), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("36bd181a-c83a-46ed-ae33-fbb7aaf5829c"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2512), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 16, 55, 137, DateTimeKind.Unspecified).AddTicks(2564), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("561c28ef-8c23-4927-92a1-703dc14c6850"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("81b0d26e-9767-441f-8fd0-565f3a6fe29a"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("36bd181a-c83a-46ed-ae33-fbb7aaf5829c"));

            migrationBuilder.DropColumn(
                name: "pickup_address",
                table: "trip_plans");

            migrationBuilder.DropColumn(
                name: "pickup_address",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "stay_info",
                table: "tours");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("9d569f7b-3fb5-480a-9e72-29148e15898a"), "System", new DateTimeOffset(new DateTime(2025, 8, 29, 0, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3680), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 29, 0, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3690), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("d7258da4-b955-442a-9daa-31ca4e1fcb6c"), "System", new DateTimeOffset(new DateTime(2025, 8, 29, 0, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3705), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 29, 0, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3707), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("d971be9e-fad5-44e2-9562-f8a2729199d6"), null, new DateTimeOffset(new DateTime(2025, 8, 29, 7, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3107), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 29, 7, 1, 22, 188, DateTimeKind.Unspecified).AddTicks(3163), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
