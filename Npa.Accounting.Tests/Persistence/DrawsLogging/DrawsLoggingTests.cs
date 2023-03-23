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
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.DbContexts;
using Npa.Accounting.Persistence.DEPRECATED.Transactions;
using Npa.Accounting.Tests.Persistence.Fixtures;
using Xunit;

namespace Npa.Accounting.Tests.Persistence.DrawsLogging;
public class DrawsLoggingTests
{
    private DbContextOptions<TransactionDbContext> options;

    public List<Draw> Draws => new List<Draw>()
    {
        new Draw("init", 1000, 1, 25, "8,8,8,8:8888")
    };

    public DrawsLoggingTests()
    {
        //context = fixture.Context;
        options = new DbContextOptionsBuilder<TransactionDbContext>()
            .UseInMemoryDatabase(databaseName: "temp").Options;
    }

    [Fact]
    public async Task CanAddDrawLogAttempt()
    {
        using var context = new TransactionDbContext(options);
        var repo = new ScheduleTransactionRepository(context);
        context.Database.EnsureDeleted();
        context.Draws.AddRange(Draws);
        context.SaveChanges();

        await repo.CreateDrawLog("test", 123456789, 654321, 100, "10.10.10.10:9000");

        context.Draws.Count().Should().Be(2);
        context.Draws.Last().DrawType.Should().Be("test");
        context.Draws.Last().PowerID.Should().Be(123456789);
        context.Draws.Last().LoanID.Should().Be(654321);
        context.Draws.Last().Amount.Should().Be(100);
        context.Draws.Last().IpAddress.Should().Be("10.10.10.10:9000");
    }
    

}