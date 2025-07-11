using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Location1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historical_locations_type_historical_locations_type_historic~",
                table: "historical_locations");

            migrationBuilder.DropTable(
                name: "location_categories");

            migrationBuilder.DropTable(
                name: "location_type_mapping");

            migrationBuilder.DropTable(
                name: "type_historical_locations");

            migrationBuilder.DropTable(
                name: "type_location");

            migrationBuilder.DropIndex(
                name: "IX_historical_locations_type_historical_location_id",
                table: "historical_locations");

            migrationBuilder.DropColumn(
                name: "type_historical_location_id",
                table: "historical_locations");

            migrationBuilder.AddColumn<float>(
                name: "distance_from_prev",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "estimated_end_time",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "estimated_start_time",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "travel_time_from_prev",
                table: "tour_plan_locations",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "location_type",
                table: "locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "type_historical_location",
                table: "historical_locations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "distance_from_prev",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "estimated_end_time",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "estimated_start_time",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "travel_time_from_prev",
                table: "tour_plan_locations");

            migrationBuilder.DropColumn(
                name: "location_type",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "type_historical_location",
                table: "historical_locations");

            migrationBuilder.AddColumn<Guid>(
                name: "type_historical_location_id",
                table: "historical_locations",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "location_type_mapping",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_type_mapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_type_mapping_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "type_historical_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_historical_locations", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "type_location",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_location", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "location_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    category_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
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
                    last_updated_time = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_categories_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_location_categories_type_location_category_id",
                        column: x => x.category_id,
                        principalTable: "type_location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_historical_locations_type_historical_location_id",
                table: "historical_locations",
                column: "type_historical_location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_categories_category_id",
                table: "location_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_categories_location_id",
                table: "location_categories",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_type_mapping_location_id",
                table: "location_type_mapping",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_historical_locations_type_historical_locations_type_historic~",
                table: "historical_locations",
                column: "type_historical_location_id",
                principalTable: "type_historical_locations",
                principalColumn: "id");
        }
    }
}
