namespace Npa.Accounting.Domain.CardTransactions;

public class CardTransaction : Entity<int>
{
    public int ReturnCodeId { get; }
    public int CardId { get; }
}