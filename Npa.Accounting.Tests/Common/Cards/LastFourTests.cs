using System;
using Npa.Accounting.Common.Cards;
using Xunit;

namespace Npa.Accounting.Tests.Common.Cards
{
    public class LastFourTests
    {
        [Theory]
        [InlineData("B4567")]
        [InlineData("A567")]
        [InlineData("156A")]
        [InlineData("1564A")]
        public void ThrowsWhenNonNumeric(string lastFour)
        {
            Assert.Throws<FormatException>(() => new LastFour(lastFour));
        }
        
        [Theory]
        [InlineData("567")]
        [InlineData("56 7")]
        [InlineData("456567")]
        [InlineData("1")]
        public void ThrowsWhenNotFourDigits(string lastFour)
        {
            Assert.Throws<FormatException>(() => new LastFour(lastFour));
        }
        
        [Theory]
        [InlineData("   4567")]
        [InlineData("4567    ")]
        [InlineData("   4567   ")]
        public void WeAreSuperNiceAndTrimStrings(string lastFour)
        {
            Assert.Equal("4567", new LastFour(lastFour).ToString());
        }
        
        [Theory]
        [InlineData("   *4567")]
        [InlineData("4567")]
        public void AcceptsValidInput(string lastFour)
        {
            Assert.Equal("4567", new LastFour(lastFour).ToString());
        }
    }
}