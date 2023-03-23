using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Infrastructure.Abstractions;
using Npa.Accounting.Infrastructure.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Tests.Helpers;
using Npa.Accounting.Tests.Infrastructure.Mocks;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Loans;

public class LoanServiceUnitTests
{
    [Fact]
    public async Task ConvertsResponseProperly()
    {
        var facade = new Mock<IDbFacade>();
        var res = new HttpResponseMessage(HttpStatusCode.OK);
        res.Content = new StringContent(FileHelper.LoadString("loanResponse.json"));
        var mockClient = new MockHttpClientFactory().Create(res);
        var service = new LoanService(mockClient.Client, facade.Object);
        var response = await service.GetLoan(1234);
        response.Balance.Should().Be(400);
        response.Credit.Limit.Should().Be(600);
        response.Credit.Available.Should().Be(100);
        response.PartialPayments.Should().BeTrue();
    }
    
    [Fact]
    public async Task ConvertsPaydayResponseProperly()
    {
        var facade = new Mock<IDbFacade>();
        var res = new HttpResponseMessage(HttpStatusCode.OK);
        res.Content = new StringContent(FileHelper.LoadString("loanResponsePayday.json"));
        var mockClient = new MockHttpClientFactory().Create(res);
        var service = new LoanService(mockClient.Client, facade.Object);
        var response = await service.GetLoan(1234);
        response.Balance.Should().Be(400);
        
        response.PartialPayments.Should().BeFalse();
    }
    
    [Fact]
    public async Task HandlesRequestExceptionCorrectly()
    {
        var facade = new Mock<IDbFacade>();
        // Tests all bad responses
        var res = new HttpResponseMessage(HttpStatusCode.NotFound);
        var mockClient = new MockHttpClientFactory().Create(res);
        var service = new LoanService(mockClient.Client, facade.Object);
        Func<Task> act = async () => await service.GetLoan(1234);
        await act.Should().ThrowAsync<HttpRequestException>();
    }
    
    [Fact]
    public async Task ConvertsStatementResponseProperly()
    {
        var facade = new Mock<IDbFacade>();
        var loanRes = new HttpResponseMessage(HttpStatusCode.OK);
        loanRes.Content = new StringContent(FileHelper.LoadString("loanResponse.json"));
        var stmtRes = new HttpResponseMessage(HttpStatusCode.OK);
        stmtRes.Content = new StringContent(FileHelper.LoadString("statementResponse.json"));
        var mockClient = new MockHttpClientFactory().Create(new List<HttpResponseMessage>()
        {
            loanRes, stmtRes
        });
        var service = new LoanService(mockClient.Client, facade.Object);
        var response = await service.GetStatement(new StatementId(1500000, DateOnly.MinValue));
        response.Balance.Should().Be(200); // Statements change loan balance to validate payment amount appropriately
        response.Credit.Limit.Should().Be(600);
        response.Credit.Available.Should().Be(0); // Statements do not allow draws
        response.PartialPayments.Should().BeTrue();
    }
}