using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;

public interface ILoanService
{
    Task<LoanInfo> GetLoan(int loanId, CancellationToken t = default);
    Task<LoanInfo> GetStatement(StatementId stmtId, CancellationToken t = default);
    Task ApplyTransaction(Transaction transaction, CancellationToken t = default, int? rescindPaymentId = null);
}