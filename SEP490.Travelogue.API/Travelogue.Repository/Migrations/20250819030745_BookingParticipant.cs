using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class BookingParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("399388f3-56cb-466c-ab7c-1f9c8575bbf2"));

            migrationBuilder.AddColumn<string>(
                name: "contact_address",
                table: "bookings",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "contact_email",
                table: "bookings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "contact_name",
                table: "bookings",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "contact_phone",
                table: "bookings",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_per_participant",
                table: "booking_participant",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_birth",
                table: "booking_participant",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "booking_participant",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "gender",
                table: "booking_participant",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("31806833-46a8-4901-b0c3-d39a21440830"), null, new DateTimeOffset(new DateTime(2025, 8, 19, 10, 7, 40, 617, DateTimeKind.Unspecified).AddTicks(3446), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 19, 10, 7, 40, 617, DateTimeKind.Unspecified).AddTicks(3507), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("31806833-46a8-4901-b0c3-d39a21440830"));

            migrationBuilder.DropColumn(
                name: "contact_address",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "contact_email",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "contact_name",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "contact_phone",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                table: "booking_participant");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "booking_participant");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "booking_participant");

            migrationBuilder.AlterColumn<decimal>(
                name: "price_per_participant",
                table: "booking_participant",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("399388f3-56cb-466c-ab7c-1f9c8575bbf2"), null, new DateTimeOffset(new DateTime(2025, 8, 18, 2, 43, 48, 592, DateTimeKind.Unspecified).AddTicks(3123), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 18, 2, 43, 48, 592, DateTimeKind.Unspecified).AddTicks(3179), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
