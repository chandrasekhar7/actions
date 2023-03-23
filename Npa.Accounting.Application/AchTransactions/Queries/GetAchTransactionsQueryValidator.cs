using FluentValidation;
using Npa.Accounting.Application.Transactions;

namespace Npa.Accounting.Application.AchTransactions.Queries;

public class GetAchTransactionsQueryValidator : AbstractValidator<GetAchTransactionsQuery>
{
    public GetAchTransactionsQueryValidator()
    {
        RuleFor(e => e.Filter).SetValidator(new TransactionFilterValidator());
    }
}