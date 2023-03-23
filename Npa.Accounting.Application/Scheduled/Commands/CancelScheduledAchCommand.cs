using MediatR;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npa.Accounting.Application.Scheduled.Commands
{
    public record CancelScheduledAchCommand(int ScheduledAchId) : IRequest<Unit>;
    public class CancelScheduledAchCommandHandler : IRequestHandler<CancelScheduledAchCommand, Unit>
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IUserService userService;
        private readonly IScheduleTransactionRepository transactionRepo;


        public CancelScheduledAchCommandHandler(ICustomerRepository customerRepository, IUserService userService, IScheduleTransactionRepository transactionRepo)
        {
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.transactionRepo = transactionRepo ?? throw new ArgumentNullException(nameof(transactionRepo));
        }


        public async Task<Unit> Handle(CancelScheduledAchCommand request, CancellationToken cancellationToken = default)
        {
            var user = userService.GetUser() ?? throw new ForbiddenException();
            int loanId = (await transactionRepo.GetById(request.ScheduledAchId)).LoanId;
            var customer = await customerRepository.GetWithLoan(loanId, cancellationToken) ?? throw new NotFoundException();

            customer.Loan.CancelScheduledAch(request.ScheduledAchId, user.Teller);
            await customerRepository.SaveChanges(cancellationToken);

            return Unit.Value;
        }
    }

}


