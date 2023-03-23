using System;
using Npa.Accounting.Common.Cards;
using Xunit;

namespace Npa.Accounting.Tests.Common.Cards
{
    public class ExpirationTests
    {
        [Theory]
        [InlineData("13/31")]
        [InlineData("13/131")]
        [InlineData("113")]
        [InlineData("1/13")]
        public void ExplicitStringCastThrowsIfInvalidFormat(string mmyy)
        {
            Assert.Throws<ArgumentException>(() => (Expiration) mmyy);
        }
        
        [Theory]
        [InlineData("01/31")]
        [InlineData("03/29")]
        [InlineData("01/25")]
        [InlineData("12/23")]
        public void ExplicitStringCast(string mmyy)
        {
            var casted = (Expiration) mmyy;
            Assert.True(true);
        }
        
        [Fact]
        public void ExplicitCastDateTimeToExpiration()
        {
            Expiration casted = (Expiration)new DateTime(2025,12,3);
            Assert.Equal("12/25", casted.ToString());
            Assert.Equal(2025, casted.Year);
            Assert.Equal(12, casted.Month);
        }
        
        [Fact]
        public void ImplicitCastExpirationToDateTime()
        {
            DateTime casted = new Expiration(12,25);
            Assert.Equal("12/25", casted.ToString("MM/yy"));
        }
        
        [Theory]
        [InlineData(13,25)]
        [InlineData(0,25)]
        [InlineData(2,100)]
        [InlineData(3,-1)]
        public void ThrowsIfInvalidCreation(int month, int year)
        {
            Assert.Throws<FormatException>(() => new Expiration(month, year));
        }
    }
}