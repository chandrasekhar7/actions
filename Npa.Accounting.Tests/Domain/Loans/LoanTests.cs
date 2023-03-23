using System;
using System.Collections.Generic;
using FluentAssertions;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Loans;

public class LoanTests
{
    [Fact]
    public void CannotSetLoanInfoIfNotNull()
    {
        var info = new LoanInfo(Location.California, new Credit(100, 100), 0, true);
        var loan = new Loan(1, 1, info);

        var ex = Assert.Throws<DomainLayerException>(() => loan.AddLoanInfo(info));
        ex.Message.Should().Be("Loan Info is already set");
    }
    
    [Fact]
    public void CanSetLoanInfo()
    {
        var info = new LoanInfo(Location.California, new Credit(100, 100), 0, true);
        var loan = new Loan(1, 1, null);

        loan.AddLoanInfo(info);
        loan.LoanInfo.Should().Be(info);
    }

    [Fact]
    public void CanAddTransaction()
    {
        var info = new LoanInfo(Location.California, new Credit(100, 100), 0, true);
        var card = new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), true);
        var loan = new Loan(1, 1, info);
        loan.AddTransaction(TransactionType.Credit, 100, new Teller("TST"), card);
        loan.Transactions.Count.Should().Be(1);
    }

    [Fact]
    public void NewTransactionThrowsForMissingLoanInfo()
    {
        var card = new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), true);
        var loan = new Loan(1, 1, null);

        List<Action> actions = new List<Action>()
        {
            () => loan.AddTransaction(TransactionType.Disburse, 100, new Teller("TST"), card),
            () => loan.AddScheduledAch(TransactionType.Disburse, 100, new Teller("TST")),
        };
        foreach (var a in actions)
        {
            a.Should().Throw<InvalidOperationException>()
                .WithMessage("Loan info is unavailable");
        }
    }
    
    [Fact]
    public void NewTransactionThrowsForInvalidAmount()
    {
        var info = new LoanInfo(Location.California, new Credit(100, 100), 0, true);
        var card = new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), true);
        var loan = new Loan(1, 1, info);

        List<Action> actions = new List<Action>()
        {
            () => loan.AddTransaction(TransactionType.Disburse, -100, new Teller("TST"), card),
            () => loan.AddScheduledAch(TransactionType.Disburse, -100, new Teller("TST")),
        };
        foreach (var a in actions)
        {
            a.Should().Throw<InvalidOperationException>()
                .WithMessage("Amount must be greater than zero, LoanId 1, Amount -100");
        }
    }

    [Fact]
    public void DuplicateACHTransaction()
    {
        List<ScheduledAch> scheduledAch = new List<ScheduledAch>();
        var card = new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), true);
        var info = new LoanInfo(Location.California, new Credit(1000, 1000), 0, true);
        scheduledAch.Add(new ScheduledAch(1, TransactionType.Disburse, null, 100, DateOnly.FromDateTime(DateTime.Now)));
        var loan = new Loan(1, 1, info, scheduledAch); 
        List<Action> actions = new List<Action>()
        {
            () => loan.AddScheduledAch(TransactionType.Disburse, 100, new Teller("TST")),
        };
        foreach (var a in actions)
        {
            a.Should().Throw<ConflictException>()
                .WithMessage("You cannot schedule another transaction");
        }
    }

    [Fact]
    public void AddScheduleDisburse()
    {
        List<ScheduledAch> scheduledAch = new List<ScheduledAch>();
        var card = new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), true);
        var info = new LoanInfo(Location.California, new Credit(1000, 1000), 0, true);
        scheduledAch.Add(new ScheduledAch(1, TransactionType.Disburse, null, 100, DateOnly.FromDateTime(DateTime.Now).AddDays(-1)));
        var loan = new Loan(1, 1, info, scheduledAch);
        List<Action> actions = new List<Action>()
        {
            () => loan.AddScheduledAch(TransactionType.Disburse, 3000, new Teller("TST")),
        };
        loan.ScheduledAch.Count.Should().Be(1);
    }
}