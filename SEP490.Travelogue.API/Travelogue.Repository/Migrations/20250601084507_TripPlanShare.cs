using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TripPlanShare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "trip_plan_id",
                table: "orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "trip_plan_version_id",
                table: "orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "version_id",
                table: "orders",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "trip_plan_share",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    trip_plan_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    permission = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_trip_plan_share", x => x.id);
                    table.ForeignKey(
                        name: "FK_trip_plan_share_trip_plans_trip_plan_id",
                        column: x => x.trip_plan_id,
                        principalTable: "trip_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trip_plan_share_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_id",
                table: "orders",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_trip_plan_version_id",
                table: "orders",
                column: "trip_plan_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_share_trip_plan_id",
                table: "trip_plan_share",
                column: "trip_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trip_plan_share_user_id",
                table: "trip_plan_share",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_trip_plan_versions_trip_plan_version_id",
                table: "orders",
                column: "trip_plan_version_id",
                principalTable: "trip_plan_versions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_trip_plans_trip_plan_id",
                table: "orders",
                column: "trip_plan_id",
                principalTable: "trip_plans",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_trip_plan_versions_trip_plan_version_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_trip_plans_trip_plan_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "trip_plan_share");

            migrationBuilder.DropIndex(
                name: "IX_orders_trip_plan_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_trip_plan_version_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "trip_plan_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "trip_plan_version_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "version_id",
                table: "orders");
        }
    }
}
