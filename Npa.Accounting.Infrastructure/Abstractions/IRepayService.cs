using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Infrastructure.Npacc;
using Npa.Accounting.Infrastructure.Repay.Responses;

namespace Npa.Accounting.Infrastructure.Abstractions;

internal interface IRepayService
{
    Task<NpaCardResponse> DebitAsync(NpaTokenPayment ct, CancellationToken t = default);
    Task<PaymentResponse> AddCardAsync(int customerId, Card card, CancellationToken t = default);
    Task<PaymentResponse> TokenPaymentAsync(CustomerCard card, decimal amount, CancellationToken t = default);
}