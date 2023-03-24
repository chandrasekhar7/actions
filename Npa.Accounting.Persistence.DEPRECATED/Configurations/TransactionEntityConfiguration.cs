using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Converters;


namespace Npa.Accounting.Persistence.DEPRECATED.Configurations
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Payments", "loan");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("PaymentID").ValueGeneratedOnAdd().IsRequired();
            builder.Property(e => e.Amount).HasColumnType("decimal(8, 2)").IsRequired();
            builder.Property(e => e.CreatedOn).HasColumnType("datetime").IsRequired();
            builder.Property(e => e.LoanId).HasColumnName("LoanID").IsRequired();

            
            // builder.OwnsOne(m => m.Attempt, a =>
            // {
            //     a.Property(p => p.AttemptNsf).HasColumnName("AttemptNSF");
            //     a.Property(p => p.AttemptFees).HasColumnName("AttemptFees");
            //     a.Property(p => p.AttemptOriginal).HasColumnName("AttemptOriginal");
            //     a.Property(p => p.OrigDueDate).HasColumnType("date").HasColumnName("OrigDueDate");
            // });

            builder.Property(e => e.Teller)
                .HasConversion<TellerConverter, TellerComparer>()
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength(true);

            builder.Property(e => e.TransactionType).HasColumnName("TypeID")
                .HasConversion<TransactionTypeConverter,TransactionTypeComparer>()
                .IsRequired();

            builder.HasOne(u => u.CardTransaction).WithOne().HasForeignKey<CardTransaction>(u => u.Id);
            builder.HasOne(u => u.AchTransaction).WithOne().HasForeignKey<AchTransaction>(u => u.Id);
        }
    }
}