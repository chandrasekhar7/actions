using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations
{
    public class AchTransactionEntityConfiguration : IEntityTypeConfiguration<AchTransaction>
    {
        public void Configure(EntityTypeBuilder<AchTransaction> builder)
        {
            builder.ToTable("AchTransactions", "loan");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("PaymentID");
            builder.Property(e => e.StatusDate)
                .HasColumnType("date");
            builder.Property(e => e.ReturnCode);
            builder.Property(e => e.BatchNum);
            builder.Property(e => e.ReturnMessage);
            builder.Property(e => e.AccountNumber)
                .HasColumnName("Acct").IsRequired();
            builder.Property(e => e.RoutingNumber)
                .HasColumnName("ABA").IsRequired();
            builder.Property(e => e.Success).HasColumnName("Status");
            builder.Property(e => e.KeyTable).IsRequired();
            builder.Property(e => e.IsCdr);
            builder.Property(e => e.AccountType).IsRequired()
                .HasConversion(t => t == BankAccountType.Savings ? "S" : "C"
                , f => f == "S" ? BankAccountType.Savings : BankAccountType.Checking);

            builder.HasOne<Transaction>().WithOne().HasForeignKey<AchTransaction>(u => u.Id);
        }
    }
}