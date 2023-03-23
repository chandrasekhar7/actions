using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npa.Accounting.Application.Transactions;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

namespace Npa.Accounting.Application.AchTransactions.Queries;

public record GetAchTransactionsQuery(TransactionFilter Filter) : IRequest<IReadOnlyList<AchTransactionViewModel>>;

public class
    GetAchTransactionsQueryHandler : IRequestHandler<GetAchTransactionsQuery, IReadOnlyList<AchTransactionViewModel>>
{
    private readonly ITransactionReadDbFacade facade;
    private readonly IAchTransactionRepository repo;

    public GetAchTransactionsQueryHandler(ITransactionReadDbFacade context, IAchTransactionRepository repo)
    {
        this.facade = context ?? throw new ArgumentNullException(nameof(context));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    /// <summary>
    /// TODO: Implement the AchTransaction repository. We will have to add powerId to the payments table to switch this over
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    public async Task<IReadOnlyList<AchTransactionViewModel>> Handle(GetAchTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        return await facade.QueryAsync<AchTransactionViewModel>(
            @"SELECT P.PaymentId AS Id, ABS(P.Amount) AS Amount, P.LoanID, P.TypeID AS TransactionType, P.CreatedOn,
       Teller, StatusDate, ReturnCode, ReturnMessage, CONVERT(BIT, IIF(C.ReturnCode IS NULL, 1, 0)) AS Pending
FROM loan.Payments P
         INNER JOIN loan.AchTransactions C ON C.PaymentID = P.PaymentID
         INNER JOIN loan.Loans L ON L.LoanId = P.LoanId
WHERE P.PaymentID = ISNULL(@TransactionId,P.PaymentID)
  AND L.PowerId = ISNULL(@PowerId, L.PowerId)
  AND P.LoanId = ISNULL(@LoanId, P.LoanId)
  AND (@Pending IS NULL OR (@Pending = 1 AND C.ReturnCode IS NULL) OR (@Pending = 0 AND C.ReturnCode IS NOT NULL))
ORDER BY CreatedOn DESC", request.Filter) ?? throw new NotFoundException();
    }
}