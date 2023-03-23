using System;
using System.Linq;
using System.Runtime.InteropServices;
using FluentAssertions;
using Newtonsoft.Json;
using Npa.Accounting.Infrastructure.Newtonsoft.ContractResolvers;
using Npa.Accounting.Infrastructure.Repay.Responses;
using Npa.Accounting.Tests.Helpers;
using Xunit;
using Xunit.Sdk;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class PaymentResponseTests
{

    [Fact]
    public void PaymentResponseDeserializesCorrectly()
    {
        var de = JsonConvert.DeserializeObject<PaymentResponse>(FileHelper.LoadString("repayResponse200.json"),new JsonSerializerSettings()
        {
            ContractResolver = new SnakeCaseContractResolver()
        });

        de.Should().BeEquivalentTo(Data.RepayResponse200);
    }
}