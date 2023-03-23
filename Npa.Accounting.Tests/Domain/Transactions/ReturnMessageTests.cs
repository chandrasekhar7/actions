using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Transactions;

public class ReturnMessageTests
{

    [Fact]
    public void ReturnMessageToString()
    {
        var message = new ReturnMessage(CardReturnStatus.Approve, "CODE", "MSG", "1234");
        Assert.Equal("Approve: MSG (CODE), RefNum: 1234", message.ToString());
    }
}