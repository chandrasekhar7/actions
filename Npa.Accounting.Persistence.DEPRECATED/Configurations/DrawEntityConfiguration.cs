using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations
{
    internal class DrawEntityConfiguration : IEntityTypeConfiguration<Draw>
    {
        public void Configure(EntityTypeBuilder<Draw> builder)
        {
            builder.ToTable("DrawAttempts", "dbo");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("DrawID");
            builder.Property(e => e.DrawType);
            builder.Property(e => e.PowerID);
            builder.Property(e => e.LoanID);
            builder.Property(e => e.CreatedOn).HasColumnType("datetime");
            builder.Property(e => e.Amount);
            builder.Property(e => e.IpAddress);
        }
    }
}