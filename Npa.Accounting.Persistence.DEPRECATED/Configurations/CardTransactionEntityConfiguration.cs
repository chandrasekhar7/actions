using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Converters;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations
{
    public class CardTransactionEntityConfiguration : IEntityTypeConfiguration<CardTransaction>
    {
        public void Configure(EntityTypeBuilder<CardTransaction> builder)
        {
            builder.ToTable("CCTransactions", "loan");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("PaymentID");
            builder.Property(p => p.StatusDate)
                .HasColumnType("datetime").HasColumnName("StatusDate");
            builder.Property(p => p.ProcessDate)
                .HasColumnType("datetime").HasColumnName("ProcessDate");
            builder.Property(p => p.MerchantId).HasConversion<MerchantConverter, MerchantComparer>().HasColumnName("MerchantId");

            //builder.Ignore(e => e.Card);
            builder.OwnsOne(e => e.Card, a =>
            {
                a.Property(e => e.CardToken).HasColumnName("PaymentMethodID");
                a.Ignore(e => e.Deleted);
                a.Ignore(e => e.Expiration);
                a.Ignore(e => e.CanDisburse);
                a.Ignore(e => e.CardType);
                a.Ignore(e => e.DeletedOn);
                a.Ignore(e => e.LastFour);
                a.Ignore(e => e.Id);
                a.Ignore(e => e.Btid);
                a.Ignore(e => e.PowerId);
            });

            builder.OwnsOne(e => e.ReturnMessage, a =>
            {
                a.Ignore(e => e.Code);
                a.Property(e => e.Message).HasColumnName("ReturnMessage");
                a.Property(e => e.RefNum).HasColumnName("RefNum");
                a.Property(e => e.Status).HasConversion<CardStatusConverter, CardStatusComparer>().HasColumnName("ReturnCode");
            });
            
            
            
        }
    }
}