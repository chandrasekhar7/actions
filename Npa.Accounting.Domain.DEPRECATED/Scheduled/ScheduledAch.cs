using System;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Domain.DEPRECATED.Scheduled;

public class ScheduledAch : Entity<int>
{
    public int LoanId { get; }
    public int? PaymentId { get; }
    public TransactionType TransactionType { get; }
    public ModifiedBy Created { get; }
    public ModifiedBy? Cancelled { get; private set;}
    public decimal Amount { get; }
    public DateOnly ScheduledDate { get; }

    private ScheduledAch() { }

    internal ScheduledAch(int loanId, TransactionType type, ModifiedBy created, decimal amount, DateOnly scheduledDate, int? paymentId = null, ModifiedBy? cancelled = null )
    {
        LoanId = loanId;
        TransactionType = type;
        Created = created;
        Amount = amount;
        ScheduledDate = scheduledDate;
        PaymentId = paymentId;
        Cancelled = cancelled;

    }

    internal void CancelAch(Teller teller){
        if(PaymentId.HasValue){
            throw new DomainLayerException("Cannot cancel a completed ACH");
        }
        if(Cancelled is not null ){
            throw new DomainLayerException("Ach already cancelled");
        }
        var modBy = new ModifiedBy(DateTime.Now, teller); 

        Cancelled = modBy;

    } 

}