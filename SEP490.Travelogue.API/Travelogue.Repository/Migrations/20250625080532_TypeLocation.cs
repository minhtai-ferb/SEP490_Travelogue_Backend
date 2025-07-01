using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TypeLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "cuisines");

            migrationBuilder.DropColumn(
                name: "content",
                table: "cuisines");

            migrationBuilder.DropColumn(
                name: "description",
                table: "cuisines");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "cuisines");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "cuisines");

            migrationBuilder.DropColumn(
                name: "name",
                table: "cuisines");

            migrationBuilder.AddColumn<Guid>(
                name: "location_id",
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
                    type = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_location_type_mapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_type_mapping_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_historical_locations_location_id",
                table: "historical_locations",
                column: "location_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_location_type_mapping_location_id",
                table: "location_type_mapping",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "FK_historical_locations_locations_location_id",
                table: "historical_locations",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historical_locations_locations_location_id",
                table: "historical_locations");

            migrationBuilder.DropTable(
                name: "location_type_mapping");

            migrationBuilder.DropIndex(
                name: "IX_historical_locations_location_id",
                table: "historical_locations");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "historical_locations");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "cuisines",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "cuisines",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "cuisines",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "cuisines",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "cuisines",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "cuisines",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
