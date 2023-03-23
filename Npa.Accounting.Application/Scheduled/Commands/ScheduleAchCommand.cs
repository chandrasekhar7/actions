using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Application.Scheduled.Commands;

public record ScheduleAchCommand(int LoanId, decimal Amount, TransactionType TransactionType, string IpAddress) : IRequest<ScheduledAchViewModel>;

class ScheduleAchCommandHandler : IRequestHandler<ScheduleAchCommand, ScheduledAchViewModel>
{
    private readonly ICustomerRepository customerRepository;
    private readonly IUserService userService;
    private readonly ICommunicationsService commService;
    private readonly ILoanLockRepository lockRepository;
    private readonly IFraudDetectionRepository fraudDetectionRepository;
    private readonly IScheduleTransactionRepository scheduleTransactionRepo;

    public ScheduleAchCommandHandler(ICustomerRepository customerRepository, IUserService userService, ILoanLockRepository lockRepository,ICommunicationsService commService,
        IScheduleTransactionRepository scheduleTransactionRepo, IFraudDetectionRepository fraudDetectionRepository)
    {
        this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        this.commService = commService ?? throw new ArgumentNullException(nameof(commService));
        this.lockRepository = lockRepository ?? throw new ArgumentNullException(nameof(lockRepository));
        this.scheduleTransactionRepo =
            scheduleTransactionRepo ?? throw new ArgumentNullException(nameof(scheduleTransactionRepo));
        this.fraudDetectionRepository = fraudDetectionRepository ?? throw new ArgumentNullException(nameof(fraudDetectionRepository));
    }
    
    public async Task<ScheduledAchViewModel> Handle(ScheduleAchCommand request, CancellationToken cancellationToken)
    {
        var user = userService.GetUser();
        var teller = user.Teller;
        var customer = await customerRepository.GetWithLoan(request.LoanId, cancellationToken) ?? throw new NotFoundException();

        await fraudDetectionRepository.FraudCheck(new FraudDetection(customer.Id, request.LoanId, customer.CardStore.Btid));


        // Make record of draw attempt
        if (request.TransactionType == TransactionType.Disburse)
        {
            await scheduleTransactionRepo.CreateDrawLog("ACH", customer.Id, request.LoanId,
                request.Amount, request.IpAddress);
        }

        if (!await lockRepository.TryLock(new LoanLock(customer.Loan.Id, teller.Value)))
        {
            throw new ApplicationLayerException("Cannot process transaction at this time");
        }
        if (customer.Loan.LoanInfo.Credit.Available < request.Amount && request.TransactionType == TransactionType.Disburse)
        {
            throw new ApplicationLayerException($"Maximum draw is {customer.Loan.LoanInfo.Credit.Available}");
        }

        if (request.TransactionType == TransactionType.Disburse)
        {
            customer.ScheduleDisburse(request.Amount, user.Teller);
        }
        else
        {
            throw new ApplicationLayerException("Can only disburse funds at this time");
        }

        await customerRepository.SaveChanges(cancellationToken);

        try
        {
            commService.SendEmail(request.LoanId, request.Amount);
            if(customer.Loan.LoanInfo.Location != Location.Texas && request.TransactionType == TransactionType.Disburse)
            {
                commService.AddCustomerServiceNote(request.LoanId, $"Draw requested: ${request.Amount}", user.Teller.Value);
            }
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }

        return new ScheduledAchViewModel(customer.Loan.ScheduledAch.Last());
    }
}
