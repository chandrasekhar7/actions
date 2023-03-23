using Npa.Accounting.Common.Transactions;

namespace Npa.Accounting.Application.Scheduled;

public class CreateScheduledAchViewModel
{
    public int LoanId { get; set; } 
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
}