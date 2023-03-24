using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers
{
    public interface ICardStoreRepository
    {
        Task<CardStore?> GetById(int customerId, CancellationToken t = default);
        
        Task<CardStore?> GetByCardToken(int token, CancellationToken t = default);

        Task SaveChanges(CancellationToken t = default);
    }
}