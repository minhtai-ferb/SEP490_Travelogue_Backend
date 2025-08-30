using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TourGuideMaxParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("be07f7ca-e5e9-4dab-be6a-0227e8a16c5e"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("c22f74f6-14ee-4924-9572-5a97e65d399a"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("3cace061-b0e5-40ee-ba4d-278c358e9624"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("be07f7ca-e5e9-4dab-be6a-0227e8a16c5e"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(1343), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(1350), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("c22f74f6-14ee-4924-9572-5a97e65d399a"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(1397), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 14, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(1398), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("3cace061-b0e5-40ee-ba4d-278c358e9624"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(622), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 21, 40, 32, 167, DateTimeKind.Unspecified).AddTicks(716), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
