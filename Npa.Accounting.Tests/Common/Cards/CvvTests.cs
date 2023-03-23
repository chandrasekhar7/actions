using System;
using Npa.Accounting.Common.Cards;
using Xunit;

namespace Npa.Accounting.Tests.Common.Cards
{
    public class CvvTests
    {
        [Theory]
        [InlineData("  123")]
        [InlineData("  123     ")]
        [InlineData("123     ")]
        public void WeAreSuperNiceAndTrimStrings(string number)
        {
            Assert.Equal(number.Trim(), new Cvv(number).ToString());
        }
        
        [Theory]
        [InlineData("12A")]
        [InlineData("A23")]
        [InlineData("A123")]
        public void OnlyDigitsAreAllowed(string number)
        {
            Assert.Throws<FormatException>(() => new Cvv(number));
        }
        
        [Theory]
        [InlineData("12 3")]
        [InlineData("1 2")]
        public void ThrowsWhenNot3Digits(string number)
        {
            Assert.Throws<FormatException>(() => new Cvv(number));
        }

        [Fact]
        public void AcceptsProperNumber()
        {
            var number = "123";
            Assert.Equal(number, new Cvv(number).ToString());
        }
    }
}