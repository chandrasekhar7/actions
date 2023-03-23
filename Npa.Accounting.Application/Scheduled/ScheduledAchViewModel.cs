using System;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;

namespace Npa.Accounting.Application.Scheduled;

public class ScheduledAchViewModel
{
    public int ScheduleId { get; }
    public int LoanId { get; }
    public int? PaymentId { get; }
    public TransactionType TransactionType { get; }
    public DateTime CreatedOn { get; }
    public string CreatedBy { get; }
    public DateTime? CancelledOn { get; }
    public string? CancelledBy { get; }
    public decimal Amount { get; }
    public DateOnly ScheduledDate { get; }

    public ScheduledAchViewModel(ScheduledAch s)
    {
        ScheduleId = s.Id;
        LoanId = s.LoanId;
        PaymentId = s.PaymentId;
        TransactionType = s.TransactionType;
        CreatedOn = s.Created.TimeStamp;
        CreatedBy = s.Created.Teller.ToString();
        CancelledOn = s.Cancelled?.TimeStamp;
        CancelledBy = s.Cancelled?.Teller.ToString();
        Amount = s.Amount;
        ScheduledDate = s.ScheduledDate;
    }
}