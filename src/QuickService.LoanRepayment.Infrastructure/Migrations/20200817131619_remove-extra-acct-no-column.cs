using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickService.LoanRepayment.Infrastructure.Migrations
{
    public partial class removeextraacctnocolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SIGNATURE_CONTENT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_PLAN",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_AMOUNT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_ACCOUNT_NO",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_CURRENT_BALANCE",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_ACCOUNT_NO",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AMOUNT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_SEGMENT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_NUMBER",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SIGNATURE_CONTENT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_PLAN",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_AMOUNT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "REPAYMENT_ACCOUNT_NO",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_CURRENT_BALANCE",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LOAN_ACCOUNT_NO",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AMOUNT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_SEGMENT",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ACCOUNT_NUMBER",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
