using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CommissionSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("74326f7d-23af-48ec-b280-f57b853fab6e"));

            migrationBuilder.RenameColumn(
                name: "rejection_reason",
                table: "refund_requests",
                newName: "note");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "requested_at",
                table: "refund_requests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "responded_at",
                table: "refund_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "commission_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_commission_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    craft_village_commission_rate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    effective_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commission_settings", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_settings",
                columns: new[] { "id", "craft_village_commission_rate", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "tour_guide_commission_rate" },
                values: new object[] { new Guid("b7afd366-c7e4-44b3-ab7e-b222e549e697"), 10m, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Unspecified).AddTicks(671), new TimeSpan(0, 7, 0, 0, 0)), null, null, new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Local).AddTicks(690), true, false, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 254, DateTimeKind.Unspecified).AddTicks(676), new TimeSpan(0, 7, 0, 0, 0)), 20m });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("8704f52d-758f-4084-a772-6be54f5c9ad3"), null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 253, DateTimeKind.Unspecified).AddTicks(9560), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 20, 9, 47, 26, 253, DateTimeKind.Unspecified).AddTicks(9808), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_settings");

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("8704f52d-758f-4084-a772-6be54f5c9ad3"));

            migrationBuilder.DropColumn(
                name: "requested_at",
                table: "refund_requests");

            migrationBuilder.DropColumn(
                name: "responded_at",
                table: "refund_requests");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "refund_requests",
                newName: "rejection_reason");

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("74326f7d-23af-48ec-b280-f57b853fab6e"), null, new DateTimeOffset(new DateTime(2025, 8, 19, 17, 11, 39, 366, DateTimeKind.Unspecified).AddTicks(8534), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 19, 17, 11, 39, 366, DateTimeKind.Unspecified).AddTicks(8616), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
