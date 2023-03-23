using FluentValidation;

namespace Npa.Accounting.Application.Transactions;

public class TransactionFilterValidator : AbstractValidator<TransactionFilter>
{
    public TransactionFilterValidator()
    {
        RuleFor(e => e).Must(e => e.LoanId.HasValue || e.PowerId.HasValue || e.TransactionId.HasValue)
            .WithMessage("You must specify either a loan or customer filter");
    }
}