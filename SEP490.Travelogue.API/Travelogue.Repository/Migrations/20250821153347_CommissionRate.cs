using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CommissionRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_settings",
                keyColumn: "id",
                keyValue: new Guid("c8bf29e3-ec7b-417d-938a-c9a4749a7b63"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("293e5e5d-ca3e-43d2-8457-54e704fb51d6"));

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "commission_settings",
                newName: "tour_guide_end_date");

            migrationBuilder.RenameColumn(
                name: "effective_date",
                table: "commission_settings",
                newName: "tour_guide_effective_date");

            migrationBuilder.AddColumn<DateTime>(
                name: "craft_village_effective_date",
                table: "commission_settings",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "craft_village_end_date",
                table: "commission_settings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "commission_rates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type = table.Column<int>(type: "int", nullable: false),
                    rate_value = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    effective_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_commission_rates", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("1d63137d-5315-4565-b3b6-4cff5d73baf5"), "System", new DateTimeOffset(new DateTime(2025, 8, 21, 15, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5758), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 21, 15, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5764), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("547efa98-ecb9-4e50-a1ea-335efcb24c34"), "System", new DateTimeOffset(new DateTime(2025, 8, 21, 15, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5769), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 21, 15, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5770), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("5eda02e6-5a47-468a-9eb9-9f446174d00a"), null, new DateTimeOffset(new DateTime(2025, 8, 21, 22, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5309), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 21, 22, 33, 45, 504, DateTimeKind.Unspecified).AddTicks(5363), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commission_rates");

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("5eda02e6-5a47-468a-9eb9-9f446174d00a"));

            migrationBuilder.DropColumn(
                name: "craft_village_effective_date",
                table: "commission_settings");

            migrationBuilder.DropColumn(
                name: "craft_village_end_date",
                table: "commission_settings");

            migrationBuilder.RenameColumn(
                name: "tour_guide_end_date",
                table: "commission_settings",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "tour_guide_effective_date",
                table: "commission_settings",
                newName: "effective_date");

            migrationBuilder.InsertData(
                table: "commission_settings",
                columns: new[] { "id", "craft_village_commission_rate", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "end_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "tour_guide_commission_rate" },
                values: new object[] { new Guid("c8bf29e3-ec7b-417d-938a-c9a4749a7b63"), 10m, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9860), new TimeSpan(0, 7, 0, 0, 0)), null, null, new DateTime(2024, 8, 21, 21, 18, 4, 677, DateTimeKind.Local).AddTicks(9875), null, true, false, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9865), new TimeSpan(0, 7, 0, 0, 0)), 20m });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("293e5e5d-ca3e-43d2-8457-54e704fb51d6"), null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9368), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 21, 21, 18, 4, 677, DateTimeKind.Unspecified).AddTicks(9457), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
