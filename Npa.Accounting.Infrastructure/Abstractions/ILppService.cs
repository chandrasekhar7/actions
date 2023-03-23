using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Infrastructure.Abstractions;

internal interface ILppService
{
    Task<ReturnMessage> DebitAsync(Transaction ct, CancellationToken t = default);
    
    Task<ReturnMessage> DisburseAsync(Transaction ct, CancellationToken t = default);
}