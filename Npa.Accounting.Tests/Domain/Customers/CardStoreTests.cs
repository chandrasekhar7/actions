using System.Collections.Generic;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Customers;

public class CardStoreTests
{
    [Fact]
    public void CanAddCard()
    {
        var store = new CardStore(1, 1, new List<CustomerCard>());
        store.AddCard(new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25), false));
    }

    [Fact]
    public void ThrowsIfCardExists()
    {
        var store = new CardStore(1, 1, new List<CustomerCard>()
        {
            new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25), false)
        });
        var ex = Assert.Throws<DomainLayerException>(() =>
            store.AddCard(new CustomerCard(1, 1, 1, new LastFour("1234"),
                new Expiration(12, 25), false)));
        
        Assert.Equal("Card already exists", ex.Message);
    }
}