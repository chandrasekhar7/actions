using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Infrastructure.Abstractions;
using Npa.Accounting.Infrastructure.Crypto;
using Npa.Accounting.Infrastructure.Lpp;
using Npa.Accounting.Tests.Helpers;
using Npa.Accounting.Tests.Infrastructure.Mocks;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Lpp;

public class LppServiceTests
{
    private readonly IOptions<LppOptions> options = new OptionsWrapper<LppOptions>(new LppOptions()
    {
        Credentials = new LppConfig[]
        {
            new LppConfig()
            {
                Merchant = 702,
                Disbursement = "15f75cb0-f2bf-416a-8f3c-8a466c31d185",
                Debit = "15f75cb0-f2bf-416a-8f3c-8a466c31d185"
            }
        }
    });

    [Fact]
    public async void CanDisburse()
    {
        var crypto = new Mock<ICryptoRepository>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponse.Content = new StringContent(FileHelper.LoadString("lppDisburse200.json"));
        var httpMock = new MockHttpClientFactory().Create(httpResponse);
        
        var cardTransaction = new CardTransaction(new CustomerCard(796829335, 0, 703126765, new LastFour("*1234"),
            new Expiration(12, DateTime.Now.Year + 2), false), new Merchant(702));
        
        crypto.Setup(c => c.GetAsync(It.IsAny<int>(), It.IsAny<Teller>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new CryptoCard()
               {
                   Address1 = "1234 Nowhere Ln.",
                   CardNumber = "4111111111111111",
                   Cvv = "123",
                   ExpirationDate = "12/25",
                   City = "Somewhere",
                   State = "KS",
                   FirstName = "Test",
                   LastName = "Tester",
                   Zip = "12345"
               });
      
        var s = new LppService(crypto.Object, httpMock.Client, options);
        var res = await s.DisburseAsync(new Transaction(1, 10, DateTime.Now, TransactionType.Disburse,
            new Teller("NMR"),cardTransaction));
       
        res.Status.Should().Be(CardReturnStatus.Approve);
        res.Message.Should().Be("Transaction Approved.");
        res.RefNum.Should().Be("01a48459-55cd-48cb-a81c-d9461c1353b8");
        res.Code.Should().Be("Approve");
    }

    [Fact]
    public async void CanDebit()
    {
        var crypto = new Mock<ICryptoRepository>();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        httpResponse.Content = new StringContent(FileHelper.LoadString("lppPayment200.json"));
        var httpMock = new MockHttpClientFactory().Create(httpResponse);
        var cardTransaction = new CardTransaction(new CustomerCard(796829335, 0, 703126765, new LastFour("*1234"),
            new Expiration(12, DateTime.Now.Year + 2), false), new Merchant(702));
        
        crypto.Setup(c => c.GetAsync(It.IsAny<int>(), It.IsAny<Teller>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CryptoCard()
            {
                Address1 = "1234 Nowhere Ln.",
                CardNumber = "4111111111111111",
                Cvv = "123",
                ExpirationDate = "12/25",
                City = "Somewhere",
                State = "KS",
                FirstName = "Test",
                LastName = "Tester",
                Zip = "12345"
            });
        
        var s = new LppService(crypto.Object, httpMock.Client, options);
        var res = await s.DebitAsync(new Transaction(1, 10, DateTime.Now, TransactionType.Debit, 
            new Teller("EC1"),cardTransaction
        ));
        res.Status.Should().Be(CardReturnStatus.Approve);
        res.Message.Should().Be("Transaction Approved.");
        res.RefNum.Should().Be("01a48459-55cd-48cb-a81c-d9461c1353b8");
        res.Code.Should().Be("Approve");
    }
}