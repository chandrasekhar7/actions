using FluentValidation;

namespace Npa.Accounting.Application.CardTransactions.Validators;

public class NewCardTransactionViewModelValidator : AbstractValidator<NewCardTransactionViewModel>
{
    public NewCardTransactionViewModelValidator()
    {
        RuleFor(e => e).SetValidator(new NewTransactionViewModelValidator());
        RuleFor(e => e.Card).SetValidator(new CardValidator());
    }
}