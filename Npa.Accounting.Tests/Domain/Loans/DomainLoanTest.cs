using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using System;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Xunit;
using Npa.Accounting.Domain.DEPRECATED;
using System.Collections.Generic;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Tests.Helpers;

namespace Npa.Accounting.Tests.Domain.Loans
{
    public class DomainLoanTest
    {
        private Teller testTeller = new Teller("ABC");
        private CustomerCard testCustomerCard = new CustomerCard(123, 123456, 1234, new LastFour("0123"), new Expiration(01, 2005));

        [Theory]
        [InlineData(Location.California, 500, 500, 0, false)]
        [InlineData(Location.Kansas, 250, 0, 250, false)]
        [InlineData(Location.Texas, 750, 750, 0, false)]
        [InlineData(Location.California, 1500, 500, 1000, true)]
        [InlineData(Location.Kansas, 1000, 250, 750, true)]
        [InlineData(Location.Texas, 500, 100, 400, true)]
        public void CanAddLoanInfo_InputLoanInfo_OutputLoanWithLoanInfo(Location Location, decimal CreditLimit, decimal CreditAvalible, decimal Balance, bool PartialPayments)
        {
            // Arrange
            Credit loanInfoCredit = new Credit(CreditLimit, CreditAvalible);
            LoanInfo infoToAdd = new LoanInfo(Location, loanInfoCredit, Balance, PartialPayments);
            Loan loan = new Loan(11111111, 222222222, null);

            // Act
            loan.AddLoanInfo(infoToAdd);

            // Assert
            Assert.Equal(Balance, loan.LoanInfo.Balance);
            Assert.Equal(CreditLimit, loan.LoanInfo.Credit.Limit);
            Assert.Equal(CreditAvalible, loan.LoanInfo.Credit.Available);
            Assert.Equal(PartialPayments, loan.LoanInfo.PartialPayments);
        }


        [Fact]
        public void CannotAddLoanInfoIfAlreadyHasInfo_InputLoanInfo_OutputError()
        {
            // Arrange
            Credit loanInfoCreditOrig = new Credit(500, 250);
            LoanInfo infoOrig = new LoanInfo(Location.California, loanInfoCreditOrig, 250, false);

            Credit loanInfoCreditNew = new Credit(1500, 500);
            LoanInfo infoToAdd = new LoanInfo(Location.Kansas, loanInfoCreditNew, 500, true);

            Loan loan = new Loan(11111111, 222222222, infoOrig);

            // Act and Assert
            var exception = Assert.Throws<DomainLayerException>(() => loan.AddLoanInfo(infoToAdd));
            Assert.Equal("Loan Info is already set", exception.Message);
        }


        [Fact]
        public void CanAddTransaction_InputTypeAmountTellerCard_OutputLoanWithTransaction()
        {
            // Arrange
            Credit loanInfoCreditOrig = new Credit(250, 250);
            LoanInfo infoOrig = new LoanInfo(Location.California, loanInfoCreditOrig, 250, false);
            Loan loan = new Loan(11111111, 222222222, infoOrig);

            // Act
            loan.AddTransaction(TransactionType.Debit, 250, testTeller, testCustomerCard);

            // Assert
            Assert.NotEmpty(loan.Transactions);
        }



        [Theory]
        [InlineData(500, 500, Location.California, 500, false, TransactionType.Debit, 500, false, "InvalidOperationException")]  // Tests that error is thrown if Loan has no info
        [InlineData(500, 500, Location.Kansas, 500, true, TransactionType.Debit, -500, true, "InvalidOperationException")]  // Tests that error is thrown if amount is below zero
        [InlineData(500, 500, Location.Texas, 500, false, TransactionType.Disburse, 1500, true, "DomainLayerException")]  // Tests that error is thrown if amount is more than availible credit
        [InlineData(500, 250, Location.Kansas, 250, true, TransactionType.Debit, 500, true, "DomainLayerException")]  // Tests that error is thrown if amount is greater than balance
        [InlineData(500, 250, Location.California, 250, false, TransactionType.Debit, 500, true, "DomainLayerException")]  // Tests that error is thrown if partial payment is attempted by not allowed
        public void CannotAddInvalidTransactionsForLoanType_InputTypeAmountTellerCard_OutputError(
            decimal creditLimit,
            decimal creditAvailable,
            Location location,
            decimal balance,
            bool partialPayments,
            TransactionType transType,
            decimal transAmount,
            bool hasInfo,
            string exceptionType)
        {
            // Arrange
            Credit loanInfoCreditOrig = new Credit(creditLimit, creditAvailable);
            LoanInfo infoOrig = new LoanInfo(location, loanInfoCreditOrig, balance, partialPayments);
            Loan loan = new Loan(11111111, 222222222, hasInfo ? infoOrig : null);

            // Act and Assert
            if (exceptionType == "InvalidOperationException")
            {
                var exception = Assert.Throws<InvalidOperationException>(() => loan.AddTransaction(transType, transAmount, testTeller, testCustomerCard));
                Assert.NotNull(exception);
            }
            else
            {
                var exception = Assert.Throws<DomainLayerException>(() => loan.AddTransaction(transType, transAmount, testTeller, testCustomerCard));
                Assert.NotNull(exception);
            }
        }

        [Fact]
        public void CanAddScheduled_InputTypeAmountTellerCard_OutputLoanWithScheduled()
        {
            // Arrange
            Credit loanInfoCreditOrig = new Credit(500, 250);
            LoanInfo infoOrig = new LoanInfo(Location.Kansas, loanInfoCreditOrig, 250, false);
            Loan loan = new Loan(11111111, 222222222, infoOrig);

            // Act
            loan.AddScheduledAch(TransactionType.Disburse, 250, testTeller);

            // Assert
            Assert.NotEmpty(loan.ScheduledAch);

        }

        [Fact]
        public void CannotAddScheduledIfTypeIsNotDisburse_InputTypeAmountTellerCard_OutputError()
        {
            // Arrange
            Credit loanInfoCreditOrig = new Credit(500, 250);
            LoanInfo infoOrig = new LoanInfo(Location.Kansas, loanInfoCreditOrig, 250, false);
            Loan loan = new Loan(11111111, 222222222, infoOrig);

            // Act and Assert
            var exception = Assert.Throws<NotImplementedException>(() => loan.AddScheduledAch(TransactionType.Debit, 250, testTeller));
            Assert.NotNull(exception);
        }

        [Fact]
        public void CanceledAchThrowsWhenAchNotFound()
        {
            // Given
            var scheduledAchList = new List<ScheduledAch>(){
                TestObjects.MockScheduledAch(),
            };

            Credit loanInfoCreditOrig = new Credit(500, 250);
            LoanInfo infoOrig = new LoanInfo(Location.Kansas, loanInfoCreditOrig, 250, false);
            Loan loan = new Loan(11111111, 222222222, infoOrig, scheduledAchList);
            // When
            // Then
            var ex = Assert.Throws<DomainLayerException>(()=> loan.CancelScheduledAch(2, new Teller("AAA")));
            Assert.Equal("Cannot find ACH to cancel with id: 2", ex.Message);
            
        }
        
    }
}
