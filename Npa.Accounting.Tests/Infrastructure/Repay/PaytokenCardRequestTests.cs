
using Newtonsoft.Json;
using Npa.Accounting.Common.Addresses;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.People;
using Npa.Accounting.Infrastructure.Repay.Paytokens;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class PaytokenCardRequestTests
{
    /// <summary>
    /// Repay is very particular about the posts to their api and when retreiving the paytoken
    /// we must NOT send anything with a paytoken field
    /// </summary>
    [Fact]
    public void SerializesWithoutPaytokenWhenNull()
    {
        var p = new PaytokenCardRequest(1, 20.00m,
            new Card(new CardNumber("4111111111111111"), new Name("Test", "Person"),
                new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
                new Expiration(12, 2025)), true, TransactionType.Auth);
        var serialized = JsonConvert.SerializeObject(p);
        Assert.False(serialized.Contains("paytoken"));
    }
    
    [Fact]
    public void SerializesWithPaytokenWhenNotNull()
    {
        var p = new PaytokenCardRequest(1, 20.00m,
            new Card(new CardNumber("4111111111111111"), new Name("Test", "Person"),
                new Address("arst", "arst", new State("KS"), new ZipCode("12345")), new Cvv("123"),
                new Expiration(12, 2025)), true, TransactionType.Auth);
        p.SetPayToken(new PaytokenResponse("1234"));
        var serialized = JsonConvert.SerializeObject(p);
        Assert.True(serialized.Contains("paytoken"));
    }
}