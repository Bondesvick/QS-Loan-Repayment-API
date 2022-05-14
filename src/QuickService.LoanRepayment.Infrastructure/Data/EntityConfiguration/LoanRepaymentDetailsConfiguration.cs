using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickService.LoanRepayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Infrastructure.Data.EntityConfiguration
{
    public class LoanRepaymentDetailsConfiguration : IEntityTypeConfiguration<LoanRepaymentDetails>
    {
        public void Configure(EntityTypeBuilder<LoanRepaymentDetails> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("ID").ValueGeneratedOnAdd();

            builder.Property(t => t.AccountName).HasColumnName("ACCOUNT_NAME").IsRequired().HasMaxLength(100);
            builder.Property(t => t.AccountNumber).HasColumnName("ACCOUNT_NUMBER").IsRequired();
            builder.Property(t => t.AccountSegment).HasColumnName("ACCOUNT_SEGMENT").IsRequired();
            builder.Property(t => t.Amount).HasColumnName("AMOUNT");
            builder.Property(t => t.LoanAccountNo).HasColumnName("LOAN_ACCOUNT_NO").HasMaxLength(10);
            builder.Property(t => t.LoanCurrentBalance).HasColumnName("LOAN_CURRENT_BALANCE");
            builder.Property(t => t.RepaymentAmount).HasColumnName("REPAYMENT_AMOUNT");
            builder.Property(t => t.RepaymentAcctNo).HasColumnName("REPAYMENT_ACCOUNT_NO").HasMaxLength(10);
            builder.Property(t => t.HaveDebitCard).HasColumnName("HAVE_DEBIT_CARD");

            builder.Property(t => t.RepaymentPlan).HasColumnName("REPAYMENT_PLAN");
            builder.Property(t => t.SignatureContent).HasColumnName("SIGNATURE_CONTENT");

            builder.Property(t => t.CustomerRequestId).HasColumnName("CUSTOMER_REQUEST_ID").IsRequired();

            builder.HasOne(t => t.CustomerRequest)
                   .WithOne(t => t.LoanRepaymentDetails)
                   .HasForeignKey<LoanRepaymentDetails>(e => e.CustomerRequestId)
                   .IsRequired(true)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(t => t.CustomerRequest)
            //    .WithMany(t => t.LoanRepaymentDetails)
            //    .HasForeignKey(e => e.CustomerRequestId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CustomerRequestId).HasName("IX_LOAN_REPAYMENT_TRANX_DET_CUST_RQ_ID").IsUnique(false);

            builder.ToTable("LOAN_REPAYMENT_TRANSACTION_DETAILS", "dbo");
        }
    }
}