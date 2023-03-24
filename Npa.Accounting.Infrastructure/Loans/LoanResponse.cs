
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Infrastructure.Loans;

public class LoanResponse
{
    public int LoanId { get; set; }
    public int PowerId { get; set; }
    public Location Location { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal AvailableLimit { get; set; }
    public Amount Balance { get; set; }
    public string LoanType { get; set; }
}