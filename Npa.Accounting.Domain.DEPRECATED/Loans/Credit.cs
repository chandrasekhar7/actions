namespace Npa.Accounting.Domain.DEPRECATED.Loans;

public record Credit
{
    public decimal Limit { get; }
    public decimal Available { get; }
    
    public Credit(decimal limit, decimal available)
    {
        Limit = limit;
        Available = available;
    }
}