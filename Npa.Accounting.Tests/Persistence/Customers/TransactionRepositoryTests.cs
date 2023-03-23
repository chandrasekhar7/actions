using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Customers;
using Npa.Accounting.Tests.Persistence.Fixtures;
using Xunit;

namespace Npa.Accounting.Tests.Persistence.Customers;

[Collection("TransactionDb")]
public class TransactionRepositoryTests
{
    private readonly ICustomerRepository repo;
    private readonly ITransactionDbContext context;
    private ICustomerInfoRepository customerInfoRepo;
    
    public List<CustomerCard> Cards => new List<CustomerCard>()
    {
        new CustomerCard(1000, 1000, 1, new LastFour("4567"), new Expiration(12, DateTime.Now.Year))
    };

    public Customer ShouldReturnCustomer(CustomerInfo customerInfo)
    {
        var mockedCustomerInfoRepo = new Mock<ICustomerInfoRepository>();
        mockedCustomerInfoRepo.Setup(x => x.GetInfoById(It.IsAny<int>(), new System.Threading.CancellationToken())).ReturnsAsync(() => customerInfo);
        customerInfoRepo = mockedCustomerInfoRepo.Object;
        return new Customer(1, customerInfoRepo.GetInfoById(It.IsAny<int>(), new System.Threading.CancellationToken()).Result, new CardStore(1, 1, Cards),
          new Loan(1000, 1, new LoanInfo(Location.California, new Credit(1000, 1000), 100, false)));
    }

    public TransactionRepositoryTests(TransactionDbFixture fixture)
    {
        var mockedCardRepo = new Mock<ICardStoreRepository>();
        var mockedLoanService = new Mock<ILoanService>();
        var mockedCustomerInfoRepo = new Mock<ICustomerInfoRepository>();
        var mockFacade = new Mock<IDbFacade>();
        context = fixture.Context;
        
        repo = new CustomerRepository(fixture.Context, mockedCardRepo.Object,mockedLoanService.Object, mockedCustomerInfoRepo.Object);
    }

    // DISABLED SINCE WE HAVE HARD STOP ON CARD DISBURSEMENT
    // [Fact]
    // public async Task CanSaveNewTransaction()
    // {
    //     CustomerInfo customerInfo = new CustomerInfo
    //     {
    //         IsLoanLocked = false,
    //         IsMilitary = false,
    //         IsPraLimit = false,
    //         Location = 700
    //     };
    //
    //     DetachEntities();
    //     var customer = ShouldReturnCustomer(customerInfo);
    //     context.Entry(customer.Loan).State = EntityState.Unchanged;
    //     //context.Loans.Attach(Customer.Loan);
    //     customer.DisburseCard(1000, 1000,  new Teller("ILM"));
    //     await repo.SaveChanges();
    //     //DetachEntities();
    //     customer.Loan.Transactions.Last().Id.Should().BeGreaterThan(0);
    // }

    private void DetachEntities()
    {

        foreach (var entry in context.ChangeTracker.Entries().ToArray())
        {
            entry.State = EntityState.Detached;
        }
    }
}