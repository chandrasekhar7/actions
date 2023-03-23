using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;

namespace Npa.Accounting.Application.Scheduled.Queries;

public record ScheduleAchQuery(int ScheduleId) : IRequest<ScheduledAchViewModel>;

public class ScheduleAchQueryHandler : IRequestHandler<ScheduleAchQuery,ScheduledAchViewModel>
{
    private readonly IScheduleTransactionRepository repo;

    public ScheduleAchQueryHandler(
        IScheduleTransactionRepository repo
    )
    {
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
    public async Task<ScheduledAchViewModel> Handle(ScheduleAchQuery request, CancellationToken cancellationToken)
    {
        var ach = await repo.GetById(request.ScheduleId, cancellationToken) ?? throw new NotFoundException();
        return new ScheduledAchViewModel(ach);
    }
}