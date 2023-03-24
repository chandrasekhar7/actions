using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Persistence.DEPRECATED.Abstractions;

public interface ICustomerInfoRepository
{
    Task<CustomerInfo> GetInfoById(int powerId, CancellationToken t = default);
}