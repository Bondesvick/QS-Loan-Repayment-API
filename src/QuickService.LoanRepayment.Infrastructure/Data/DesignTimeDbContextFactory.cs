//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace QuickService.LoanRepayment.Infrastructure.Data
//{
//    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//    {
//        //public AppDbContext CreateDbContext(string[] args)
//        //{
//        //    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
//        //    optionsBuilder.UseSqlServer(@"Data Source=52.166.249.18;Initial Catalog=QuickService;User Id=sa;Password=sql@Passw0rd123;",
//        //         x => x.MigrationsHistoryTable("__FailedTransactionMigrationsHistory", "dbo"));

//        //    return new AppDbContext(optionsBuilder.Options);
//        //}
//        public AppDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
//            optionsBuilder.UseSqlServer(@"Server=10.234.200.193;Database=QuickService;UID=appsvrusr;PWD=gu1n355;",
//                 x => x.MigrationsHistoryTable("__FailedTransactionMigrationsHistory", "dbo"));

//            return new AppDbContext(optionsBuilder.Options);
//        }
//    }
//}