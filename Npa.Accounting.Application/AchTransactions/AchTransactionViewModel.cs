using System;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Application.AchTransactions;

public class AchTransactionViewModel
{
    public int Id { get; init; }
    public int LoanId { get; init; }
    public TransactionType TransactionType { get; init; }
    public decimal Amount { get; init; }
    public DateTime CreatedOn { get; init; }
    public string Teller { get; init; } = default!;
    public DateTime? StatusDate { get; init; }
    public string? ReturnCode { get; init; } = default!;
    public string? ReturnMessage { get; init; }
    public bool Pending { get; init; }

    private AchTransactionViewModel() {}
    public AchTransactionViewModel(Transaction t)
    {
        if (t.AchTransaction == null)
        {
            throw new ArgumentNullException(nameof(t.AchTransaction));
        }

        Id = t.Id;
        LoanId = t.LoanId;
        TransactionType = t.TransactionType;
        Amount = t.Amount;
        CreatedOn = t.CreatedOn;
        Teller = t.Teller.ToString();
        StatusDate = t.AchTransaction.StatusDate;
        ReturnCode = t.AchTransaction.ReturnCode;
        ReturnMessage = t.AchTransaction.ReturnMessage;
        Pending = t.AchTransaction.ReturnCode == null;
    }
}