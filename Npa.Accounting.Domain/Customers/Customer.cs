using Npa.Accounting.Domain.CardTransactions;

namespace Npa.Accounting.Domain.Customers;

public class Customer : Entity<int>
{
    private readonly List<CardTransaction> cardTransactions = new List<CardTransaction>();
    private readonly List<CustomerCard> cards = new List<CustomerCard>();
    public List<CardTransaction> CardTransactions => cardTransactions.ToList();
    public List<CustomerCard> Cards => cards.ToList();

    private Customer()
    {
        
    }

    public void AddTransaction(CardTransaction t)
    {
        // TODO: Add logic to delete card
        if (t.ReturnCodeId > 300) // Whatever you had Stephen
        {
            cards.FirstOrDefault(x => x.Id == t.CardId)?.RemoveCard();
        }
    }
}