using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickService.LoanRepayment.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.EnsureSchema(
            //    name: "dbo");

            //migrationBuilder.CreateTable(
            //    name: "CUSTOMER_REQUEST",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        ID = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TRAN_ID = table.Column<string>(maxLength: 20, nullable: true),
            //        ACCOUNT_NUMBER = table.Column<string>(maxLength: 10, nullable: true),
            //        ACCOUNT_NAME = table.Column<string>(maxLength: 500, nullable: true),
            //        CUSTOMER_AUTH_TYPE = table.Column<string>(maxLength: 20, nullable: false),
            //        STATUS = table.Column<string>(maxLength: 50, nullable: false),
            //        CREATED_DATE = table.Column<DateTime>(nullable: false),
            //        TREATED_BY = table.Column<string>(maxLength: 20, nullable: true),
            //        TREATED_DATE = table.Column<DateTime>(nullable: true),
            //        REQUEST_TYPE = table.Column<string>(maxLength: 200, nullable: false),
            //        TREATED_BY_UNIT = table.Column<string>(maxLength: 100, nullable: true),
            //        REJECTION_REASON = table.Column<string>(maxLength: 300, nullable: true),
            //        REMARKS = table.Column<string>(maxLength: 500, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CUSTOMER_REQUEST", x => x.ID);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_REQUEST_STATUS",
            //    schema: "dbo",
            //    table: "CUSTOMER_REQUEST",
            //    column: "STATUS");

            //migrationBuilder.CreateIndex(
            //    name: "IX_TRBY_UNIT",
            //    schema: "dbo",
            //    table: "CUSTOMER_REQUEST",
            //    column: "TREATED_BY_UNIT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CUSTOMER_REQUEST",
                schema: "dbo");
        }
    }
}
