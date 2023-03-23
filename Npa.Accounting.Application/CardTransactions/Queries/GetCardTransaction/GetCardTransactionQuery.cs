using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npa.Accounting.Application.Transactions;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

namespace Npa.Accounting.Application.CardTransactions.Queries.GetCardTransaction;

public record GetCardTransactionQuery(TransactionFilter Filter) : IRequest<IReadOnlyList<CardTransactionViewModel>>;

public class GetCardTransactionQueryHandler : IRequestHandler<GetCardTransactionQuery, IReadOnlyList<CardTransactionViewModel>>
{
    private readonly ITransactionReadDbFacade facade;

    public GetCardTransactionQueryHandler(ITransactionReadDbFacade context)
    {
        this.facade = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<CardTransactionViewModel>> Handle(GetCardTransactionQuery request,
        CancellationToken cancellationToken)
    {
        
        //throw new ApplicationException("You must specify either a loan or customer filter");
        return await facade.QueryAsync<CardTransactionViewModel>(
            @"SELECT P.PaymentId AS Id, ABS(P.Amount) AS Amount, P.LoanID, P.TypeID AS TransactionType, P.CreatedOn,
       Teller, C.PaymentMethodID AS CardToken, StatusDate, ReturnCode, ReturnMessage, MerchantID, RefNum, M.CardNumber AS LastFour
                    FROM loan.Payments P
                     INNER JOIN loan.CCTransactions C ON C.PaymentID = P.PaymentID
                     INNER JOIN ChoosePdf.dbo.USAePayPaymentMethods M ON M.PaymentMethodID = C.PaymentMethodID
                    INNER JOIN loan.Loans L ON L.LoanId = P.LoanId
                    WHERE P.PaymentID = ISNULL(@TransactionId,P.PaymentID) 
                      AND L.PowerId = ISNULL(@PowerId, L.PowerId) 
                      AND P.LoanId = ISNULL(@LoanId, P.LoanId)
                    ORDER BY CreatedOn DESC", request.Filter) ?? throw new NotFoundException();
    }
}