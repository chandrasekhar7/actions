using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Converters;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations;

internal class ScheduledAchEntityConfiguration : IEntityTypeConfiguration<ScheduledAch>
{
    public void Configure(EntityTypeBuilder<ScheduledAch> builder)
    {
        builder.ToTable("ScheduledAch", "loan");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
        builder.Property(e => e.Amount).HasColumnType("decimal(8,2)");
        builder.Property(e => e.PaymentId);
        builder.Property(e => e.LoanId).HasColumnName("LoanId");
        builder.Property(e => e.ScheduledDate)
            .HasColumnType("date").HasConversion<DateOnlyConverter, DateOnlyComparer>();
        builder.Property(e => e.TransactionType).HasColumnName("TypeID")
            .HasConversion<TransactionTypeConverter,TransactionTypeComparer>()
            .IsRequired();
        
        builder.OwnsOne(e => e.Created, a =>
        {

            a.Property(e => e.TimeStamp)
                .HasColumnType("datetime").HasColumnName("CreatedOn");
            a.Property(e => e.Teller).HasColumnName("CreatedBy")
                .HasConversion<TellerConverter, TellerComparer>();
        });
        builder.OwnsOne(e => e.Cancelled, a =>
        {

            a.Property(e => e.TimeStamp)
                .HasColumnType("datetime").HasColumnName("CancelledOn");
            a.Property(e => e.Teller).HasColumnName("CancelledBy")
                .HasConversion<TellerConverter, TellerComparer>();
        });
        builder.HasOne<AchTransaction>().WithOne().HasForeignKey<ScheduledAch>(u => u.PaymentId);
    }
}