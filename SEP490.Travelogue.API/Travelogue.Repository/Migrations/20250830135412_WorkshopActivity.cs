using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class WorkshopActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("612606ff-ea3d-472c-ac90-14f1129ae07c"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("881b17a3-7cae-4b2f-a2a6-32955763d291"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("c55eb4a9-771c-41e8-aeea-b89de4b4dd9d"));

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "workshop_recurring_rules");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "workshop_recurring_rules");

            migrationBuilder.DropColumn(
                name: "end_date",
                table: "workshop_recurring_rule_requests");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "workshop_recurring_rule_requests");

            migrationBuilder.DropColumn(
                name: "end_hour",
                table: "workshop_activity_requests");

            migrationBuilder.DropColumn(
                name: "start_hour",
                table: "workshop_activity_requests");

            migrationBuilder.DropColumn(
                name: "end_hour",
                table: "workshop_activities");

            migrationBuilder.DropColumn(
                name: "start_hour",
                table: "workshop_activities");

            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                table: "workshop_activity_requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "duration_minutes",
                table: "workshop_activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("19bb7afe-6436-4f2f-8c6a-19896f55b13e"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 13, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(851), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 13, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(852), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("f7188b12-540b-465f-af09-cd539c428844"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 13, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(836), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 13, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(843), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("eb3cbbfd-1398-4fa2-afe8-db6bca29acc8"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 20, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(367), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 20, 54, 9, 472, DateTimeKind.Unspecified).AddTicks(431), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("19bb7afe-6436-4f2f-8c6a-19896f55b13e"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("f7188b12-540b-465f-af09-cd539c428844"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("eb3cbbfd-1398-4fa2-afe8-db6bca29acc8"));

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                table: "workshop_activity_requests");

            migrationBuilder.DropColumn(
                name: "duration_minutes",
                table: "workshop_activities");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "workshop_recurring_rules",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "workshop_recurring_rules",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "workshop_recurring_rule_requests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "workshop_recurring_rule_requests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_hour",
                table: "workshop_activity_requests",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_hour",
                table: "workshop_activity_requests",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "end_hour",
                table: "workshop_activities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "start_hour",
                table: "workshop_activities",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("612606ff-ea3d-472c-ac90-14f1129ae07c"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5155), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5157), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("881b17a3-7cae-4b2f-a2a6-32955763d291"), "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5140), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 30, 11, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(5148), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("c55eb4a9-771c-41e8-aeea-b89de4b4dd9d"), null, new DateTimeOffset(new DateTime(2025, 8, 30, 18, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(4609), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 30, 18, 21, 19, 270, DateTimeKind.Unspecified).AddTicks(4684), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
