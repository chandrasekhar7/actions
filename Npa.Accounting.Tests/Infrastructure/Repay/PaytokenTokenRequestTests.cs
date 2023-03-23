using Newtonsoft.Json;
using Npa.Accounting.Infrastructure.Repay.Paytokens;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class PaytokenTokenRequestTests
{
    /// <summary>
    /// Repay is very particular about the posts to their api and when retreiving the paytoken
    /// we must NOT send anything with a paytoken field
    /// </summary>
    [Fact]
    public void SerializesWithoutPaytokenWhenNull()
    {
        var p = new PaytokenTokenRequest(1, 20.00m,1, TransactionType.Sale);
        var serialized = JsonConvert.SerializeObject(p);
        Assert.False(serialized.Contains("paytoken"));
    }
    
    [Fact]
    public void SerializesWithPaytokenWhenNotNull()
    {
        var p = new PaytokenTokenRequest(1, 20.00m,1, TransactionType.Sale);
        p.SetPayToken(new PaytokenResponse("1234"));
        var serialized = JsonConvert.SerializeObject(p);
        Assert.True(serialized.Contains("paytoken"));
    }
}