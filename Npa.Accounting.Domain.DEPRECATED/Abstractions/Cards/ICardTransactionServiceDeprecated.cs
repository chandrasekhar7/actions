using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Cards;

public interface ICardTransactionServiceDeprecated
{
    Task<ReturnMessage> Process(Transaction transaction, CancellationToken t = default);
}