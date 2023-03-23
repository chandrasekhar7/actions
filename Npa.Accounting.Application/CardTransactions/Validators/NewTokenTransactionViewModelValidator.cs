using FluentValidation;

namespace Npa.Accounting.Application.CardTransactions.Validators;

public class NewTokenTransactionViewModelValidator : AbstractValidator<NewTokenTransactionViewModel>
{
    public NewTokenTransactionViewModelValidator()
    {
        RuleFor(e => e).SetValidator(new NewTransactionViewModelValidator());
        RuleFor(e => e.Token).GreaterThan(0);
    }
}