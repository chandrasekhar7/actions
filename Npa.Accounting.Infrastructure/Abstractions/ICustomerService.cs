using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Infrastructure.Abstractions;

public interface ICustomerService
{
    Task<CustomerInfo> GetCustomerInfoAsync(int customerId, CancellationToken t = default);
}