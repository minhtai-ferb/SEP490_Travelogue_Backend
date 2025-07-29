using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "transaction_date",
                table: "transactions");

            migrationBuilder.AddColumn<string>(
                name: "account_number",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "counter_account_bank_id",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "counter_account_name",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "counter_account_number",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "paid_amount",
                table: "transactions",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_link_id",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "payment_reference",
                table: "transactions",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "payment_status",
                table: "transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "transaction_date_time",
                table: "transactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "workshop_schedule_id",
                table: "bookings",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_workshop_schedule_id",
                table: "bookings",
                column: "workshop_schedule_id");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_workshop_schedules_workshop_schedule_id",
                table: "bookings",
                column: "workshop_schedule_id",
                principalTable: "workshop_schedules",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_workshop_schedules_workshop_schedule_id",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_workshop_schedule_id",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "account_number",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "counter_account_bank_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "counter_account_name",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "counter_account_number",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "paid_amount",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "payment_link_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "payment_reference",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "payment_status",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "transaction_date_time",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "workshop_schedule_id",
                table: "bookings");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                table: "transactions",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "transaction_date",
                table: "transactions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
