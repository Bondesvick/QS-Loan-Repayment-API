using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickService.LoanRepayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Infrastructure.Data.EntityConfiguration
{
   public class LoanRepaymentDocumentConfiguration : IEntityTypeConfiguration<LoanRepaymentDocument>
    {
        public void Configure(EntityTypeBuilder<LoanRepaymentDocument> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("ID").ValueGeneratedOnAdd();

            builder.Property(t => t.DocExtension).HasColumnName("DOC_EXTENSION").HasMaxLength(10).IsRequired();
            builder.Property(t => t.DocName).HasColumnName("DOC_NAME").HasMaxLength(500).IsRequired();
            builder.Property(t => t.DocContent).HasColumnName("DOC_CONTENT").IsRequired();
            builder.Property(t => t.CustomerRequestId).HasColumnName("CUSTOMER_REQ_ID").IsRequired();

            builder.HasOne(t => t.CustomerRequest)
                .WithMany(t => t.LoanRepaymentDocuments)
                .HasForeignKey(t => t.CustomerRequestId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CustomerRequestId).HasName("IX_LOAN_REPAYMENT_DOC_CUST_REQ_ID").IsUnique(false);

            builder.ToTable("LOAN_REPAYMENT_DOC", "dbo");

        }
    }
}
