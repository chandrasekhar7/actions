using System;
using Npa.Accounting.Common.Cards;
using Xunit;

namespace Npa.Accounting.Tests.Common.Cards
{
    public class CardNumberTests
    {
        [Theory]
        [InlineData("  1234123412341234")]
        [InlineData("  1234123412341234     ")]
        [InlineData("1234123412341234     ")]
        public void WeAreSuperNiceAndTrimStrings(string number)
        {
            Assert.Equal(number.Trim(), new CardNumber(number).ToString());
        }
        
        [Theory]
        [InlineData("12341A3412341234")]
        [InlineData("A234123412341234")]
        [InlineData("123412341234123A")]
        public void OnlyDigitsAreAllowed(string number)
        {
            Assert.Throws<FormatException>(() => new CardNumber(number));
        }
        
        [Theory]
        [InlineData("12341234123412")]
        [InlineData("1234123 412341234")]
        [InlineData("1234123 41234123")]
        public void ThrowsWhenNot16Digits(string number)
        {
            Assert.Throws<FormatException>(() => new CardNumber(number));
        }

        [Fact]
        public void AcceptsProperNumber()
        {
            var number = "1234123412341234";
            Assert.Equal(number, new CardNumber(number).ToString());
        }
    }
}