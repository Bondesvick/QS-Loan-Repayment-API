using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickService.LoanRepayment.Infrastructure.Migrations
{
    public partial class adddebitcardprop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HAVE_DEBIT_CARD",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HAVE_DEBIT_CARD",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS");
        }
    }
}
