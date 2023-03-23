using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Npa.Accounting.Common.Addresses;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.People;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Infrastructure.Repay;
using Npa.Accounting.Tests.Helpers;
using Npa.Accounting.Tests.Infrastructure.Mocks;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class RepayServiceTests
{
    private readonly MockHttpClientFactory factory = new MockHttpClientFactory();

    private readonly IOptions<RepayOptions> options = Options.Create(new RepayOptions()
    {
        Uri = "http://repay.test.testing",
        Auth = "authTest",
        ChannelUser = "ChannelUser",
        PaymentChannel = "PaymentChannel",
        CheckoutForm = new CheckoutForm()
        {
            CardAuth = "A",
            TokenPayment = "T"
        }
    });

    [Fact]
    public async Task Repay400ThrowsWithMessage()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(FileHelper.LoadString("repayStoreCard400.json"))
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        var message = await Assert.ThrowsAsync<HttpRequestException>(() => repay.AddCardAsync(1,
            new Card(new CardNumber("4111111111111111"), new Name("Test", "Person"),
                new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
                new Expiration(12, 2025))));

        var expected = string.Join(Environment.NewLine, new string[]
        {
            "Error (body) - card_cvc: Invalid CVV Number.",
            "Error (body) - card_expiration: Expiration must be in the format MMYY"
        });
        Assert.Equal(expected, message.Message);
    }

    [Fact]
    public async Task Empty500ThrowsWithMessage()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        var message = await Assert.ThrowsAsync<HttpRequestException>(() => repay.AddCardAsync(1,
            new Card(new CardNumber("4111111111111111"), new Name("Test", "Person"),
                new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
                new Expiration(12, 2025))));

        var expected = "An unknown error occured";
        Assert.Equal(expected, message.Message);
    }

    [Fact]
    public async Task Repay500ThrowsWithMessage()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("An error occured")
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        var message = await Assert.ThrowsAsync<HttpRequestException>(() => repay.AddCardAsync(1,
            new Card(new CardNumber("4111111111111111"), new Name("Test", "Person"),
                new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
                new Expiration(12, 2025))));

        var expected = "An error occured";
        Assert.Equal(expected, message.Message);
    }

    [Fact]
    public async Task AddCardReturnsPaymentResults()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(FileHelper.LoadString("repayResponse200.json"))
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        var reponse = await repay.AddCardAsync(1, new Card(new CardNumber("4111111111111111"),
            new Name("Test", "Person"),
            new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
            new Expiration(12, 2025)));

        reponse.Should().BeEquivalentTo(Data.RepayResponse200);
    }

    [Fact]
    public async Task RunTokenPaymentReturnsPaymentResults()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(FileHelper.LoadString("repayResponse200.json"))
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        var reponse =
            await repay.TokenPaymentAsync(
                new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 2025), false), 20m);

        reponse.Should().BeEquivalentTo(Data.RepayResponse200);
    }

    [Fact]
    public async Task RepayHeaderContainsToken()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(FileHelper.LoadString("repayResponse200.json"))
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        await repay.AddCardAsync(1, new Card(new CardNumber("4111111111111111"),
            new Name("Test", "Person"),
            new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
            new Expiration(12, 2025)));

        var paytokenRequest = (mockHttp.Mock.Invocations[0].Arguments[0] as HttpRequestMessage);
        paytokenRequest.Headers.Authorization.ToString().Should().Be($"apptoken {options.Value.Auth}");
    }

    [Fact]
    public async Task RepayAddCardRequestCorrect()
    {
        var mockHttp = factory.Create(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(FileHelper.LoadString("repayResponse200.json"))
        });
        var repay = new RepayService(mockHttp.Client, new LegacyRepayService(mockHttp.Client),
            options);

        await repay.AddCardAsync(1, new Card(new CardNumber("4111111111111111"),
            new Name("Test", "Person"),
            new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
            new Expiration(12, 2025)));

        // Serialization tests cover serialization values so we will dont need to check the content here
        var paytokenRequest = (mockHttp.Mock.Invocations[0].Arguments[0] as HttpRequestMessage);
        paytokenRequest.RequestUri.Should()
            .Be($"{options.Value.Uri}/checkout-forms/{options.Value.CheckoutForm.CardAuth}/paytoken");
        paytokenRequest.Content.Headers.ContentType.MediaType.Should().Be("application/json");

        var addCardRequest = (mockHttp.Mock.Invocations[1].Arguments[0] as HttpRequestMessage);
        addCardRequest.RequestUri.Should()
            .Be($"{options.Value.Uri}/checkout-forms/{options.Value.CheckoutForm.CardAuth}/token-payment");
        addCardRequest.Content.Headers.ContentType.MediaType.Should().Be("application/json");
    }
}