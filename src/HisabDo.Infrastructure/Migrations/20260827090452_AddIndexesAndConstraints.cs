using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HisabDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_CustomerId",
                table: "Transactions",
                columns: new[] { "UserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId_Email",
                table: "Customers",
                columns: new[] { "UserId", "Email" },
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_CustomerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Customers_UserId_Email",
                table: "Customers");
        }
    }
}
