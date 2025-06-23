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
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "trip_plan_exchanges");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "trip_plan_exchanges");

            migrationBuilder.RenameColumn(
                name: "user_response_message",
                table: "trip_plan_exchanges",
                newName: "response_message");

            migrationBuilder.RenameColumn(
                name: "user_responded_at",
                table: "trip_plan_exchanges",
                newName: "responded_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "response_message",
                table: "trip_plan_exchanges",
                newName: "user_response_message");

            migrationBuilder.RenameColumn(
                name: "responded_at",
                table: "trip_plan_exchanges",
                newName: "user_responded_at");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "end_date",
                table: "trip_plan_exchanges",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_date",
                table: "trip_plan_exchanges",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
