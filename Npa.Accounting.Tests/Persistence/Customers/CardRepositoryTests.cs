using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.DbContexts;
using Npa.Accounting.Tests.Persistence.Fixtures;
using Xunit;

namespace Npa.Accounting.Tests.Persistence.Customers;
public class CardRepositoryTests
{
    private DbContextOptions<CardDbContext> options;

    public List<CustomerCard> Cards = new List<CustomerCard>()
    {
        new CustomerCard(1000, 1000, 1, new LastFour("4567"), new Expiration(12, DateTime.Now.Year))
    };

    public Customer Customer => new Customer(1, new CustomerInfo(), new CardStore(1, 1, Cards),
          new Loan(1000, 1, new LoanInfo(Location.California, new Credit(1000, 1000), 100, false)));

    public CardRepositoryTests()
    {
        //context = fixture.Context;
        options = new DbContextOptionsBuilder<CardDbContext>()
            .UseInMemoryDatabase(databaseName: "temp").Options;
    }

    [Fact]
    public async Task CanUpdateCard()
    {
        using var context = new CardDbContext(options);
        var repo = new CardStoreRepository(context);
        context.Database.EnsureDeleted();
        context.Cards.AddRange(Cards);
        context.SaveChanges();
        
        var cards = await context.Cards.Where(c => c.PowerId == 1).ToListAsync();
        var newExp = new Expiration(12, DateTime.Now.Year + 5);
        cards.First().UpdateCard(newExp, false);
        await repo.SaveChanges();
        
        
        cards.First().Expiration.Should().Be(newExp);
        cards.First().CanDisburse.Should().BeFalse();
    }
    
    [Fact]
    public async Task CanRemoveCard()
    {
        using var context = new CardDbContext(options);
        var repo = new CardStoreRepository(context);
        context.Database.EnsureDeleted();
        context.Cards.AddRange(Cards);
        context.SaveChanges();
        
        var cards = await context.Cards.Where(c => c.PowerId == Customer.Id).ToListAsync();
        cards.First().RemoveCard();
        await repo.SaveChanges();
        
        
        cards.First().Deleted.Should().BeTrue();
        cards.First().DeletedOn.Should().NotBeNull();
    }
    
}