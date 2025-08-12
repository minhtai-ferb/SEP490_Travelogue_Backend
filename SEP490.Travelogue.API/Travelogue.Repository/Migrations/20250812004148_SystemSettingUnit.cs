using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SystemSettingUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("0f91b49b-565a-49e1-af9f-53e6f6b958ca"));

            migrationBuilder.DropColumn(
                name: "approved_at",
                table: "refund_requests");

            migrationBuilder.DropColumn(
                name: "rejection_at",
                table: "refund_requests");

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "system_settings",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "refund_amount",
                table: "refund_requests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("14fd4275-5988-4d86-97ed-844f230a4927"), null, new DateTimeOffset(new DateTime(2025, 8, 12, 7, 41, 36, 217, DateTimeKind.Unspecified).AddTicks(9422), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 12, 7, 41, 36, 217, DateTimeKind.Unspecified).AddTicks(9519), new TimeSpan(0, 7, 0, 0, 0)), null, "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("14fd4275-5988-4d86-97ed-844f230a4927"));

            migrationBuilder.DropColumn(
                name: "unit",
                table: "system_settings");

            migrationBuilder.AlterColumn<decimal>(
                name: "refund_amount",
                table: "refund_requests",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_at",
                table: "refund_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "rejection_at",
                table: "refund_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "value" },
                values: new object[] { new Guid("0f91b49b-565a-49e1-af9f-53e6f6b958ca"), null, new DateTimeOffset(new DateTime(2025, 8, 10, 6, 57, 51, 93, DateTimeKind.Unspecified).AddTicks(292), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 0, null, new DateTimeOffset(new DateTime(2025, 8, 10, 6, 57, 51, 93, DateTimeKind.Unspecified).AddTicks(1172), new TimeSpan(0, 7, 0, 0, 0)), "10" });
        }
    }
}
