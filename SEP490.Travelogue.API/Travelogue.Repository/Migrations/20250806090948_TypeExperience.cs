using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class TypeExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "transactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "type_experience",
                table: "news",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id",
                table: "transactions",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_users_user_id",
                table: "transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_users_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "type_experience",
                table: "news");
        }
    }
}
