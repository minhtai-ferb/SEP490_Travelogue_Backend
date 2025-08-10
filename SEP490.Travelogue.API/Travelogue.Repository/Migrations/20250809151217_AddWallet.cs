using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_requests_craft_villages_craft_village_id",
                table: "withdrawal_requests");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "withdrawal_requests");

            migrationBuilder.DropColumn(
                name: "processed_at",
                table: "withdrawal_requests");

            migrationBuilder.DropColumn(
                name: "processed_by",
                table: "withdrawal_requests");

            migrationBuilder.RenameColumn(
                name: "craft_village_id",
                table: "withdrawal_requests",
                newName: "bank_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_craft_village_id",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_bank_account_id");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "withdrawal_requests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "booking_id",
                table: "announcements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                table: "announcements",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "report_id",
                table: "announcements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "review_id",
                table: "announcements",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "bank_accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    bank_account_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    bank_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bank_account_number = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bank_owner_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_default = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
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
                    table.PrimaryKey("PK_bank_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_bank_accounts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_booking_id",
                table: "announcements",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_report_id",
                table: "announcements",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_review_id",
                table: "announcements",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_bank_accounts_user_id",
                table: "bank_accounts",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_announcements_bookings_booking_id",
                table: "announcements",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_announcements_reports_report_id",
                table: "announcements",
                column: "report_id",
                principalTable: "reports",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_announcements_reviews_review_id",
                table: "announcements",
                column: "review_id",
                principalTable: "reviews",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_requests_bank_accounts_bank_account_id",
                table: "withdrawal_requests",
                column: "bank_account_id",
                principalTable: "bank_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_announcements_bookings_booking_id",
                table: "announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_announcements_reports_report_id",
                table: "announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_announcements_reviews_review_id",
                table: "announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_requests_bank_accounts_bank_account_id",
                table: "withdrawal_requests");

            migrationBuilder.DropTable(
                name: "bank_accounts");

            migrationBuilder.DropIndex(
                name: "IX_announcements_booking_id",
                table: "announcements");

            migrationBuilder.DropIndex(
                name: "IX_announcements_report_id",
                table: "announcements");

            migrationBuilder.DropIndex(
                name: "IX_announcements_review_id",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "booking_id",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "is_read",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "report_id",
                table: "announcements");

            migrationBuilder.DropColumn(
                name: "review_id",
                table: "announcements");

            migrationBuilder.RenameColumn(
                name: "bank_account_id",
                table: "withdrawal_requests",
                newName: "craft_village_id");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_bank_account_id",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_craft_village_id");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "withdrawal_requests",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "withdrawal_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "processed_at",
                table: "withdrawal_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "processed_by",
                table: "withdrawal_requests",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_requests_craft_villages_craft_village_id",
                table: "withdrawal_requests",
                column: "craft_village_id",
                principalTable: "craft_villages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
