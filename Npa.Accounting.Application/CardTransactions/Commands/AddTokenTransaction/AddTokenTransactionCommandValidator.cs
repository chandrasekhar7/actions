using FluentValidation;
using Npa.Accounting.Application.CardTransactions.Validators;

namespace Npa.Accounting.Application.CardTransactions.Commands.AddTokenTransaction;

public class AddTokenTransactionCommandValidator : AbstractValidator<AddTokenTransactionCommand>
{
    public AddTokenTransactionCommandValidator()
    {
        RuleFor(e => e.NewTransaction).NotNull().SetValidator(new NewTransactionViewModelValidator());
        RuleFor(e => e.Token).GreaterThan(0);
    }
}