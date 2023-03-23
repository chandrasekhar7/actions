using System;
using FluentAssertions;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Customers;

public class CustomerCardTests
{
    [Fact]
    public void UpdatingExpirationToBeforeTodayMarksForDeletion()
    {
        var card = new CustomerCard(1000, 1000, 1,new LastFour("4567"), new Expiration(12, DateTime.Now.Year));
        card.UpdateCard(new Expiration(12, 2020), card.CanDisburse);
        card.Deleted.Should().BeTrue();
        card.DeletedOn.Should().NotBeNull();
    }
    
    [Fact]
    public void UpdatingExpirationMarksForUpdate()
    {
        var card = new CustomerCard(1000, 1000, 1,new LastFour("4567"), new Expiration(12, DateTime.Now.Year));
        card.UpdateCard(new Expiration(12, DateTime.Now.Year), card.CanDisburse);
    }
}