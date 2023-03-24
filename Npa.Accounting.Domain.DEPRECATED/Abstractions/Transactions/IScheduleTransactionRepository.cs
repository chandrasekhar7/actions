using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

public interface IScheduleTransactionRepository
{
    Task<ScheduledAch?> GetById(int id, CancellationToken cancellationToken = default);
    Task<List<ScheduledAch>> GetByLoanId(int loanId, CancellationToken cancellationToken = default);
    Task CreateDrawLog(string drawType, int powerID, int loanID, decimal amount, string ipAddress,
        CancellationToken cancellationToken = default);
}