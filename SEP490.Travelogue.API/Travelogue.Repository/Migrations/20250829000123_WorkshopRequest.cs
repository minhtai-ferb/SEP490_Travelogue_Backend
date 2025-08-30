using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class WorkshopRequest : Migration
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

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_hour",
                table: "workshop_activities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_hour",
                table: "workshop_activities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "workshop_id",
                table: "craft_village_requests",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "workshop_exceptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_workshop_exceptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_exceptions_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_media_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    media_url = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_thumbnail = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_workshop_media_requests", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_recurring_rules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    workshop_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    days_of_week = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_workshop_recurring_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_recurring_rules_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_requests",
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
                    medias = table.Column<string>(type: "longtext", nullable: false)
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
                    table.PrimaryKey("PK_workshop_requests", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_ticket_types",
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
                    table.PrimaryKey("PK_workshop_ticket_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_ticket_types_workshops_workshop_id",
                        column: x => x.workshop_id,
                        principalTable: "workshops",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_session_rules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    recurring_rule_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_workshop_session_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_session_rules_workshop_recurring_rules_recurring_ru~",
                        column: x => x.recurring_rule_id,
                        principalTable: "workshop_recurring_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_exception_requests",
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
                    table.PrimaryKey("PK_workshop_exception_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_exception_requests_workshop_requests_workshop_reque~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_requests",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_recurring_rule_requests",
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
                    table.PrimaryKey("PK_workshop_recurring_rule_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_recurring_rule_requests_workshop_requests_workshop_~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_requests",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_ticket_type_requests",
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
                    table.PrimaryKey("PK_workshop_ticket_type_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_ticket_type_requests_workshop_requests_workshop_req~",
                        column: x => x.workshop_request_id,
                        principalTable: "workshop_requests",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_session_requests",
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
                    table.PrimaryKey("PK_workshop_session_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_session_requests_workshop_recurring_rule_requests_w~",
                        column: x => x.workshop_recurring_rule_request_id,
                        principalTable: "workshop_recurring_rule_requests",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "workshop_activity_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    activity = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    start_hour = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    end_hour = table.Column<TimeSpan>(type: "time(6)", nullable: false),
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
                    table.PrimaryKey("PK_workshop_activity_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_workshop_activity_requests_workshop_ticket_type_requests_wor~",
                        column: x => x.workshop_ticket_type_request_id,
                        principalTable: "workshop_ticket_type_requests",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_craft_village_requests_workshop_id",
                table: "craft_village_requests",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_activity_requests_workshop_ticket_type_request_id",
                table: "workshop_activity_requests",
                column: "workshop_ticket_type_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_exception_requests_workshop_request_id",
                table: "workshop_exception_requests",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_exceptions_workshop_id",
                table: "workshop_exceptions",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_recurring_rule_requests_workshop_request_id",
                table: "workshop_recurring_rule_requests",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_recurring_rules_workshop_id",
                table: "workshop_recurring_rules",
                column: "workshop_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_session_requests_workshop_recurring_rule_request_id",
                table: "workshop_session_requests",
                column: "workshop_recurring_rule_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_session_rules_recurring_rule_id",
                table: "workshop_session_rules",
                column: "recurring_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_ticket_type_requests_workshop_request_id",
                table: "workshop_ticket_type_requests",
                column: "workshop_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_workshop_ticket_types_workshop_id",
                table: "workshop_ticket_types",
                column: "workshop_id");

            migrationBuilder.AddForeignKey(
                name: "FK_craft_village_requests_workshop_requests_workshop_id",
                table: "craft_village_requests",
                column: "workshop_id",
                principalTable: "workshop_requests",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_workshop_activities_workshop_ticket_types_workshop_ticket_ty~",
                table: "workshop_activities",
                column: "workshop_ticket_type_id",
                principalTable: "workshop_ticket_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_craft_village_requests_workshop_requests_workshop_id",
                table: "craft_village_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_workshop_activities_workshop_ticket_types_workshop_ticket_ty~",
                table: "workshop_activities");

            migrationBuilder.DropTable(
                name: "workshop_activity_requests");

            migrationBuilder.DropTable(
                name: "workshop_exception_requests");

            migrationBuilder.DropTable(
                name: "workshop_exceptions");

            migrationBuilder.DropTable(
                name: "workshop_media_requests");

            migrationBuilder.DropTable(
                name: "workshop_session_requests");

            migrationBuilder.DropTable(
                name: "workshop_session_rules");

            migrationBuilder.DropTable(
                name: "workshop_ticket_types");

            migrationBuilder.DropTable(
                name: "workshop_ticket_type_requests");

            migrationBuilder.DropTable(
                name: "workshop_recurring_rule_requests");

            migrationBuilder.DropTable(
                name: "workshop_recurring_rules");

            migrationBuilder.DropTable(
                name: "workshop_requests");

            migrationBuilder.DropIndex(
                name: "IX_craft_village_requests_workshop_id",
                table: "craft_village_requests");

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
