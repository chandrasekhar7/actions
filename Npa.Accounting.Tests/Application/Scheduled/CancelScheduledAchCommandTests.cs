using System.Threading;
using System.Threading.Tasks;
using Moq;
using Npa.Accounting.Application.Scheduled.Commands;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Domain.DEPRECATED.Users;
using Npa.Accounting.Tests.Helpers;
using Xunit;

namespace Npa.Accounting.Tests.Application.Cards.Fixtures;


public class CancelScheduledAchTests 
{    


    [Fact]
    public async Task ShouldThrowWhenUserInvalid()
    {
        // Given
        var customerRepo = new Mock<ICustomerRepository>();
        var userServ = new Mock<IUserService>();
        var transactionRepo = new Mock<IScheduleTransactionRepository>();

        // When
        var cancelHandler = new CancelScheduledAchCommandHandler(customerRepo.Object, userServ.Object, transactionRepo.Object);
        var cancelCommand = new CancelScheduledAchCommand(2);

        // Then
        await Assert.ThrowsAsync<ForbiddenException>( () => cancelHandler.Handle(cancelCommand, CancellationToken.None));
    }

    [Fact]
    public async Task ShouldThrowWhenCannotFindCustomer()
    {
        // Given
        var customerRepo = new Mock<ICustomerRepository>();
        var userServ = new Mock<IUserService>();
        var transactionRepo = new Mock<IScheduleTransactionRepository>();

        transactionRepo.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockScheduledAch);
        userServ.Setup(x => x.GetUser()).Returns(new User("AAA", null, null));
        // When
        var cancelHandler = new CancelScheduledAchCommandHandler(customerRepo.Object, userServ.Object, transactionRepo.Object);
        var cancelCommand = new CancelScheduledAchCommand(2);
        // Then
        await Assert.ThrowsAsync<NotFoundException>( () => cancelHandler.Handle(cancelCommand, CancellationToken.None));
    }

    [Fact]
    public async Task ShouldCancelWhenNoError()
    {
        // Given
        var customerRepo = new Mock<ICustomerRepository>();
        var userServ = new Mock<IUserService>();
        var transactionRepo = new Mock<IScheduleTransactionRepository>();

        userServ.Setup(x => x.GetUser()).Returns(new User("AAA", null, null));
        customerRepo.Setup(x => x.GetWithLoan(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockCustomer);
        transactionRepo.Setup(x => x.GetById(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestObjects.MockScheduledAch);
        // When
        var cancelHandler = new CancelScheduledAchCommandHandler(customerRepo.Object, userServ.Object, transactionRepo.Object);
        var cancelCommand = new CancelScheduledAchCommand(0);
        await cancelHandler.Handle(cancelCommand);
        // Then
        customerRepo.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>(), It.IsAny<TransactionType?>(), It.IsAny<int?>()));
    }

}