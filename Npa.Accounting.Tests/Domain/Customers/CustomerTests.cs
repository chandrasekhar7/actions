using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Customers;

public class CustomerTests
{
    private ICustomerInfoRepository customerInfoRepo;

    public List<CustomerCard> DefaultCards => new List<CustomerCard>()
    {
        new CustomerCard(1000, 1000, 1, new LastFour("4567"), new Expiration(12, DateTime.Now.Year))
    };

    public Customer GetCustomer(Location l, List<CustomerCard>? cards, CustomerInfo customerInfo, bool partialPayments = false)
    {
        var mockedCustomerInfoRepo = new Mock<ICustomerInfoRepository>();
        mockedCustomerInfoRepo.Setup(x => x.GetInfoById(It.IsAny<int>(), new System.Threading.CancellationToken())).ReturnsAsync(() => customerInfo);
        customerInfoRepo = mockedCustomerInfoRepo.Object;
        if (cards is null || cards.Count == 0)
            return new Customer(1, customerInfoRepo.GetInfoById(It.IsAny<int>(), new System.Threading.CancellationToken()).Result, new CardStore(1, 1, DefaultCards),
                new Loan(1000, 1, new LoanInfo(l, new Credit(1000, 1000), 100, partialPayments)));
        else
            return new Customer(1, customerInfoRepo.GetInfoById(It.IsAny<int>(), new System.Threading.CancellationToken()).Result, new CardStore(1, 1, cards),
                new Loan(1000, 1, new LoanInfo(l, new Credit(1000, 1000), 100, partialPayments)));
    }
    // DISABLED SINCE WE HAVE HARD STOP ON CARD DISBURSEMENT
    // [Theory]
    // [InlineData(Location.California)]
    // [InlineData(Location.Texas)]
    // [InlineData(Location.Kansas)]
    // public void Merchant702UsedForDisbursement(Location l)
    // {
    //     CustomerInfo customerInfo = new CustomerInfo
    //     {
    //         IsLoanLocked = false,
    //         IsMilitary = false,
    //         IsPraLimit = false,
    //         Location = 700
    //     };
    //     var customer = ShouldReturnCustomer(l, null, customerInfo);
    //
    //     var trans = customer.DisburseCard(1000, 1000, new Teller("ILM"));
    //     trans.CardTransaction.MerchantId.Id.Should().Be(702);
    // }
    //
    // DISABLED SINCE WE HAVE HARD STOP ON CARD DISBURSEMENT
    [Fact]
    public void SchedulesDispursement()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };
    
        var start = DateTime.Now;
        var customer = GetCustomer(Location.California, null, customerInfo);
        var scheduled = customer.ScheduleDisburse(200, new Teller("EC1"));
        scheduled.Amount.Should().Be(200);
        scheduled.Cancelled.Should().BeNull();
        scheduled.Created.TimeStamp.Should().BeWithin((DateTime.Now - start));
        scheduled.Created.Teller.Value.Should().Be("EC1");
        scheduled.LoanId.Should().Be(1000);
        scheduled.ScheduledDate.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        scheduled.PaymentId.Should().BeNull();
        scheduled.TransactionType.Should().Be(TransactionType.Disburse);
    }

    [Fact]
    public void ThrowsIfCannotDisburse()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };

        var cards = new List<CustomerCard>();
        cards.Add(new CustomerCard(1001, 1000, 1,new LastFour("1234"), new Expiration(12, DateTime.Now.Year), false));
        var customer = GetCustomer(Location.California, cards, customerInfo);
        Action act = () => customer.DisburseCard(1001, 1000,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    [Fact]
    public void ThrowsIfCannotDisbursePraLimit()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = true,
            Location = 700
        };

        var customer = GetCustomer(Location.California, null, customerInfo);

        Action act = () => customer.DisburseCard(1000, 1000,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsIfLoanLocked()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = true,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };

        var customer = GetCustomer(Location.California, null, customerInfo);

        Action act = () => customer.DisburseCard(1000, 500,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsIfMilitary()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = true,
            IsPraLimit = false,
            Location = 700
        };

        var customer = GetCustomer(Location.California, null, customerInfo);

        Action act = () => customer.DisburseCard(1000, 500,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsIfLessThan50Draw()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };
        
        var customer = GetCustomer(Location.California, null, customerInfo);

        Action act = () => customer.DisburseCard(1000, 49,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsWhenPartialPaymentsNotAllowed()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };
        var customer = GetCustomer(Location.California, null, customerInfo, false);
        Action act = () => customer.Debit(1000, 49,  new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }

    [Fact]
    public void DebitCreatesTransaction()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };
        var customer = GetCustomer(Location.California, null, customerInfo, true);
        var trans = customer.Debit(1000, 49, new Teller("ILM"));
        trans.TransactionType.Should().Be(TransactionType.Debit);
        trans.Amount.Should().Be(49);
        trans.CardTransaction?.Card.CardToken.Should().Be(1000);
        trans.CardTransaction?.MerchantId.Should().Be(new Merchant(Location.California));
        trans.CardTransaction?.ReturnMessage.Should().Be(ReturnMessage.Default);
    }
    
    [Fact]
    public void UpdatingTransactionChangesValues()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 700
        };
        var customer = GetCustomer(Location.California, null, customerInfo, true);
        var trans = customer.Debit(1000, 49, new Teller("ILM"));
        trans.CardTransaction!.UpdateResult(new ReturnMessage(CardReturnStatus.Deny, "Something", "Message", "1234"));
        trans.CardTransaction.ReturnMessage.Should().Be(new ReturnMessage(CardReturnStatus.Deny, "Something", "Message", "1234"));
    }

    [Fact]
    public void ThrowsIfLoanLockedForScheduleDisburse()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = true,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 702
        };
        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        Action act = () => customer.ScheduleDisburse(100, new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsPraLimitForScheduleDisburse()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = true,
            Location = 702
        };

        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        Action act = () => customer.ScheduleDisburse(100, new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }
    
    [Fact]
    public void ThrowsMilitaryForScheduleDisburse()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = true,
            IsPraLimit = false,
            Location = 702
        };

        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        Action act = () => customer.ScheduleDisburse(100, new Teller("ILM"));
        act.Should().Throw<DomainLayerException>();
    }

    [Fact]
    public void CanScheduleDisburse()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 702
        };

        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        var ach = customer.ScheduleDisburse(100, new Teller("ILM"));
        ach.TransactionType.Should().Be(TransactionType.Disburse);
        ach.Amount.Should().Be(100);
        ach.ScheduledDate.Should().Be(DateOnly.FromDateTime(DateTime.Now));
    }
    
    [Fact]
    public void ThrowsWhenDisbursingMoreThanCreditMinusPendingAchFunding()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 702
        };

        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        var ach = customer.ScheduleDisburse(1000, new Teller("ILM"));
        Assert.Throws<DomainLayerException>(() =>
            customer.DisburseCard(1000, 100, new Teller("TST")));
    }
    
    [Fact]
    public void AllowsDisbursingWhenLessIsScheduled()
    {
        CustomerInfo customerInfo = new CustomerInfo
        {
            IsLoanLocked = false,
            IsMilitary = false,
            IsPraLimit = false,
            Location = 702
        };

        var customer = GetCustomer(Location.Kansas, null, customerInfo, true);
        var ach = customer.ScheduleDisburse(100, new Teller("ILM"));
        customer.DisburseCard(1000, 900, new Teller("TST"));
        customer.Loan.ScheduledAch.Count.Should().Be(1);
        customer.Loan.Transactions.Count.Should().Be(1);
    }
}