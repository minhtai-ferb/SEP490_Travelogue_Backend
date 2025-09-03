using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class NullableTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("db4f808d-6588-4d6f-abe2-5b8f9c4376c2"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("fc9e85ee-0603-4037-9f35-d0e2cc0db804"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("39c8465b-083a-49d3-af37-bfd3eab3f3ef"));

            migrationBuilder.AlterColumn<int>(
                name: "channel",
                table: "transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("18bc7834-adf6-42c4-932e-1978ca74a1d5"), "System", new DateTimeOffset(new DateTime(2025, 9, 1, 9, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(4268), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 1, 9, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(4270), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("fee89a68-f93a-4dad-a04c-1833bba1c7c4"), "System", new DateTimeOffset(new DateTime(2025, 9, 1, 9, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(4252), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 9, 1, 9, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(4260), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("7ffdc624-9338-4510-9d16-fbb37856b177"), null, new DateTimeOffset(new DateTime(2025, 9, 1, 16, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(3679), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 9, 1, 16, 23, 14, 464, DateTimeKind.Unspecified).AddTicks(3769), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("18bc7834-adf6-42c4-932e-1978ca74a1d5"));

            migrationBuilder.DeleteData(
                table: "commission_rates",
                keyColumn: "id",
                keyValue: new Guid("fee89a68-f93a-4dad-a04c-1833bba1c7c4"));

            migrationBuilder.DeleteData(
                table: "system_settings",
                keyColumn: "id",
                keyValue: new Guid("7ffdc624-9338-4510-9d16-fbb37856b177"));

            migrationBuilder.AlterColumn<int>(
                name: "channel",
                table: "transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "commission_rates",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "effective_date", "expiry_date", "is_active", "is_deleted", "last_updated_by", "last_updated_time", "rate_value", "type" },
                values: new object[,]
                {
                    { new Guid("db4f808d-6588-4d6f-abe2-5b8f9c4376c2"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2273), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2275), new TimeSpan(0, 0, 0, 0, 0)), 3m, 2 },
                    { new Guid("fc9e85ee-0603-4037-9f35-d0e2cc0db804"), "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2249), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "System", new DateTimeOffset(new DateTime(2025, 8, 31, 7, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(2258), new TimeSpan(0, 0, 0, 0, 0)), 5m, 1 }
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "created_by", "created_time", "deleted_by", "deleted_time", "is_active", "is_deleted", "key", "last_updated_by", "last_updated_time", "unit", "value" },
                values: new object[] { new Guid("39c8465b-083a-49d3-af37-bfd3eab3f3ef"), null, new DateTimeOffset(new DateTime(2025, 8, 31, 14, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(1500), new TimeSpan(0, 7, 0, 0, 0)), null, null, true, false, 1, null, new DateTimeOffset(new DateTime(2025, 8, 31, 14, 11, 48, 632, DateTimeKind.Unspecified).AddTicks(1565), new TimeSpan(0, 7, 0, 0, 0)), "%", "10" });
        }
    }
}
