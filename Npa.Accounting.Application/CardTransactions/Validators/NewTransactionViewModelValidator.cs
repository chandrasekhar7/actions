using System;
using System.Globalization;
using FluentValidation;

namespace Npa.Accounting.Application.CardTransactions.Validators;

public class NewTransactionViewModelValidator : AbstractValidator<NewTransactionViewModel>
{
    public NewTransactionViewModelValidator()
    {
        RuleFor(e => e.Amount).GreaterThan(0);
        RuleFor(e => e.LoanId).GreaterThan(0);
        RuleFor(e => e.TransactionType).NotNull();
        RuleFor(e => e.StatementId).Must(e => e == null || DateOnly.TryParseExact(e,"yyyyMMdd",CultureInfo.InvariantCulture, 
            DateTimeStyles.None,out var _));
    }
}