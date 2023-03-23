using FluentValidation;
using Npa.Accounting.Application.Transactions;

namespace Npa.Accounting.Application.CardTransactions.Queries.GetCardTransaction;

public class GetCardTransactionQueryValidator : AbstractValidator<GetCardTransactionQuery>
{
    public GetCardTransactionQueryValidator()
    {
        RuleFor(e => e.Filter).SetValidator(new TransactionFilterValidator());
    }
}