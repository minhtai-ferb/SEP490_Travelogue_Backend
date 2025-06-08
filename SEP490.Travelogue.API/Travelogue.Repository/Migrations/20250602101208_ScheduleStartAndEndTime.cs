using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ScheduleStartAndEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "scheduled_date",
                table: "orders",
                newName: "scheduled_start_date");

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_end_date",
                table: "orders",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scheduled_end_date",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "scheduled_start_date",
                table: "orders",
                newName: "scheduled_date");
        }
    }
}
