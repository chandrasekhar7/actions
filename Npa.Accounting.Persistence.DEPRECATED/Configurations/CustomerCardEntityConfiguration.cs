using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Converters;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations;

internal class CustomerCardEntityConfiguration : IEntityTypeConfiguration<CustomerCard>, IChooseDb
{
    public void Configure(EntityTypeBuilder<CustomerCard> builder)
    {
        builder.ToTable("USAePayPaymentMethods", "dbo");
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
        builder.Property(e => e.Btid).IsRequired();
        builder.Property(e => e.LastFour).HasConversion<LastFourConverter, LastFourComparer>()
            .HasColumnName("CardNumber").IsRequired();
        builder.Property(e => e.Expiration).HasConversion<ExpirationConverter, ExpirationComparer>()
            .HasColumnType("datetime").IsRequired();
        builder.Property(e => e.CardToken).HasColumnName("PaymentMethodID").IsRequired();
        builder.Property(e => e.CardType).HasColumnType("char(1)");
        builder.Property(e => e.PowerId).HasColumnName("Power_id").IsRequired();
        builder.Property(e => e.Deleted).HasColumnName("Deleted").IsRequired();
        // builder.Property(e => e.DateAdded).HasColumnName("DateAdded").HasColumnType("datetime").IsRequired();
        // builder.Property(e => e.DateEdited).HasColumnName("DateEdited").HasColumnType("datetime");
        builder.Property(e => e.DeletedOn).HasColumnName("DateDeleted").HasColumnType("datetime");
        //builder.Property(e => e.SortOrder).IsRequired();
        builder.Property(e => e.CanDisburse);
    }
}