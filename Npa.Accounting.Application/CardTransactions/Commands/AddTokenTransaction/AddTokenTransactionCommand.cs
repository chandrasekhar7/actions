using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Cards;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using static Npa.Accounting.Common.Teller;

namespace Npa.Accounting.Application.CardTransactions.Commands.AddTokenTransaction;

public record AddTokenTransactionCommand(int Token, NewTransactionViewModel NewTransaction, string IpAddress) : IRequest<CardTransactionViewModel>;

public class AddTokenTransactionCommandHandler : IRequestHandler<AddTokenTransactionCommand, CardTransactionViewModel>
{
    private readonly ICardTransactionServiceDeprecated paymentServiceDeprecated;
    private readonly ICustomerRepository customerRepository;
    private readonly IUserService userService;
    private readonly ILoanLockRepository lockRepository;
    private readonly IFraudDetectionRepository fraudDetectionRepository;
    private readonly ICommunicationsService commService;
    private readonly ITransactionReadDbFacade facade;
    private readonly IScheduleTransactionRepository scheduleTransactionRepo;

    public AddTokenTransactionCommandHandler(ICustomerRepository customerRepository, ICardTransactionServiceDeprecated paymentServiceDeprecated,
        IUserService userService, ILoanLockRepository lockRepository, ICommunicationsService commService, ITransactionReadDbFacade context,
        IScheduleTransactionRepository scheduleTransactionRepo, IFraudDetectionRepository fraudDetectionRepository)
    {
        facade = context ?? throw new ArgumentNullException(nameof(context));
        this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        this.paymentServiceDeprecated = paymentServiceDeprecated ?? throw new ArgumentNullException(nameof(paymentServiceDeprecated));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        this.lockRepository = lockRepository ?? throw new ArgumentNullException(nameof(lockRepository));
        this.commService = commService ?? throw new ArgumentNullException(nameof(commService));
        this.scheduleTransactionRepo = scheduleTransactionRepo ?? throw new ArgumentNullException(nameof(scheduleTransactionRepo));
        this.fraudDetectionRepository = fraudDetectionRepository ?? throw new ArgumentNullException(nameof(fraudDetectionRepository));
    }

    public async Task<CardTransactionViewModel> Handle(AddTokenTransactionCommand request, CancellationToken t1 = default)
    {
        var t = request.NewTransaction;
        var user = userService.GetUser();
        var teller = user.Teller;
        var customer = await customerRepository.GetWithLoanByToken(request.Token, request.NewTransaction.LoanId, t1) ??
                       throw new NotFoundException();

        // Make record of draw attempt
        if (t.TransactionType == TransactionType.Disburse)
        {
            await scheduleTransactionRepo.CreateDrawLog("DC", customer.Id, request.NewTransaction.LoanId,
                request.NewTransaction.Amount, request.IpAddress);
        }

        if (!user.IsInRole("administrator") && customer.Id != user.PowerId)
        {
            throw new NotFoundException();
        }

        var checkFraud = await fraudDetectionRepository.FraudCheck(new FraudDetection(customer.Id, request.NewTransaction.LoanId, customer.CardStore.Btid));


        if (!await lockRepository.TryLock(new LoanLock(customer.Loan.Id, teller.Value)))
        {
            throw new ApplicationLayerException("Cannot process transaction at this time");
        }

        if (customer.Loan.LoanInfo.Credit.Available < request.NewTransaction.Amount && t.TransactionType == TransactionType.Disburse)
        {
            throw new ApplicationLayerException($"Maximum draw is {customer.Loan.LoanInfo.Credit.Available}");
        }

        if (t.TransactionType == TransactionType.Rescind && teller.Value == "ILM")
        {
            throw new ApplicationLayerException("Can only rescind by advocates");
        }

        try
        {
            if (customer.Loan.LoanInfo.Location != Location.Texas && t.TransactionType == TransactionType.Disburse)
            {
                commService.AddCustomerServiceNote(request.NewTransaction.LoanId, $"Draw requested: ${request.NewTransaction.Amount}, attempting to instant fund", user.Teller.Value);
            }
        }
        catch
        {
            throw new ApplicationLayerException("Unable to create customer service note");
        }

        var isEligible = (await facade.QueryProcAsync<bool>("dbo.CheckLeadsInstantFunding", new { LoanID = customer.Loan.Id })).ToList();

        if (isEligible.Count > 0 && !isEligible.First() && t.TransactionType == TransactionType.Disburse)
        {
            throw new ApplicationLayerException("The lead is not eligible for instant funding");
        }

        var trans = t.TransactionType switch
        {
            TransactionType.Disburse => customer.DisburseCard(request.Token, t.Amount, teller),
            TransactionType.Debit => customer.Debit(request.Token, request.NewTransaction.Amount, teller),
            TransactionType.Credit => throw new NotImplementedException(),
            TransactionType.Rescind => customer.Debit(request.Token, request.NewTransaction.Amount, teller),
            _ => throw new ApplicationLayerException($"Transaction type {t.TransactionType} not allowed")
        };

        trans.CardTransaction!.UpdateResult(await paymentServiceDeprecated.Process(trans, t1));
        await customerRepository.SaveChanges(t1, t.TransactionType, t.rescindPaymentID);


        if (trans.TransactionType == TransactionType.Debit && trans.CardTransaction.ReturnMessage.Status == CardReturnStatus.Approve)
        {
            commService.SendPaymentConfirmSMS(trans.Id);
        }

        if (trans.TransactionType == TransactionType.Disburse && trans.CardTransaction.ReturnMessage.Status != CardReturnStatus.Approve)
        {
            commService.SendInstantFundFailedSMS(trans.LoanId);
        }

        if(fraudDetectionRepository.LookupFraudReturnCode(trans, teller))
        {
            await fraudDetectionRepository.ExecFraudLock("DC PAYMENT RESULT", "DC PAYMENT RESULT", customer.Id);
        }

        trans.TransactionType = t.TransactionType;

        try
        {
            if (teller.isAdditionalPaymentTellers(teller.ToString())
                && GetReturnCode(trans.CardTransaction.ReturnMessage.Status) == "A"
                )
            {
                commService.AddAdditionalPayment(request.NewTransaction.LoanId, request.NewTransaction.Amount);
            }
        }
        catch
        {
            commService.AddCustomerServiceNote(t.LoanId, "An error occured while adding additional payment", user.Teller.Value);
        }
        return new CardTransactionViewModel(trans);
    }

    private static string GetReturnCode(CardReturnStatus c) => c switch
    {
        CardReturnStatus.Deny => "D",
        CardReturnStatus.Error => "E",
        CardReturnStatus.Approve => "A",
        CardReturnStatus.Void => "V",
        CardReturnStatus.NotStarted => "N",
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };
}