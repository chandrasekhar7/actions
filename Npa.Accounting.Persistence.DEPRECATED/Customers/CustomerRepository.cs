using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;

namespace Npa.Accounting.Persistence.DEPRECATED.Customers;

public class CustomerRepository : ICustomerRepository
{
    private readonly ICardStoreRepository cardStoreRepo;
    private readonly ICustomerInfoRepository customerInfoRepo;
    private readonly ILoanService loanService;
    private readonly ITransactionDbContext context;

    public CustomerRepository(
        ITransactionDbContext context,
        ICardStoreRepository cardStoreRepo,
        ILoanService loanService,
        ICustomerInfoRepository customerInfoRepo
    )
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.cardStoreRepo = cardStoreRepo ?? throw new ArgumentNullException(nameof(cardStoreRepo));
        this.loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        this.customerInfoRepo = customerInfoRepo ?? throw new ArgumentNullException(nameof(customerInfoRepo));
   
    }

    public async Task<Customer?> GetWithLoanByToken(int token, int loanId, CancellationToken t = default)
    {
        var cardStore = await cardStoreRepo.GetByCardToken(token, t) ?? throw new NotFoundException();
        var loan = await context.Loans.Include(x => x.ScheduledAch.Where(y => y.PaymentId == null))
                       .FirstOrDefaultAsync(l => l.CustomerId == cardStore.CustomerId && l.Id == loanId, t) ??
                   throw new NotFoundException();
        return await GetCustomer(cardStore, loan, t);
    }

    public async Task<Customer?> GetWithLoan(int loanId, CancellationToken cancellationToken)
    {
        var loan = await context.Loans
                       .Include(x => x.ScheduledAch.Where(x => x.Cancelled == null))
                       .FirstOrDefaultAsync(l => l.Id == loanId) ??
                   throw new NotFoundException();
        var cardStore = await cardStoreRepo.GetById(loan.CustomerId, cancellationToken) ?? new CardStore(0,loan.CustomerId, new List<CustomerCard>());
        return await GetCustomer(cardStore, loan, cancellationToken);
    }

    public async Task<Customer?> GetWithLoan(int customerId, int loanId, CancellationToken cancellationToken)
    {
        var loan = await context.Loans.Include(x => x.ScheduledAch.Where(x => x.PaymentId == null))
            .FirstOrDefaultAsync(l => l.CustomerId == customerId && l.Id == loanId) ?? throw new NotFoundException();
        var cardStore = await cardStoreRepo.GetById(loan.CustomerId, cancellationToken) ?? new CardStore(0,customerId, new List<CustomerCard>());
        return await GetCustomer(cardStore, loan, cancellationToken);
    }

    private async Task<Customer> GetCustomer(CardStore cardStore, Loan loan,
        CancellationToken cancellationToken = default)
    {
        var loanInfoTask = loanService.GetLoan(loan.Id,
            cancellationToken);
        var customerInfoTask = customerInfoRepo.GetInfoById(cardStore.CustomerId, cancellationToken);
        try
        {
            await Task.WhenAll(loanInfoTask, customerInfoTask);
        }
        catch (AggregateException ex)
        {
            throw ex.InnerExceptions.First();
        }

        loan.AddLoanInfo(loanInfoTask.Result);
        return new Customer(cardStore.CustomerId, customerInfoTask.Result, cardStore, loan);
    }


    public async Task SaveChanges(CancellationToken t = default, TransactionType? originalTransType = null, int? rescindPaymentId = null)
    {
        await context.SaveChangesAsync(t);
        await cardStoreRepo.SaveChanges(t);
        var loans = context.ChangeTracker.Entries<Loan>();
        foreach (var l in loans)
        {
            foreach (var trans in l.Entity.Transactions)
            {
                if (rescindPaymentId is not null) 
                {
                    trans.TransactionType = TransactionType.Rescind;
                }
                try
                {
                    await loanService.ApplyTransaction(trans, CancellationToken.None, rescindPaymentId);
                }
                catch(Exception ex)
                {
                    throw new PersistenceLayerException(ex.Message);
                }
                
            }
        }
    }
}