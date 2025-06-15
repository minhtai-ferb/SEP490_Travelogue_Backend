using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TripPlanExchangeSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tour_guide_booking_requests");

            migrationBuilder.CreateTable(
                name: "trip_plan_exchange_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_by_user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    final_status = table.Column<int>(type: "int", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_trip_plan_exchange_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchange_sessions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "trip_plan_exchanges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    suggested_trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    session_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    start_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    end_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    user_responded_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    user_response_message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
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
                    table.PrimaryKey("PK_trip_plan_exchanges", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_tour_guides_tour_guide_id",
                        column: x => x.tour_guide_id,
                        principalTable: "tour_guides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_exchange_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "trip_plan_exchange_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_versions_suggested_trip_plan_v~",
                        column: x => x.suggested_trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plan_versions_trip_plan_version_id",
                        column: x => x.trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_exchanges_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_created_by_user_id",
                table: "trip_plan_exchange_sessions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_tour_guide_id",
                table: "trip_plan_exchange_sessions",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchange_sessions_trip_plan_id",
                table: "trip_plan_exchange_sessions",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_session_id",
                table: "trip_plan_exchanges",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_suggested_trip_plan_version_id",
                table: "trip_plan_exchanges",
                column: "suggested_trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_tour_guide_id",
                table: "trip_plan_exchanges",
                column: "tour_guide_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_trip_plan_id",
                table: "trip_plan_exchanges",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_trip_plan_version_id",
                table: "trip_plan_exchanges",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_exchanges_user_id",
                table: "trip_plan_exchanges",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trip_plan_exchanges");

            migrationBuilder.DropTable(
                name: "trip_plan_exchange_sessions");

            migrationBuilder.CreateTable(
                name: "tour_guide_booking_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    suggested_trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    tour_guide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_version_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deleted_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    end_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    last_updated_by = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    start_date = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    user_responded_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    user_response_message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                        name: "FK_tour_guide_booking_requests_trip_plan_versions_suggested_tri~",
                        column: x => x.suggested_trip_plan_version_id,
                        principalTable: "trip_plan_versions",
                        principalColumn: "id");
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
                name: "IX_tour_guide_booking_requests_suggested_trip_plan_version_id",
                table: "tour_guide_booking_requests",
                column: "suggested_trip_plan_version_id");

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
        }
    }
}
