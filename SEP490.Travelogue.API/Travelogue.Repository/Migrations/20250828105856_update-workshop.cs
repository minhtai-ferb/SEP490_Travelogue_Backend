using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateworkshop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_workshop_activities_workshops_workshop_id",
                table: "workshop_activities");

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("1d63137d-5315-4565-b3b6-4cff5d73baf5"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("547efa98-ecb9-4e50-a1ea-335efcb24c34"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("5eda02e6-5a47-468a-9eb9-9f446174d00a"));

            migrationBuilder.DropColumn(
                name: "adult_price",
                table: "workshop_schedules");

            migrationBuilder.DropColumn(
                name: "children_price",
                table: "workshop_schedules");

            migrationBuilder.DropColumn(
                name: "end_time",
                table: "workshop_activities");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "workshop_activities");

            migrationBuilder.DropColumn(
                name: "start_time",
                table: "workshop_activities");

            migrationBuilder.RenameColumn(
                name: "max_participant",
                table: "workshop_schedules",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "workshop_id",
                table: "workshop_activities",
                newName: "workshop_ticket_type_id");

            migrationBuilder.RenameColumn(
                name: "day_order",
                table: "workshop_activities",
                newName: "activity_order");

            migrationBuilder.RenameIndex(
                name: "IX_workshop_activities_workshop_id",
                table: "workshop_activities",
                newName: "IX_workshop_activities_workshop_ticket_type_id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "workshops",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "workshops",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "capacity",
                table: "workshop_schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "end_hour",
                table: "workshop_activities",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "start_hour",
                table: "workshop_activities",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "workshop_id",
                table: "craft_village_requests",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "workshop_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    craft_village_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_workshop_request", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_ticket_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    is_combo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_workshop_ticket_type", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_ticket_type_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_exception_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    workshop_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workshop_exception_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_exception_request_workshop_request_workshop_request~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_media_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    workshop_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_workshop_media_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_media_request_workshop_request_workshop_request_id",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_recurring_rule_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    days_of_week = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    workshop_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_workshop_recurring_rule_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_recurring_rule_request_workshop_request_workshop_re~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_ticket_type_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    is_combo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    duration_minutes = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    workshop_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_workshop_ticket_type_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_ticket_type_request_workshop_request_workshop_reque~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_session_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    workshop_recurring_rule_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_workshop_session_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_session_request_workshop_recurring_rule_request_wor~",
                        column: x => x.workshop_recurring_rule_request_id,
                        principalTable: "workshop_recurring_rule_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_activity_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    activity = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_hour = table.Column<double>(type: "double", nullable: false),
                    end_hour = table.Column<double>(type: "double", nullable: false),
                    activity_order = table.Column<int>(type: "int", nullable: false),
                    workshop_ticket_type_request_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_workshop_activity_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_activity_request_workshop_ticket_type_request_works~",
                        column: x => x.workshop_ticket_type_request_id,
                        principalTable: "workshop_ticket_type_request",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("92b50155-f384-4a9b-843f-356b85165586"), "System", new DateTimeOffset(new DateTime(2025, 8, 28, 10, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7928), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 28, 10, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7933), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 },
                    { new Guid("fdb75c2c-73cc-4f32-837e-d7a3888fc199"), "System", new DateTimeOffset(new DateTime(2025, 8, 28, 10, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7939), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 28, 10, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7940), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("122a5a24-b0d8-486d-9b11-d22cf1994512"), null, new DateTimeOffset(new DateTime(2025, 8, 28, 17, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7621), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 28, 17, 58, 55, 634, DateTimeKind.Unspecified).AddTicks(7679), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });

            migrationBuilder.CreateIndex(
                name: "IX_craft_village_requests_workshop_id",
                table: "craft_village_requests",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_activity_request_workshop_ticket_type_request_id",
                table: "workshop_activity_request",
                column: "workshop_ticket_type_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_exception_request_workshop_request_id",
                table: "workshop_exception_request",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_media_request_workshop_request_id",
                table: "workshop_media_request",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_recurring_rule_request_workshop_request_id",
                table: "workshop_recurring_rule_request",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_session_request_workshop_recurring_rule_request_id",
                table: "workshop_session_request",
                column: "workshop_recurring_rule_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_ticket_type_workshop_id",
                table: "workshop_ticket_type",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_ticket_type_request_workshop_request_id",
                table: "workshop_ticket_type_request",
                column: "workshop_request_id");

            migrationBuilder.AddForeignKey(
                name: "FK_craft_village_requests_workshop_request_workshop_id",
                table: "craft_village_requests",
                column: "workshop_id",
                principalTable: "workshop_request",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_workshop_activities_workshop_ticket_type_workshop_ticket_typ~",
                table: "workshop_activities",
                column: "workshop_ticket_type_id",
                principalTable: "workshop_ticket_type",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_craft_village_requests_workshop_request_workshop_id",
                table: "craft_village_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_workshop_activities_workshop_ticket_type_workshop_ticket_typ~",
                table: "workshop_activities");

            migrationBuilder.DropTable(
                name: "workshop_activity_request");

            migrationBuilder.DropTable(
                name: "workshop_exception_request");

            migrationBuilder.DropTable(
                name: "workshop_media_request");

            migrationBuilder.DropTable(
                name: "workshop_session_request");

            migrationBuilder.DropTable(
                name: "workshop_ticket_type");

            migrationBuilder.DropTable(
                name: "workshop_ticket_type_request");

            migrationBuilder.DropTable(
                name: "workshop_recurring_rule_request");

            migrationBuilder.DropTable(
                name: "workshop_request");

            migrationBuilder.DropIndex(
                name: "IX_craft_village_requests_workshop_id",
                table: "craft_village_requests");

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("92b50155-f384-4a9b-843f-356b85165586"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("fdb75c2c-73cc-4f32-837e-d7a3888fc199"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("122a5a24-b0d8-486d-9b11-d22cf1994512"));

            migrationBuilder.DropColumn(
                name: "capacity",
                table: "workshop_schedules");

            migrationBuilder.DropColumn(
                name: "end_hour",
                table: "workshop_activities");

            migrationBuilder.DropColumn(
                name: "start_hour",
                table: "workshop_activities");

            migrationBuilder.DropColumn(
                name: "workshop_id",
                table: "craft_village_requests");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "workshop_schedules",
                newName: "max_participant");

            migrationBuilder.RenameColumn(
                name: "workshop_ticket_type_id",
                table: "workshop_activities",
                newName: "workshop_id");

            migrationBuilder.RenameColumn(
                name: "activity_order",
                table: "workshop_activities",
                newName: "day_order");

            migrationBuilder.RenameIndex(
                name: "IX_workshop_activities_workshop_ticket_type_id",
                table: "workshop_activities",
                newName: "IX_workshop_activities_workshop_id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "workshops",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "workshops",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "adult_price",
                table: "workshop_schedules",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "children_price",
                table: "workshop_schedules",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_time",
                table: "workshop_activities",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "workshop_activities",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_time",
                table: "workshop_activities",
                type: "time(6)",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_workshop_activities_workshops_workshop_id",
                table: "workshop_activities",
                column: "workshop_id",
                principalTable: "workshops",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
