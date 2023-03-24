using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.Transactions;

public class AchTransactionRepository : IAchTransactionRepository
{
    private readonly ITransactionDbContext context;

    public AchTransactionRepository(
        ITransactionDbContext context
    )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Transaction> GetById(int id, CancellationToken cancellationToken = default) =>
        context.Transactions.Include(x => x.AchTransaction)
            .FirstOrDefaultAsync(x => x.AchTransaction.Id == id, cancellationToken);

    public Task<List<Transaction>> Get(AchFilter filter, CancellationToken cancellationToken = default) =>
        context.Transactions.Include(x => x.AchTransaction)
            .Where(x => !filter.LoanId.HasValue || x.LoanId == filter.LoanId)
            .Where(x => !filter.Pending.HasValue || (filter.Pending.Value && x.AchTransaction.ReturnCode == null) ||
                        (!filter.Pending.Value && x.AchTransaction.ReturnCode != null))
                        .ToListAsync(cancellationToken);
}