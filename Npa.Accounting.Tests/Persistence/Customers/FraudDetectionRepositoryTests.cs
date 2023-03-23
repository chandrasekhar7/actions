using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Application;
using System.Threading.Tasks;
using Npa.Accounting.Common;
using FluentAssertions;
using System.Threading;
using System.Data;
using System;
using Xunit;
using Moq;

namespace Npa.Accounting.Tests.Persistence.Customers
{
    public class FraudDetectionRepositoryTests
    {

        [Theory]
        [InlineData(false, "Draw Limit Reached - 72", true, "Credit Limit reached", false, true)]
        [InlineData(false, "Draw Limit Reached - 24", true, "Credit Limit reached", false, true)]
        [InlineData(true, "", false, "", false, false)]
        public async Task DrawIsDeniedForFraudWhenAppropriate(bool IsEligible, string LoanLockNote, bool CCBlock, string ErrorMessage, bool RunDeny, bool willThrow)
        {
            // Arrange
            var facadeMock = new Mock<IDbFacade>();
            var fraudDetection = new FraudDetection(123, 456, 789);
            var FraudDetectionResult = new FraudDetectionResult(IsEligible, LoanLockNote, CCBlock, ErrorMessage, RunDeny);
            facadeMock.Setup(
                f => f.ExecSingleProc<FraudDetectionResult>(
                    "dbo.FraudDetection",
                    It.IsAny<FraudDetection>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<CancellationToken>())
                ).ReturnsAsync(FraudDetectionResult);
            var FraudDetectionRepository = new FraudDetectionRepository(facadeMock.Object);

            // Act
            bool? result = null;
            Func<Task> act = async () => result = await FraudDetectionRepository.FraudCheck(fraudDetection);

            // Assert
            if (willThrow)
            {
                await act.Should().ThrowAsync<ApplicationLayerException>();
            }
            else
            {
                await act.Should().NotThrowAsync();
                result.Should().Be(true);
            }
        }

        [Theory]
        [InlineData("ILM", "HoLd-CaLl Or PiCk Up CaRd", 703, true)] // Test a LPP account will find the LPP return message regardless of capitalization
        [InlineData("ABC", "pick up card, special condition (fraud account)", 700, false)] // Test a Paydini Teller attempt does not return fraud
        [InlineData("ILM", "Any other Random Message", 700, false)] // Test Repay account will not return fraud for code not in Repay list 
        [InlineData("ILM", "lost card, pick up (fraud account)", 700, true)] // Test a Repay account will find Repay the return message
        [InlineData("ILM", "Any other Random Message", 704, false)] // Test LPP account will not return fraud for code not in LPP list 
        [InlineData("ILM", "hold-call or pick up card", 700, false)] // Test a Repay account will not find the LPP return message
        [InlineData("ILM", "hold-call or pick up card", 703, true)] // Test a LPP account will find the LPP return message
        public void ReturnCodesIndicateFraud(string tellerName, string returnMessage, int merchantID, bool isFraudulent)
        {
            // Arrange
            var merchant = new Merchant(merchantID);
            var facadeMock = new Mock<IDbFacade>();
            var teller = new Teller(tellerName);

            var transaction = 
                new Transaction(
                    1, 
                    200, 
                    DateTime.Now, 
                    TransactionType.Debit,
                    new Teller(tellerName),
                    new CardTransaction(
                        new CustomerCard(
                            1,
                            1,
                            1,
                            new LastFour("1234"),
                            new Expiration(12, 25)),
                        new Merchant(merchantID), 
                    new ReturnMessage(
                        CardReturnStatus.Deny,
                        "D",
                        returnMessage,
                        "123456"))
                    );

            var FraudDetectionRepository = new FraudDetectionRepository(facadeMock.Object);

            // Act
            bool? result = null;
            result = FraudDetectionRepository.LookupFraudReturnCode(transaction, teller);

            // Assert
            Assert.Equal(isFraudulent, result);
        }
    }
}


