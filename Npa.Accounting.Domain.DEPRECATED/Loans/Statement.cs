namespace Npa.Accounting.Domain.DEPRECATED.Loans;

public class Statement : Entity<StatementId>
{
    public decimal Balance { get; }

    public Statement(StatementId id, decimal balance)
    {
        Id = id;
        Balance = balance;
    }
}