using System;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Transactions;

public class TransactionUnitTests
{
    [Fact]
    public void TransactionDoesNotInvertDebitAmount()
    {
        var t = new Transaction(1, 200, DateTime.Now, TransactionType.Debit, new Teller("EC1"),
            new CardTransaction(new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25)), new Merchant(700)));
        
        Assert.Equal(200, t.Amount);
    }
    
    [Fact]
    public void TransactionInvertCreditAmount()
    {
        var t = new Transaction(1, 200, DateTime.Now, TransactionType.Credit, new Teller("EC1"),
            new CardTransaction(new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25)), new Merchant(700)));
        
        Assert.Equal(-200, t.Amount);
    }
    
    [Fact]
    public void TransactionInvertDisburseAmount()
    {
        var t = new Transaction(1, 200, DateTime.Now, TransactionType.Disburse, new Teller("EC1"),
            new CardTransaction(new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25)), new Merchant(700)));
        
        Assert.Equal(-200, t.Amount);
    }
}