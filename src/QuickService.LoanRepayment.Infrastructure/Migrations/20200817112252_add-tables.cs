using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickService.LoanRepayment.Infrastructure.Migrations
{
    public partial class addtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOAN_REPAYMENT_DOC",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocTitle = table.Column<string>(nullable: true),
                    DOC_EXTENSION = table.Column<string>(maxLength: 10, nullable: false),
                    DOC_NAME = table.Column<string>(maxLength: 500, nullable: false),
                    DOC_CONTENT = table.Column<string>(nullable: false),
                    CUSTOMER_REQ_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAN_REPAYMENT_DOC", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOAN_REPAYMENT_DOC_CUSTOMER_REQUEST_CUSTOMER_REQ_ID",
                        column: x => x.CUSTOMER_REQ_ID,
                        principalSchema: "dbo",
                        principalTable: "CUSTOMER_REQUEST",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                schema: "dbo",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ACCOUNT_NAME = table.Column<string>(maxLength: 100, nullable: false),
                    ACCOUNT_NUMBER = table.Column<string>(maxLength: 10, nullable: false),
                    SIGNATURE_CONTENT = table.Column<string>(maxLength: 200, nullable: true),
                    SignatureExt = table.Column<string>(nullable: true),
                    REPAYMENT_PLAN = table.Column<string>(maxLength: 200, nullable: true),
                    ACCOUNT_SEGMENT = table.Column<string>(maxLength: 10, nullable: false),
                    REPAYMENT_AMOUNT = table.Column<string>(maxLength: 200, nullable: true),
                    LOAN_ACCOUNT_NO = table.Column<string>(maxLength: 200, nullable: true),
                    LOAN_CURRENT_BALANCE = table.Column<string>(maxLength: 200, nullable: true),
                    REPAYMENT_ACCOUNT_NO = table.Column<string>(maxLength: 200, nullable: true),
                    AMOUNT = table.Column<string>(maxLength: 150, nullable: true),
                    CUSTOMER_REQUEST_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAN_REPAYMENT_TRANSACTION_DETAILS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOAN_REPAYMENT_TRANSACTION_DETAILS_CUSTOMER_REQUEST_CUSTOMER_REQUEST_ID",
                        column: x => x.CUSTOMER_REQUEST_ID,
                        principalSchema: "dbo",
                        principalTable: "CUSTOMER_REQUEST",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LOAN_REPAYMENT_DOC_CUST_REQ_ID",
                schema: "dbo",
                table: "LOAN_REPAYMENT_DOC",
                column: "CUSTOMER_REQ_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LOAN_REPAYMENT_TRANX_DET_CUST_RQ_ID",
                schema: "dbo",
                table: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                column: "CUSTOMER_REQUEST_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOAN_REPAYMENT_DOC",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LOAN_REPAYMENT_TRANSACTION_DETAILS",
                schema: "dbo");
        }
    }
}
