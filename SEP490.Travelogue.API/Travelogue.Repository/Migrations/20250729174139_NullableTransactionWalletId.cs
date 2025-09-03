using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travelogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class NullableTransactionWalletId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_wallets_wallet_id",
                table: "transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "wallet_id",
                table: "transactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_wallets_wallet_id",
                table: "transactions",
                column: "wallet_id",
                principalTable: "wallets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_wallets_wallet_id",
                table: "transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "wallet_id",
                table: "transactions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_wallets_wallet_id",
                table: "transactions",
                column: "wallet_id",
                principalTable: "wallets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
