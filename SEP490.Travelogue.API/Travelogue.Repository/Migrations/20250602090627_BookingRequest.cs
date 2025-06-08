using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class BookingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_plan_share_trip_plans_trip_plan_id",
                table: "trip_plan_share");

            migrationBuilder.DropForeignKey(
                name: "FK_trip_plan_share_users_user_id",
                table: "trip_plan_share");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trip_plan_share",
                table: "trip_plan_share");

            migrationBuilder.RenameTable(
                name: "trip_plan_share",
                newName: "trip_plan_shares");

            migrationBuilder.RenameIndex(
                name: "IX_trip_plan_share_user_id",
                table: "trip_plan_shares",
                newName: "IX_trip_plan_shares_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_trip_plan_share_trip_plan_id",
                table: "trip_plan_shares",
                newName: "IX_trip_plan_shares_trip_plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trip_plan_shares",
                table: "trip_plan_shares",
                column: "id");

            migrationBuilder.CreateTable(
                name: "tour_guide_booking_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    requested_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    user_responded_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_response_message = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_tour_guide_booking_requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_tour_guide_booking_requests_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_guide_booking_requests_trip_plan_versions_trip_plan_ver~",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_guide_booking_requests_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tour_guide_booking_requests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_booking_requests_tour_guide_id",
                table: "tour_guide_booking_requests",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_booking_requests_trip_plan_id",
                table: "tour_guide_booking_requests",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_booking_requests_trip_plan_version_id",
                table: "tour_guide_booking_requests",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_tour_guide_booking_requests_user_id",
                table: "tour_guide_booking_requests",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_plan_shares_trip_plans_trip_plan_id",
                table: "trip_plan_shares",
                column: "trip_plan_id",
                principalTable: "trip_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trip_plan_shares_users_user_id",
                table: "trip_plan_shares",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trip_plan_shares_trip_plans_trip_plan_id",
                table: "trip_plan_shares");

            migrationBuilder.DropForeignKey(
                name: "FK_trip_plan_shares_users_user_id",
                table: "trip_plan_shares");

            migrationBuilder.DropTable(
                name: "tour_guide_booking_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trip_plan_shares",
                table: "trip_plan_shares");

            migrationBuilder.RenameTable(
                name: "trip_plan_shares",
                newName: "trip_plan_share");

            migrationBuilder.RenameIndex(
                name: "IX_trip_plan_shares_user_id",
                table: "trip_plan_share",
                newName: "IX_trip_plan_share_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_trip_plan_shares_trip_plan_id",
                table: "trip_plan_share",
                newName: "IX_trip_plan_share_trip_plan_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trip_plan_share",
                table: "trip_plan_share",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_trip_plan_share_trip_plans_trip_plan_id",
                table: "trip_plan_share",
                column: "trip_plan_id",
                principalTable: "trip_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trip_plan_share_users_user_id",
                table: "trip_plan_share",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
