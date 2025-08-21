using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class EndDateCommission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_settings",
                keyColumn: "id",
                keyValue: new Guid("b7afd366-c7e4-44b3-ab7e-b222e549e697"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("8704f52d-758f-4084-a772-6be54f5c9ad3"));

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "commission_settings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "commission_settings",
                columns: new[] { "id", "craft_village_commission_rate", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "end_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "tour_guide_commission_rate" },
                values: new object[] { new Guid("c8bf29e3-ec7b-417d-938a-c9a4749a7b63"), 10m, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9860), new TimeSpan(0, 7, 0, 0, 0)), null, null, new DateTime(2024, 8, 21, 21, 18, 4, 677, DateTimeKind.Local).AddTicks(9875), null, true, false, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9865), new TimeSpan(0, 7, 0, 0, 0)), 20m });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("293e5e5d-ca3e-43d2-8457-54e704fb51d6"), null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9368), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9457), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_settings",
                keyColumn: "id",
                keyValue: new Guid("c8bf29e3-ec7b-417d-938a-c9a4749a7b63"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("293e5e5d-ca3e-43d2-8457-54e704fb51d6"));

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "commission_settings");

            migrationBuilder.InsertData(
                table: "commission_settings",
                columns: new[] { "id", "craft_village_commission_rate", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "tour_guide_commission_rate" },
                values: new object[] { new Guid("b7afd366-c7e4-44b3-ab7e-b222e549e697"), 10m, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Unspecified).AddTicks(671), new TimeSpan(0, 7, 0, 0, 0)), null, null, new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Local).AddTicks(690), true, false, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Unspecified).AddTicks(676), new TimeSpan(0, 7, 0, 0, 0)), 20m });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("8704f52d-758f-4084-a772-6be54f5c9ad3"), null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 253, DateTimeKind.Unspecified).AddTicks(9560), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 253, DateTimeKind.Unspecified).AddTicks(9808), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
