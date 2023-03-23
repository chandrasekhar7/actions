using System.Threading.Tasks;
using Xunit;
using Moq;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using System.Threading;
using Npa.Accounting.Application.CardTransactions.Commands.AddTokenTransaction;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Cards;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;
using Npa.Accounting.Application.CardTransactions;
using Npa.Accounting.Domain.DEPRECATED.Users;
using Npa.Accounting.Tests.Helpers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using FluentAssertions;
using Npa.Accounting.Persistence.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using System.Data;
using System;
using Npa.Accounting.Application;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using System.Collections.Generic;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Transactions;

using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED;
using System.Collections.Generic;
using Npa.Accounting.Domain.DEPRECATED.Transactions;


namespace Npa.Accounting.Tests.Application.Commands
{
    public class AddTokenTransactionCommandTests
    {
        [Fact]

        public async Task ShouldSendSMSForFailedInstantFunding()
        {
            // Given
            var mockLoanInfo = new LoanInfo(Location.Kansas, new Credit(100.1m, 100.1m), 1.1m, true);
            var mockLoan = new Loan(1, 1, mockLoanInfo, new List<ScheduledAch>() { });
            var mockCustomer = new Customer(1, new CustomerInfo(), new CardStore(1, 1, new List<CustomerCard> { new CustomerCard(1, 2, 1, new Accounting.Common.Cards.LastFour("1234"), new Accounting.Common.Cards.Expiration(01, 2025)) }), mockLoan);
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var mockCardTransService = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var scheduleRepo = new Mock<IScheduleTransactionRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 50, TransactionType = Accounting.Common.Transactions.TransactionType.Disburse };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var returnMessage = new ReturnMessage(Accounting.Common.Cards.CardReturnStatus.Deny, "0", "Test", "0" );
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("AAA", 1, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockCustomer);
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(true);
            mockFacade.Setup(x => x.QueryProcAsync<bool>(It.IsAny<string>(), It.IsAny<object>(), null)).ReturnsAsync(new List<bool> { true, true });
            mockCardTransService.Setup(x => x.Process(It.IsAny<Transaction>(), It.IsAny<CancellationToken>())).ReturnsAsync(returnMessage);


