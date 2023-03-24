using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.Transactions;

public class ScheduleTransactionRepository : IScheduleTransactionRepository
{
    private readonly ITransactionDbContext context;

    public ScheduleTransactionRepository(
        ITransactionDbContext context
    )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<ScheduledAch?> GetById(int id, CancellationToken cancellationToken = default) =>
        context.ScheduledAchs.FirstOrDefaultAsync(s => s.Id == id);

    public Task<List<ScheduledAch>> GetByLoanId(int loanId, CancellationToken cancellationToken = default) =>
        context.ScheduledAchs.Where(s => s.LoanId == loanId && s.PaymentId == null && s.Cancelled == null).ToListAsync();

    public Task CreateDrawLog(string drawType, int powerID, int loanID, decimal amount, string ipAddress,
        CancellationToken cancellationToken = default)
    {
        context.Draws.Add(new Draw(drawType, powerID, loanID, amount, ipAddress));
        context.SaveChanges();
        return Task.CompletedTask;
    }
}