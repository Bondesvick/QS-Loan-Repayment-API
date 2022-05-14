using Microsoft.EntityFrameworkCore;
using QuickService.LoanRepayment.Core.Entities;
using QuickService.LoanRepayment.Infrastructure.Data.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace QuickService.LoanRepayment.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext()
        //{
        //}

        public AppDbContext(DbContextOptions<AppDbContext> opts)
         : base(opts)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseSqlServer(@"Data Source=52.166.249.18;Initial Catalog=QuickService;User Id=sa;Password=sql@Passw0rd123;").EnableSensitiveDataLogging();
            //    base.OnConfiguring(optionsBuilder);
            //}
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = ServiceResolver.Resolve<IConfiguration>().GetConnectionString("QuickServiceDbConn");
                optionsBuilder.UseSqlServer(connStr).EnableSensitiveDataLogging();
                base.OnConfiguring(optionsBuilder);
            }
        }

        public DbSet<CustomerRequest> CustomerRequests { get; set; }
        public DbSet<LoanRepaymentDetails> LoanRepaymentDetails { get; set; }
        public DbSet<LoanRepaymentDocument> LoanRepaymentDocuments { get; set; }
        public DbSet<Audit> Audits { get; set; }
        //public DbSet<VicTest> VicTests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerRequestConfiguration());
            modelBuilder.ApplyConfiguration(new LoanRepaymentDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new LoanRepaymentDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new AuditConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}