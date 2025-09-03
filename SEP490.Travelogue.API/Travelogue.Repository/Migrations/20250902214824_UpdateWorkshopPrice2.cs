using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkshopPrice2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("64922192-6816-4c7c-bd55-6ba7e7536233"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("be6ed906-935c-47ad-ab11-a0bfb6946929"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("afd87827-5fef-4cb1-9276-54451f3ab28d"));

            migrationBuilder.CreateTable(
                name: "workshop_ticket_price_updates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ticket_type_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    old_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    new_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    request_reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    moderator_note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requested_by = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    decided_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    decided_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_workshop_ticket_price_updates", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("90e1106e-4014-4388-b754-35da0cd33741"), "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1815), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1824), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("c78b42b6-ff36-471f-b519-927c4218c3f8"), "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1834), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1835), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("349286b7-9619-46b8-87f1-64d41bc45622"), null, new DateTimeOffset(new DateTime(2025, 9, 3, 4, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1306), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 9, 3, 4, 48, 22, 416, DateTimeKind.Unspecified).AddTicks(1405), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workshop_ticket_price_updates");

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("90e1106e-4014-4388-b754-35da0cd33741"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("c78b42b6-ff36-471f-b519-927c4218c3f8"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("349286b7-9619-46b8-87f1-64d41bc45622"));

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("64922192-6816-4c7c-bd55-6ba7e7536233"), "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(7915), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(7936), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("be6ed906-935c-47ad-ab11-a0bfb6946929"), "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(7961), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 2, 21, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(7963), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("afd87827-5fef-4cb1-9276-54451f3ab28d"), null, new DateTimeOffset(new DateTime(2025, 9, 3, 4, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(1425), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 9, 3, 4, 33, 50, 307, DateTimeKind.Unspecified).AddTicks(5741), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
