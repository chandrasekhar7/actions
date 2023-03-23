using System;
using FluentAssertions;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Loans;

public class StatementIdTests
{
    [Theory]
    [InlineData("1-20220601", 1, 2022,6,1)]
    [InlineData("1-20221231", 1, 2022,12,31)]
    public void ConstructsCorrectly(string stmtId, int loanId, int year, int month, int day)
    {
        var id = new StatementId(stmtId);
        id.StatementDate.Year.Should().Be(year);
        id.StatementDate.Month.Should().Be(month);
        id.StatementDate.Day.Should().Be(day);
        id.LoanId.Should().Be(loanId);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("a-20220101")]
    [InlineData("4-12212022")]
    public void ThrowsOnInvalidString(string stmtId)
    {
        var ex = Assert.Throws<ArgumentException>(() => new StatementId(stmtId));
        ex.Message.Should().Be("Invalid statement Id");
    }
}