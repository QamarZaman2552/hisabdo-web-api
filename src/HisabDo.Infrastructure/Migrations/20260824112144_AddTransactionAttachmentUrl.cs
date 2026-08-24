using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HisabDo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionAttachmentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Transactions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Transactions");
        }
    }
}
