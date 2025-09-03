using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ReviewReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("74326f7d-23af-48ec-b280-f57b853fab6e"));

            migrationBuilder.AddColumn<int>(
                name: "final_report_status",
                table: "reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "handled_at",
                table: "reviews",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "handled_by",
                table: "reviews",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "moderator_note",
                table: "reviews",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("1616cf78-7cdd-483a-ae3f-4c42aefa3780"), null, new DateTimeOffset(new DateTime(2025, 8, 20, 11, 56, 50, 909, DateTimeKind.Unspecified).AddTicks(3341), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 20, 11, 56, 50, 909, DateTimeKind.Unspecified).AddTicks(3391), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("1616cf78-7cdd-483a-ae3f-4c42aefa3780"));

            migrationBuilder.DropColumn(
                name: "final_report_status",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "handled_at",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "handled_by",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "moderator_note",
                table: "reviews");

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("74326f7d-23af-48ec-b280-f57b853fab6e"), null, new DateTimeOffset(new DateTime(2025, 8, 19, 17, 11, 39, 366, DateTimeKind.Unspecified).AddTicks(8534), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 19, 17, 11, 39, 366, DateTimeKind.Unspecified).AddTicks(8616), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
