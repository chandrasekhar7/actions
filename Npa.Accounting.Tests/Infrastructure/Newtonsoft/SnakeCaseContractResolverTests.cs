using System;
using System.Text.Json.Nodes;
using FluentAssertions;
using Newtonsoft.Json;
using Npa.Accounting.Infrastructure.Attributes;
using Npa.Accounting.Infrastructure.Newtonsoft.ContractResolvers;
using Npa.Accounting.Infrastructure.Repay;
using Npa.Accounting.Infrastructure.Repay.Responses;
using Npa.Accounting.Tests.Helpers;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Newtonsoft;

public class SnakeCaseContractResolverTests
{
    private const string json = "{\"snake_case\":\"string\", \"camelCase\":\"1\", \"PascelCase\":\"1.239\"}";

    [SnakeCased]
    private class MyClass
    {
        public string SnakeCase { get; set; }
        public int CamelCase { get; set; }
        public decimal PascelCase { get; set; }
    }

    [Fact]
    public void CanConvertFromSnakeCase()
    {
        var o = JsonConvert.DeserializeObject<MyClass>(json, new JsonSerializerSettings()
        {
            ContractResolver = new SnakeCaseContractResolver()
        });
        
        o.SnakeCase.Should().Be("string");
        o.CamelCase.Should().Be(1);
        o.PascelCase.Should().Be(1.239m);
    }

    [Fact]
    public void CanConvertRepayCardStoreResponse200()
    {
        var o = JsonConvert.DeserializeObject<CardStoreResponse>(FileHelper.LoadString("repayResponse200.json"), new JsonSerializerSettings()
        {
            ContractResolver = new SnakeCaseContractResolver()
        });

        o.CustomerId.Should().Be("123456");
        o.Date.ToLocalTime().Should().Be(DateTime.Parse("2022-09-27T16:41:49.8318408Z"));
        o.MerchantId.Should().Be(536875818);
        o.MerchantName.Should().Be("Net Pay Advance Channels test 1");
        o.NameOnCard.Should().Be("Test Name");
        o.PnRef.Should().Be("565135833");
        o.ReceiptId.Should()
            .Be(
                "565135833.FhS5Tg.12A72aEm3_FmU7fuBbkx9ojFEhYvBHIzkD-3o9-3d3eQmTJ4SELp9OKmX0mHXPuPYUBJw8ondSpHAByTXWA-Kg");
        o.UserName.Should().Be("ebpp_api_ntpyadv1");
        
        o.ResultDetails.AuthorizationReversed.Should().BeFalse();
        o.ResultDetails.DelayedInReporting.Should().BeFalse();
        o.ResultDetails.CardInfo.Brand.Should().Be("MASTERCARD");
        o.ResultDetails.CardInfo.BankName.Should().Be("FISERV SOLUTIONS INC.");
        o.ResultDetails.CardInfo.Category.Should().BeNull();
        o.ResultDetails.CardInfo.Country.Should().BeNull();
        o.ResultDetails.CardInfo.Type.Should().Be("DEBIT");
        o.SavedPaymentMethod.Id.Should().Be("9d6e15af-ca55-4402-a303-3297d974eccd");
        o.SavedPaymentMethod.Token.Should().Be("560132784");
        o.SavedPaymentMethod.IsEligibleForDisbursement.Should().BeFalse();
    }

    [Fact]
    public void CanConvertRepayCardStoreResponse400()
    {
        var o = JsonConvert.DeserializeObject<RepayErrors>(FileHelper.LoadString("repayStoreCard400.json"), new JsonSerializerSettings()
        {
            ContractResolver = new SnakeCaseContractResolver()
        });

        Assert.Equal(2, o.Errors.Count);
        o.Errors[0].Should().Be(new ErrorItem("body", "card_cvc", "Invalid CVV Number."));

    }
}