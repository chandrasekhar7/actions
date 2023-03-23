
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

public class AchFilter
{
    public bool? Pending { get; set; }
    public int? LoanId { get; set; }
}
public interface IAchTransactionRepository
{
    Task<Transaction> GetById(int id, CancellationToken cancellationToken = default);
    Task<List<Transaction>> Get(AchFilter filter, CancellationToken cancellationToken = default);
}