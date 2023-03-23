using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

namespace Npa.Accounting.Application.Scheduled.Queries
{
    public record GetScheduledAchByLoanIdQuery(int LoanId) : IRequest<List<ScheduledAchViewModel>>;

    public class GetScheduledAchByLoanIdQueryQueryHandler : IRequestHandler<GetScheduledAchByLoanIdQuery, List<ScheduledAchViewModel>>
    {
        private readonly IScheduleTransactionRepository repo;

        public GetScheduledAchByLoanIdQueryQueryHandler(
            IScheduleTransactionRepository repo
        )
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        public async Task<List<ScheduledAchViewModel>> Handle(GetScheduledAchByLoanIdQuery request, CancellationToken cancellationToken)
        {
            var achList = await repo.GetByLoanId(request.LoanId, cancellationToken) ?? throw new NotFoundException();
            var returnList = new List<ScheduledAchViewModel>();
            foreach (var ach in achList)
            {
                var viewModel = new ScheduledAchViewModel(ach);
                returnList.Add(viewModel);
            }
            return returnList;
        }
    }
}