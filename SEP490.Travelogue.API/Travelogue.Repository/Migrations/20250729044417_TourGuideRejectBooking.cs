using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TourGuideRejectBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tour_guide_schedules_tours_tour_id",
                table: "tour_guide_schedules");

            migrationBuilder.DropTable(
                name: "tour_guide_mapping");

            migrationBuilder.RenameColumn(
                name: "tour_id",
                table: "tour_guide_schedules",
                newName: "tour_schedule_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_guide_schedules_tour_id",
                table: "tour_guide_schedules",
                newName: "IX_tour_guide_schedules_tour_schedule_id");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "tour_guide_schedules",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "booking_price_request",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    rejection_reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reviewed_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_booking_price_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_booking_price_request_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rejection_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    request_type = table.Column<int>(type: "int", nullable: false),
                    tour_schedule_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    booking_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    moderator_comment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reviewed_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_rejection_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_rejection_requests_bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "bookings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_rejection_requests_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rejection_requests_tour_schedules_tour_schedule_id",
                        column: x => x.tour_schedule_id,
                        principalTable: "tour_schedules",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_booking_price_request_tour_guide_id",
                table: "booking_price_request",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_rejection_requests_booking_id",
                table: "rejection_requests",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_rejection_requests_tour_guide_id",
                table: "rejection_requests",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_rejection_requests_tour_schedule_id",
                table: "rejection_requests",
                column: "tour_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tour_guide_schedules_tour_schedules_tour_schedule_id",
                table: "tour_guide_schedules",
                column: "tour_schedule_id",
                principalTable: "tour_schedules",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tour_guide_schedules_tour_schedules_tour_schedule_id",
                table: "tour_guide_schedules");

            migrationBuilder.DropTable(
                name: "booking_price_request");

            migrationBuilder.DropTable(
                name: "rejection_requests");

            migrationBuilder.RenameColumn(
                name: "tour_schedule_id",
                table: "tour_guide_schedules",
                newName: "tour_id");

            migrationBuilder.RenameIndex(
                name: "IX_tour_guide_schedules_tour_schedule_id",
                table: "tour_guide_schedules",
                newName: "IX_tour_guide_schedules_tour_id");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "tour_guide_schedules",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tour_guide_mapping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_schedule_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_tour_guide_mapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_guide_mapping_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_guide_mapping_tour_schedules_tour_schedule_id",
                        column: x => x.tour_schedule_id,
                        principalTable: "tour_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_mapping_tour_guide_id",
                table: "tour_guide_mapping",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_mapping_tour_schedule_id",
                table: "tour_guide_mapping",
                column: "tour_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tour_guide_schedules_tours_tour_id",
                table: "tour_guide_schedules",
                column: "tour_id",
                principalTable: "tours",
                principalColumn: "id");
        }
    }
}
