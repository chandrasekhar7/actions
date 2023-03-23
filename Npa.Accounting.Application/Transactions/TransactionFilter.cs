namespace Npa.Accounting.Application.Transactions;

public class TransactionFilter
{
    public int? TransactionId { get; set; }
    public bool? Pending { get; set; }
    public int? PowerId { get; set; }
    public int? LoanId { get; set; }
}