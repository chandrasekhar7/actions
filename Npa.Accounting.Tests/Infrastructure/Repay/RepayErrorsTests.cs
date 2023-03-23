using System.Collections.Generic;
using Npa.Accounting.Infrastructure.Repay;
using Xunit;

namespace Npa.Accounting.Tests.Infrastructure.Repay;

public class RepayErrorsTests
{
    [Fact]
    public void CanBuildStringFromErrors()
    {
        var e = new RepayErrors()
        {
            Errors = new List<ErrorItem>()
            {
                new ErrorItem("x", "y", "z"),
                new ErrorItem("a", "b", "c")
            }
        };
    }
}