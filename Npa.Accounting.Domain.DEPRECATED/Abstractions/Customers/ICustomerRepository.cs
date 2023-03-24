using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;

public interface ICustomerRepository
{
    Task<Customer?> GetWithLoan(int loanId, CancellationToken cancellationToken);
    Task<Customer?> GetWithLoan(int customerId, int loanId, CancellationToken cancellationToken = default);
    Task<Customer?> GetWithLoanByToken(int token, int loanId, CancellationToken t = default);
    Task SaveChanges(CancellationToken t = default, TransactionType? originalTransType = null, int? rescindPaymentId = null);

}