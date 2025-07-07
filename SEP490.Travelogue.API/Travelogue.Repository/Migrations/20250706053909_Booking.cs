using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refund_requests_orders_order_id",
                table: "refund_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_group_members_orders_order_id",
                table: "tour_group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_join_requests_orders_from_order_id",
                table: "tour_join_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_join_requests_orders_to_order_id",
                table: "tour_join_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_schedules_tours_tour_id",
                table: "tour_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_orders_order_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "location_hotel_suggestions");

            migrationBuilder.DropTable(
                name: "order_withdrawals");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropTable(
                name: "hotels");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "transactions",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_order_id",
                table: "transactions",
                newName: "IX_transactions_booking_id");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "tour_plan_versions",
                newName: "children_price");

            migrationBuilder.RenameColumn(
                name: "to_order_id",
                table: "tour_join_requests",
                newName: "to_booking_id");

            migrationBuilder.RenameColumn(
                name: "from_order_id",
                table: "tour_join_requests",
                newName: "from_booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_join_requests_to_order_id",
                table: "tour_join_requests",
                newName: "IX_tour_join_requests_to_booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_join_requests_from_order_id",
                table: "tour_join_requests",
                newName: "IX_tour_join_requests_from_booking_id");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "tour_group_members",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_group_members_order_id",
                table: "tour_group_members",
                newName: "IX_tour_group_members_booking_id");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "refund_requests",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_refund_requests_order_id",
                table: "refund_requests",
                newName: "IX_refund_requests_booking_id");

            migrationBuilder.AddColumn<Guid>(
                name: "tour_guide_id",
                table: "tours",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "tour_id",
                table: "tour_schedules",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "total_days",
                table: "tour_schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "tour_plan_version_id",
                table: "tour_schedules",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<decimal>(
                name: "adult_price",
                table: "tour_plan_versions",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "tour_plan_versions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "current_version_id",
                table: "tour_plan_versions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "total_days",
                table: "tour_plan_versions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "tour_type_id",
                table: "tour_plan_versions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "tour_type_text",
                table: "tour_plan_versions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "booking_id",
                table: "tour_guide_schedules",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "file_type",
                table: "location_medias",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "promotions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    promotion_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    discount_type = table.Column<int>(type: "int", nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    applicable_type = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_promotions", x => x.id);
                    table.ForeignKey(
                        name: "FK_promotions_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_guide_mapping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_tour_guide_mapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_guide_mapping_tour_guides_guide_id",
                        column: x => x.guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_guide_mapping_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_schedule_guide",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_schedule_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_tour_schedule_guide", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_schedule_guide_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_schedule_guide_tour_schedules_tour_schedule_id",
                        column: x => x.tour_schedule_id,
                        principalTable: "tour_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_schedule_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    payment_link_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    booking_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    cancelled_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    is_open_to_join = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    promotion_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    original_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    final_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_bookings", x => x.id);
                    table.ForeignKey(
                        name: "FK_bookings_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_tour_plan_versions_tour_plan_version_id",
                        column: x => x.tour_plan_version_id,
                        principalTable: "tour_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bookings_tour_schedules_tour_schedule_id",
                        column: x => x.tour_schedule_id,
                        principalTable: "tour_schedules",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_bookings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "promotion_applicable",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    promotion_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    service_type = table.Column<int>(type: "int", nullable: false),
                    guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_promotion_applicable", x => x.id);
                    table.ForeignKey(
                        name: "FK_promotion_applicable_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promotion_applicable_tour_guides_guide_id",
                        column: x => x.guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_promotion_applicable_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "booking_participant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    booking_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    type = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    price_per_participant = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_booking_participant", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_participant_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "booking_withdrawals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    booking_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    withdrawal_request_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_booking_withdrawals", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_withdrawals_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_withdrawals_withdrawal_requests_withdrawal_request_id",
                        column: x => x.withdrawal_request_id,
                        principalTable: "withdrawal_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tours_tour_guide_id",
                table: "tours",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedules_tour_plan_version_id",
                table: "tour_schedules",
                column: "tour_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_schedules_booking_id",
                table: "tour_guide_schedules",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_participant_booking_id",
                table: "booking_participant",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_withdrawals_booking_id",
                table: "booking_withdrawals",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_withdrawals_withdrawal_request_id",
                table: "booking_withdrawals",
                column: "withdrawal_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_promotion_id",
                table: "bookings",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_tour_guide_id",
                table: "bookings",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_tour_id",
                table: "bookings",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_tour_plan_version_id",
                table: "bookings",
                column: "tour_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_tour_schedule_id",
                table: "bookings",
                column: "tour_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_trip_plan_id",
                table: "bookings",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_trip_plan_version_id",
                table: "bookings",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_user_id",
                table: "bookings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_applicable_guide_id",
                table: "promotion_applicable",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_applicable_promotion_id",
                table: "promotion_applicable",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotion_applicable_tour_id",
                table: "promotion_applicable",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_tour_id",
                table: "promotions",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_promotions_user_id",
                table: "promotions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_mapping_guide_id",
                table: "tour_guide_mapping",
                column: "guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_mapping_tour_id",
                table: "tour_guide_mapping",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_guide_tour_guide_id",
                table: "tour_schedule_guide",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_schedule_guide_tour_schedule_id",
                table: "tour_schedule_guide",
                column: "tour_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refund_requests_bookings_booking_id",
                table: "refund_requests",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_group_members_bookings_booking_id",
                table: "tour_group_members",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_guide_schedules_bookings_booking_id",
                table: "tour_guide_schedules",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tour_join_requests_bookings_from_booking_id",
                table: "tour_join_requests",
                column: "from_booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_join_requests_bookings_to_booking_id",
                table: "tour_join_requests",
                column: "to_booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_schedules_tour_plan_versions_tour_plan_version_id",
                table: "tour_schedules",
                column: "tour_plan_version_id",
                principalTable: "tour_plan_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_schedules_tours_tour_id",
                table: "tour_schedules",
                column: "tour_id",
                principalTable: "tours",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tours_tour_guides_tour_guide_id",
                table: "tours",
                column: "tour_guide_id",
                principalTable: "tour_guides",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_bookings_booking_id",
                table: "transactions",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refund_requests_bookings_booking_id",
                table: "refund_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_group_members_bookings_booking_id",
                table: "tour_group_members");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_guide_schedules_bookings_booking_id",
                table: "tour_guide_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_join_requests_bookings_from_booking_id",
                table: "tour_join_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_join_requests_bookings_to_booking_id",
                table: "tour_join_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_schedules_tour_plan_versions_tour_plan_version_id",
                table: "tour_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_tour_schedules_tours_tour_id",
                table: "tour_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_tours_tour_guides_tour_guide_id",
                table: "tours");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_bookings_booking_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "booking_participant");

            migrationBuilder.DropTable(
                name: "booking_withdrawals");

            migrationBuilder.DropTable(
                name: "promotion_applicable");

            migrationBuilder.DropTable(
                name: "tour_guide_mapping");

            migrationBuilder.DropTable(
                name: "tour_schedule_guide");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "promotions");

            migrationBuilder.DropIndex(
                name: "IX_tours_tour_guide_id",
                table: "tours");

            migrationBuilder.DropIndex(
                name: "IX_tour_schedules_tour_plan_version_id",
                table: "tour_schedules");

            migrationBuilder.DropIndex(
                name: "IX_tour_guide_schedules_booking_id",
                table: "tour_guide_schedules");

            migrationBuilder.DropColumn(
                name: "tour_guide_id",
                table: "tours");

            migrationBuilder.DropColumn(
                name: "total_days",
                table: "tour_schedules");

            migrationBuilder.DropColumn(
                name: "tour_plan_version_id",
                table: "tour_schedules");

            migrationBuilder.DropColumn(
                name: "adult_price",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "content",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "current_version_id",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "total_days",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "tour_type_id",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "tour_type_text",
                table: "tour_plan_versions");

            migrationBuilder.DropColumn(
                name: "booking_id",
                table: "tour_guide_schedules");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "transactions",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_booking_id",
                table: "transactions",
                newName: "IX_transactions_order_id");

            migrationBuilder.RenameColumn(
                name: "children_price",
                table: "tour_plan_versions",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "to_booking_id",
                table: "tour_join_requests",
                newName: "to_order_id");

            migrationBuilder.RenameColumn(
                name: "from_booking_id",
                table: "tour_join_requests",
                newName: "from_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_join_requests_to_booking_id",
                table: "tour_join_requests",
                newName: "IX_tour_join_requests_to_order_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_join_requests_from_booking_id",
                table: "tour_join_requests",
                newName: "IX_tour_join_requests_from_order_id");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "tour_group_members",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_group_members_booking_id",
                table: "tour_group_members",
                newName: "IX_tour_group_members_order_id");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "refund_requests",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_refund_requests_booking_id",
                table: "refund_requests",
                newName: "IX_refund_requests_order_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "tour_id",
                table: "tour_schedules",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "location_medias",
                keyColumn: "file_type",
                keyValue: null,
                column: "file_type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "file_type",
                table: "location_medias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "hotels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    email = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    phone_number = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price_per_night = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    website = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotels", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotels_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    cancelled_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_open_to_join = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    order_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    scheduled_end_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    scheduled_start_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    total_paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tour_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_tour_plan_versions_tour_plan_version_id",
                        column: x => x.tour_plan_version_id,
                        principalTable: "tour_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_orders_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vouchers", x => x.id);
                    table.ForeignKey(
                        name: "FK_vouchers_tours_tour_id",
                        column: x => x.tour_id,
                        principalTable: "tours",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vouchers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_hotel_suggestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    hotel_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    location_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_hotel_suggestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_hotel_suggestions_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_hotel_suggestions_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_withdrawals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    withdrawal_request_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_withdrawals", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_withdrawals_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_withdrawals_withdrawal_requests_withdrawal_request_id",
                        column: x => x.withdrawal_request_id,
                        principalTable: "withdrawal_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_hotels_location_id",
                table: "hotels",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_location_hotel_suggestions_hotel_id",
                table: "location_hotel_suggestions",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_hotel_suggestions_location_id",
                table: "location_hotel_suggestions",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_withdrawals_order_id",
                table: "order_withdrawals",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_withdrawals_withdrawal_request_id",
                table: "order_withdrawals",
                column: "withdrawal_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tour_guide_id",
                table: "orders",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tour_id",
                table: "orders",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tour_plan_version_id",
                table: "orders",
                column: "tour_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_id",
                table: "orders",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_version_id",
                table: "orders",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_tour_id",
                table: "vouchers",
                column: "tour_id");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_user_id",
                table: "vouchers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refund_requests_orders_order_id",
                table: "refund_requests",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_group_members_orders_order_id",
                table: "tour_group_members",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_join_requests_orders_from_order_id",
                table: "tour_join_requests",
                column: "from_order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_join_requests_orders_to_order_id",
                table: "tour_join_requests",
                column: "to_order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tour_schedules_tours_tour_id",
                table: "tour_schedules",
                column: "tour_id",
                principalTable: "tours",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_orders_order_id",
                table: "transactions",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