            var addTransactionCommandHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, mockCardTransService.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, scheduleRepo.Object, mockFraudDetection.Object);
            var addTransactionCommand = new AddTokenTransactionCommand(1, testTransaction, "192.1.1.1");
            await addTransactionCommandHandler.Handle(addTransactionCommand);
            // Then
            commService.Verify(x => x.SendInstantFundFailedSMS(1));
        }

        [Fact]
        public async Task ShouldSendSMSForSuccessfulDebit()
        {
            // Given
            var mockLoanInfo = new LoanInfo(Location.Kansas, new Credit(200.0m, 100.1m), 100.1m, true);
            var mockLoan = new Loan(1, 1, mockLoanInfo, new List<ScheduledAch>() { });
            var mockCustomer = new Customer(1, new CustomerInfo(), new CardStore(1, 1, new List<CustomerCard> { new CustomerCard(1, 2, 1, new Accounting.Common.Cards.LastFour("1234"), new Accounting.Common.Cards.Expiration(01, 2025)) }), mockLoan);
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var mockCardTransService = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var scheduleRepo = new Mock<IScheduleTransactionRepository>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 50, TransactionType = Accounting.Common.Transactions.TransactionType.Debit };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var returnMessage = new ReturnMessage(Accounting.Common.Cards.CardReturnStatus.Approve, "0", "Test", "0");
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("AAA", 1, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(mockCustomer);
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(true);
            mockFacade.Setup(x => x.QueryProcAsync<bool>(It.IsAny<string>(), It.IsAny<object>(), null)).ReturnsAsync(new List<bool> { true, true });
            mockCardTransService.Setup(x => x.Process(It.IsAny<Transaction>(), It.IsAny<CancellationToken>())).ReturnsAsync(returnMessage);

            var addTransactionCommandHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, mockCardTransService.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, scheduleRepo.Object, mockFraudDetection.Object);
            var addTransactionCommand = new AddTokenTransactionCommand(1, testTransaction, "192.1.1.1");
            await addTransactionCommandHandler.Handle(addTransactionCommand);

            // Then
            commService.Verify(x => x.SendPaymentConfirmSMS(0));
        }
        
           public async Task ShouldThrowWhenLoanWithCardNotFound()
        {
            // Given
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 100, TransactionType = Accounting.Common.Transactions.TransactionType.Disburse };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();
            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();

            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("AAA", null, null));
            var cancelHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection.Object);
            var cancelCommand = new AddTokenTransactionCommand(1, testTransaction,"1.1.1.1");

            // Then
            await Assert.ThrowsAsync<NotFoundException>(() => cancelHandler.Handle(cancelCommand, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldThrowWhenPowerIdDoesNotMatch()
        {
            // Given
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 100, TransactionType = Accounting.Common.Transactions.TransactionType.Disburse };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();
            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("AAA", null, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockCustomer); var cancelHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection.Object);
            var cancelCommand = new AddTokenTransactionCommand(1, testTransaction,"1.1.1");

            // Then
            await Assert.ThrowsAsync<NotFoundException>(() => cancelHandler.Handle(cancelCommand, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldThrowWhenCannotLockLoan()
        {
            // Given
            var customerRepo = new Mock<ICustomerRepository>();
            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();
            var userServ = new Mock<IUserService>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1595417, Amount = 100, TransactionType = Accounting.Common.Transactions.TransactionType.Disburse };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("ILM", 1, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockCustomer);
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(false);
            var cancelHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection.Object);
            var cancelCommand = new AddTokenTransactionCommand(1, testTransaction,"1.1.1");

            // Then
            var ex = await Assert.ThrowsAsync<Accounting.Application.ApplicationLayerException>(() => cancelHandler.Handle(cancelCommand, CancellationToken.None));
            Assert.Equal("Cannot process transaction at this time", ex.Message);
        }

        [Fact]
        public async Task ShouldThrowWhenDisburseGreaterThanCredit()
        {
            // Given
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 100, TransactionType = Accounting.Common.Transactions.TransactionType.Disburse };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();
            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("AAA", 1, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockCustomer);
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(true);
            var cancelHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection.Object);
            var cancelCommand = new AddTokenTransactionCommand(1, testTransaction,"1.1.1");

            // Then
            var ex = await Assert.ThrowsAsync<Accounting.Application.ApplicationLayerException>(() => cancelHandler.Handle(cancelCommand, CancellationToken.None));
            Assert.Equal("Maximum draw is 1.1", ex.Message);
        }

        [Fact]
        public async Task ShouldThrowWhenRescindTellerIsILM()
        {
            // Given
            var customerRepo = new Mock<ICustomerRepository>();
            var userServ = new Mock<IUserService>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var commService = new Mock<ICommunicationsService>();
            var testTransaction = new NewTransactionViewModel { LoanId = 1, Amount = 100, TransactionType = Accounting.Common.Transactions.TransactionType.Rescind };
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var mockFraudDetection = new Mock<IFraudDetectionRepository>();

            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();
            // When
            userServ.Setup(x => x.GetUser()).Returns(new User("ILM", 1, null));
            customerRepo.Setup(x => x.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockCustomer);
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(true);
            var cancelHandler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection.Object);
            var cancelCommand = new AddTokenTransactionCommand(1, testTransaction,"1.1.1");

            // Then
            var ex = await Assert.ThrowsAsync<Accounting.Application.ApplicationLayerException>(() => cancelHandler.Handle(cancelCommand, CancellationToken.None));
            Assert.Equal("Can only rescind by advocates", ex.Message);
        }

        [Theory]
        [InlineData("ABC", "no credit account", Location.California, true, "DC PAYMENT RESULT", false, "DC PAYMENT RESULT", false, false)]  // Should Not Throw if returnCode is fraud but done in paydini
        [InlineData("ABC", "Approved", Location.California, false, "Draw Limit Reached - 72", true, "Credit Limit reached", false, true)]  // Should Not Throw if 8-Day limit reached but done in paydini
        [InlineData("ILM", "Approved", Location.California, false, "Draw Limit Reached - 72", true, "Credit Limit reached", false, true)]  // Should Throw if 8-Day limit reached
        [InlineData("ILM", "Approved", Location.California, false, "Draw Limit Reached - 24", true, "Credit Limit reached", false, true)]  // Should Throw if 1-Day limit reached
        [InlineData("ILM", "no credit account", Location.California, true, "DC PAYMENT RESULT", false, "DC PAYMENT RESULT", false, true)]  // Should Throw if returnCode is fraud
        [InlineData("ILM", "Approved", Location.California, true, "NA", false, "NA", false, false)]  // Should Not Throw if fraudCheck is good
        public async Task ShouldThrowWhenFraudCheckReturnsFraud(string teller, string returnedMessage, Location location, bool IsEligible, string LoanLockNote, bool CCBlock, string ErrorMessage, bool RunDeny, bool willThrow)
        {
            // Arrange
            var Cards = new List<CustomerCard>() { new CustomerCard(123, 123, 123, new LastFour("1230"), new Expiration(12, DateTime.Now.Year)) };
            var testCustomer = new Customer( 123, new CustomerInfo(), new CardStore(123, 123, Cards), new Loan( 123, 123, new LoanInfo(location, new Credit(1000, 500), 500, true)));
            var testTransaction = new NewTransactionViewModel { LoanId = 123, Amount = 100, TransactionType = TransactionType.Disburse };
            var FraudDetectionResult = new FraudDetectionResult(IsEligible, LoanLockNote, CCBlock, ErrorMessage, RunDeny);
            var returnMessage = new ReturnMessage(CardReturnStatus.Approve, "A", returnedMessage, "123456");
            var testUser = new User(teller, 123);

            var ScheduleTransactionRepo = new Mock<IScheduleTransactionRepository>();
            var ps = new Mock<ICardTransactionServiceDeprecated>();
            var mockFacade = new Mock<ITransactionReadDbFacade>();
            var commService = new Mock<ICommunicationsService>();
            var customerRepo = new Mock<ICustomerRepository>();
            var lockRepo = new Mock<ILoanLockRepository>();
            var userServ = new Mock<IUserService>();
            var facadeMock = new Mock<IDbFacade>();

            facadeMock.Setup(f => f.ExecSingleProc<FraudDetectionResult>("dbo.FraudDetection", It.IsAny<FraudDetection>(), It.IsAny<IDbTransaction>(), It.IsAny<CancellationToken>())).ReturnsAsync(FraudDetectionResult);
            mockFacade.Setup(f => f.QueryProcAsync<bool>("dbo.CheckLeadsInstantFunding", It.IsAny<object>(), It.IsAny<IDbTransaction>())).ReturnsAsync(new List<bool> { true }.AsReadOnly());
            customerRepo.Setup( f => f.GetWithLoanByToken(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(testCustomer);
            ps.Setup(f => f.Process( It.IsAny<Transaction>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(returnMessage));
            lockRepo.Setup(x => x.TryLock(It.IsAny<LoanLock>())).ReturnsAsync(true);
            userServ.Setup(x => x.GetUser()).Returns(testUser);

            var mockFraudDetection = new FraudDetectionRepository(facadeMock.Object);
            var Handler = new AddTokenTransactionCommandHandler(customerRepo.Object, ps.Object, userServ.Object, lockRepo.Object, commService.Object, mockFacade.Object, ScheduleTransactionRepo.Object, mockFraudDetection);
            var Command = new AddTokenTransactionCommand(123, testTransaction, "1.1.1");

            // Act
            CardTransactionViewModel result;
            Func<Task> act = async () => result = await Handler.Handle(Command, CancellationToken.None);

            // Assert
            if (willThrow)
            {
                await act.Should().ThrowAsync<ApplicationLayerException>().WithMessage(ErrorMessage);
            }
            else
            {
                await act.Should().NotThrowAsync();
            }
        }
    }
}


     


