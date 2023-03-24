using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Npa.Accounting.Common;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Loans;

public class Loan : Entity<int>
{
    // private readonly List<AchTransaction> achTransactions = new List<AchTransaction>();
    // private readonly List<CardTransaction> cardTransactions = new List<CardTransaction>();
    private readonly List<Transaction> transactions = new List<Transaction>();
    private readonly List<ScheduledAch> scheduledAch = new List<ScheduledAch>();
    
    public int CustomerId { get; }
    public LoanInfo LoanInfo { get; private set; }

    public List<Transaction> Transactions => transactions.ToList();
    // public List<AchTransaction> AchTransactions => achTransactions.ToList();
    // public List<CardTransaction> CardTransactions => cardTransactions.ToList();
    public List<ScheduledAch> ScheduledAch => scheduledAch.ToList();

    private Loan()
    {
    }

    public Loan(int id, int customerId, LoanInfo loanInfo, List<ScheduledAch>? scheduledAch = null)
    {
        Id = id;
        LoanInfo = loanInfo;
        CustomerId = customerId;
        this.scheduledAch = scheduledAch ?? new List<ScheduledAch>();
    }

    public void AddLoanInfo(LoanInfo loanInfo)
    {
        if (LoanInfo != null)
        {
            throw new DomainLayerException("Loan Info is already set");
        }

        LoanInfo = loanInfo;
    }

    private decimal LimitAvailable() => LoanInfo.Credit.Available - scheduledAch.Where(x => x.PaymentId == null).Sum(x => x.Amount);

    internal void AddTransaction(TransactionType type, decimal amount, Teller teller, CustomerCard card)
    {
        ThrowIfInvalidForLoan(type, amount, Id);
        var trans = new Transaction(Id, amount, DateTime.Now, type, teller,new CardTransaction(card,new Merchant((int) LoanInfo.Location)));
        transactions.Add(trans);
    }

    internal void AddScheduledAch(TransactionType type, decimal amount, Teller teller)
    {
        ThrowIfInvalidForLoan(type, amount, Id);
        var ach = new ScheduledAch(Id, type, new ModifiedBy(DateTime.Now, teller), amount,
            DateOnly.FromDateTime(DateTime.Now));
        if (ach.TransactionType == TransactionType.Disburse)
        {
            ScheduleDisbursement(ach);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public void CancelScheduledAch(int id, Teller teller)
    {
        var ach = scheduledAch.FirstOrDefault(x => x.Id == id) ?? throw new DomainLayerException($"Cannot find ACH to cancel with id: {id}");

        ach.CancelAch(teller);
    }

    private void ScheduleDisbursement(ScheduledAch ach)
    {
        if (scheduledAch.Any(a => a.ScheduledDate == DateOnly.FromDateTime(DateTime.Today)))
        {
            throw new ConflictException("You cannot schedule another transaction");
        }

        if (ach.ScheduledDate < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new DomainLayerException(
                $"Cannot schedule for {ach.ScheduledDate}: Must be a date in the future");
        }

        scheduledAch.Add(ach);
    }

    private void ThrowIfInvalidForLoan(TransactionType type, decimal amount, int loanId)
    {
        if (loanId == 0)
        {
            throw new InvalidOperationException("LoanId is required");
        }
        if (LoanInfo == null)
        {
            throw new InvalidOperationException("Loan info is unavailable");
        }
        if (amount < 0)
        {
            throw new InvalidOperationException($"Amount must be greater than zero, LoanId {loanId}, Amount {amount}");
        }
        if (type == TransactionType.Disburse && LimitAvailable() < amount)
        {
            throw new DomainLayerException($"Only ${LimitAvailable()} available credit, LoanId {loanId}, Amount Attempted ${amount}");
        }
        else if (type == TransactionType.Debit && LoanInfo.Balance < amount)
        {
            throw new DomainLayerException($"Loan only has a balance of ${LoanInfo.Balance}, LoanId {loanId}, Amount Attempted ${amount}, Balance {LoanInfo.Balance}");
        }
        else if (type == TransactionType.Debit && !LoanInfo.PartialPayments &&
                 LoanInfo.Balance != amount)
        {
            throw new DomainLayerException($"Partial payments not allowed, LoanId {loanId}, Amount Attempted {amount}, Balance {LoanInfo.Balance}");
        }
    }

    // public CardTransaction AddTransaction(TransactionType type, Customer customer, CustomerCard card, decimal amount, Teller t)
    // {
    //     Merchant m = type == TransactionType.Disburse || Location == Location.Kansas ? new Merchant(702) : new Merchant((int) Location);
    //     if (type == TransactionType.Disburse && !customer.CanDisburse())
    //     {
    //         throw new DomainLayerException("Customers card cannot be disbursed to");
    //     }
    //     var trans = CardTransaction.Create(m, Id, new TokenizedCard(card.Id, Id),
    //         new Transaction(amount, DateTime.Now, type, t));
    //     cardTransactions.Add(trans);
    //     return trans;
    // }
}