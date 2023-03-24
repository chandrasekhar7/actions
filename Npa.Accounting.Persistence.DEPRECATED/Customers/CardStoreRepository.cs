using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.Customers;

internal class CardStoreRepository : ICardStoreRepository
{
    private readonly ICardDbContext context;

    public CardStoreRepository(ICardDbContext cardContext)
    {
        this.context = cardContext ?? throw new ArgumentNullException(nameof(cardContext));
    }

    public async Task<CardStore?> GetById(int customerId, CancellationToken t = default)
    {
        var cards = await context.Cards.Where(f => f.PowerId == customerId).ToListAsync(t);
        if (cards.Count > 0)
        {
            return new CardStore(int.Parse(cards.First().Btid), cards.First().PowerId, cards.Where(d => !d.Deleted).ToList());
        }

        return null;
    }
    
    public async Task<CardStore?> GetByCardToken(int token, CancellationToken t = default)
    {
        
        var card = await context.Cards.FirstOrDefaultAsync(f => !f.Deleted && f.CardToken == token, t);
        if (card != null)
        {
            return new CardStore(int.Parse(card.Btid), card.PowerId, new List<CustomerCard>() { card});
        }

        return null;
    }

    public async Task SaveChanges(CancellationToken t = default)
    {
        await context.SaveChangesAsync(t);
    }

    public Task<bool> CanDisburseAsync(Customer c, CancellationToken t = default)
    {
        throw new NotImplementedException();
    }


    // public Task<bool> IsValidTransactionAsync(CardTransaction trans, CancellationToken t = default) =>
    //     read.QueryFirstOrDefaultAsync<bool>("SELECT 1 FROM loan.Loans WHERE PowerId = @PowerId AND LoanId = @LoanId",
    //         new {PowerId = trans.Card.CustomerId, LoanId = trans.LoanId}, null, t);
}