using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickService.LoanRepayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickService.LoanRepayment.Infrastructure.Data.EntityConfiguration
{
    public class CustomerRequestConfiguration : IEntityTypeConfiguration<CustomerRequest>
    {
        public void Configure(EntityTypeBuilder<CustomerRequest> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("ID").ValueGeneratedOnAdd();

            builder.Property(t => t.AccountName).HasColumnName("ACCOUNT_NAME").HasMaxLength(500);
            builder.Property(t => t.AccountNumber).HasColumnName("ACCOUNT_NUMBER").HasMaxLength(10);
            builder.Property(t => t.CreatedDate).HasColumnName("CREATED_DATE").IsRequired();
            builder.Property(t => t.CustomerAuthType).HasColumnName("CUSTOMER_AUTH_TYPE").HasMaxLength(20).IsRequired();
            builder.Property(t => t.RequestType).HasColumnName("REQUEST_TYPE").HasMaxLength(200).IsRequired();
            builder.Property(t => t.Status).HasColumnName("STATUS").HasMaxLength(50).IsRequired();
            builder.Property(t => t.TranId).HasColumnName("TRAN_ID").HasMaxLength(20);
            builder.Property(t => t.TreatedBy).HasColumnName("TREATED_BY").HasMaxLength(20);
            builder.Property(t => t.TreatedByUnit).HasColumnName("TREATED_BY_UNIT").HasMaxLength(100);
            builder.Property(t => t.TreatedDate).HasColumnName("TREATED_DATE");
            builder.Property(t => t.RejectionReason).HasColumnName("REJECTION_REASON").HasMaxLength(300);
            builder.Property(t => t.Remarks).HasColumnName("REMARKS").HasMaxLength(500);

            builder.HasIndex(x => x.Status).HasName("IX_REQUEST_STATUS").IsUnique(false);
            builder.HasIndex(x => x.TreatedByUnit).HasName("IX_TRBY_UNIT").IsUnique(false);



            builder.ToTable("CUSTOMER_REQUEST", "dbo");
        }
    }
}
