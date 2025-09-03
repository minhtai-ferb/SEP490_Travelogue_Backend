using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TourWithWorkshop : Migration
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

            migrationBuilder.CreateTable(
                name: "tour_plan_location_workshops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_plan_location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_ticket_type_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    workshop_session_rule_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    planned_start_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    planned_end_time = table.Column<TimeSpan>(type: "time(6)", nullable: true),
                    notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_tour_plan_location_workshops", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_plan_location_workshops_tour_plan_locations_tour_plan_l~",
                        column: x => x.tour_plan_location_id,
                        principalTable: "tour_plan_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_plan_location_workshops_workshop_session_rules_workshop~",
                        column: x => x.workshop_session_rule_id,
                        principalTable: "workshop_session_rules",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tour_plan_location_workshops_workshop_ticket_types_workshop_~",
                        column: x => x.workshop_ticket_type_id,
                        principalTable: "workshop_ticket_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tour_plan_location_workshops_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_schedule_workshops",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_schedule_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_plan_location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_schedule_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_ticket_type_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_tour_schedule_workshops", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_schedule_workshops_tour_plan_locations_tour_plan_locati~",
                        column: x => x.tour_plan_location_id,
                        principalTable: "tour_plan_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_schedule_workshops_tour_schedules_tour_schedule_id",
                        column: x => x.tour_schedule_id,
                        principalTable: "tour_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_schedule_workshops_workshop_schedules_workshop_schedule~",
                        column: x => x.workshop_schedule_id,
                        principalTable: "workshop_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_schedule_workshops_workshop_ticket_types_workshop_ticke~",
                        column: x => x.workshop_ticket_type_id,
                        principalTable: "workshop_ticket_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_tour_schedule_workshops_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("612606ff-ea3d-472c-ac90-14f1129ae07c"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5155), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5157), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("881b17a3-7cae-4b2f-a2a6-32955763d291"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5140), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5148), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("c55eb4a9-771c-41e8-aeea-b89de4b4dd9d"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 18, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(4609), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 18, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(4684), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_location_workshops_tour_plan_location_id",
                table: "tour_plan_location_workshops",
                column: "tour_plan_location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_location_workshops_workshop_id",
                table: "tour_plan_location_workshops",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_location_workshops_workshop_session_rule_id",
                table: "tour_plan_location_workshops",
                column: "workshop_session_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_plan_location_workshops_workshop_ticket_type_id",
                table: "tour_plan_location_workshops",
                column: "workshop_ticket_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_workshops_tour_plan_location_id",
                table: "tour_schedule_workshops",
                column: "tour_plan_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_workshops_tour_schedule_id",
                table: "tour_schedule_workshops",
                column: "tour_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_workshops_workshop_id",
                table: "tour_schedule_workshops",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_workshops_workshop_schedule_id",
                table: "tour_schedule_workshops",
                column: "workshop_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_workshops_workshop_ticket_type_id",
                table: "tour_schedule_workshops",
                column: "workshop_ticket_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tour_plan_location_workshops");

            migrationBuilder.DropTable(
                name: "tour_schedule_workshops");

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("612606ff-ea3d-472c-ac90-14f1129ae07c"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("881b17a3-7cae-4b2f-a2a6-32955763d291"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("c55eb4a9-771c-41e8-aeea-b89de4b4dd9d"));

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
