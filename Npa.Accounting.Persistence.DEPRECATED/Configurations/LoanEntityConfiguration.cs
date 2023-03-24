using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Converters;

namespace Npa.Accounting.Persistence.DEPRECATED.Configurations;

/// <summary>
/// This exists because we are missing a root aggregate. Our root aggregate should be the customer but all
/// the referential stuff is built around loanId
/// </summary>
public class LoanTransactionEntityConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans", "loan");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("LoanID");
        builder.Property(e => e.CustomerId).HasColumnName("PowerID");
        builder.Ignore(e => e.LoanInfo);
    }
}