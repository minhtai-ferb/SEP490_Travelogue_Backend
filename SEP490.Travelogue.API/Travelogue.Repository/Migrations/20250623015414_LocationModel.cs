using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class LocationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "content",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "description",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "name",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "latitude",
                table: "craft_villages");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "craft_villages");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "close_time",
                table: "locations",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "open_time",
                table: "locations",
                type: "time(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "close_time",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "open_time",
                table: "locations");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "hotels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "hotels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "hotels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "hotels",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "craft_villages",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "craft_villages",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
