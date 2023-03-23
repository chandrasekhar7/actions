using System.Collections.Generic;
using System.Linq;

namespace Npa.Accounting.Domain.DEPRECATED.Customers;

public class CardStore
{
    private readonly List<CustomerCard> cards = new List<CustomerCard>();
    public int Btid { get; }
    public int CustomerId { get; private set; }
    public List<CustomerCard> Cards => cards.ToList();

    private CardStore() {}
    public CardStore(int btid,int customerId, List<CustomerCard> cards)
    {
        Btid = btid;
        CustomerId = customerId;
        this.cards = cards;
    }
    
    public void AddCard(CustomerCard card)
    {
        if (cards.Exists(c => c.CardToken == card.CardToken))
        {
            throw new DomainLayerException("Card already exists");
        }

        // New cards automatically become primary card
        cards.Insert(0,card);
    }
}