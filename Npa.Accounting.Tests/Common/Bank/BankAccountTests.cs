using System;
using Npa.Accounting.Common.Bank;
using Xunit;

namespace Npa.Accounting.Tests.Common.Bank
{
    public class BankAccountTests
    {
        [Theory]
        [InlineData("12345678", false)]
        [InlineData("", false)]
        [InlineData("123456ARS", false)]
        [InlineData("QWF123456", false)]
        [InlineData("123456123456", false)]
        [InlineData("123456789  ", false)]
        [InlineData("  123456789", false)]
        [InlineData("  1234567", false)]
        [InlineData("123456789", true)]
        public void RoutingNumberMustBeValid(string rn, bool valid)
        {
            var acct = "1234";
            if (valid)
            {
                Assert.Equal(rn, new BankAccount(rn, acct).RoutingNumber);
            }
            else
            {
                var ex = Assert.Throws<FormatException>(() => new BankAccount(rn, acct));
                Assert.Equal("Routing number must be 9 digits", ex.Message);
            }
        }
        
        [Theory]
        [InlineData("", false)]
        [InlineData("123456ARS", false)]
        [InlineData("QWF123456", false)]
        [InlineData("123456123456", true)]
        [InlineData("123456789  ", false)]
        [InlineData("  123456789", false)]
        [InlineData("  1234567", false)]
        [InlineData("123456789", true)]
        public void AccountNumberMustBeValid(string acct, bool valid)
        {
            var rn = "123456789";
            if (valid)
            {
                Assert.Equal(acct, new BankAccount(rn, acct).AccountNumber);
            }
            else
            {
                var ex = Assert.Throws<FormatException>(() => new BankAccount(rn, acct));
                Assert.Equal("Account number must be numeric", ex.Message);
            }
        }
    }
}