using System;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Transactions;

public class Transaction: Entity<int>
{
    public int LoanId { get; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; }
    public DateTime CreatedOn { get; }
    public Teller Teller { get; }
    public AchTransaction? AchTransaction { get; }
    public CardTransaction? CardTransaction { get; }
    private Transaction() {}
    
    private Transaction(int loanId, decimal amount, DateTime createdOn, TransactionType transactionType, Teller teller)
    {
        if (amount < 0)
        {
            throw new ArgumentException($"{nameof(amount)} cannot be less than 0");
        }

        LoanId = loanId;
        Amount = transactionType == TransactionType.Debit ? amount : -amount;
        CreatedOn = createdOn;
        TransactionType = transactionType;
        Teller = teller;
    }

    internal Transaction(int loanId, decimal amount, DateTime createdOn, TransactionType transactionType, Teller teller,
        AchTransaction achTransaction) : this(loanId, amount, createdOn, transactionType, teller)
    {
        AchTransaction = achTransaction;
    }
    
    internal Transaction(int loanId, decimal amount, DateTime createdOn, TransactionType transactionType, Teller teller,
        CardTransaction cardTransaction) : this(loanId, amount, createdOn, transactionType, teller)
    {
        CardTransaction = cardTransaction;
    }
}