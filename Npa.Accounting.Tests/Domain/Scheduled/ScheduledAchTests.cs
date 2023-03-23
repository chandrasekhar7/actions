using System;
using FluentAssertions;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Tests.Helpers;
using Xunit;

namespace Npa.Accounting.Tests.Domain.Transactions;

public class ScheduledAchTests {
  [Fact]
  public void CanceledScheduledAchWorksWhenNoPaymentId()
  {
       // Given
        var ach = TestObjects.MockScheduledAch();
        // When
        ach.CancelAch(new Teller("AAA"));
        // Then

        ach.Cancelled.Should().NotBeNull();
        ach.Cancelled.TimeStamp.Should().BeAfter(DateTime.Now.AddSeconds(-1));
        ach.Cancelled.Teller.Should().BeEquivalentTo(new Teller("AAA"));
  }  

    [Fact]
    public void ShouldThrowWhenAchAlreadyCanceled()
    {
        // Given
        var ach = new ScheduledAch(1, TransactionType.Disburse, TestObjects.MockModifiedBy(), 1.1m, DateOnly.FromDateTime(DateTime.Now), null, new ModifiedBy(DateTime.Now, new Teller("AAB")) );

        // Then
        var ex = Assert.Throws<DomainLayerException>(() => ach.CancelAch(new Teller("aaa")));
        ex.Message.Should().Be("Ach already cancelled");
    }

    [Fact]
    public void CanceledScheduledAchThrowsWhenPaymentIdNotNull()
    {
        // Given
        var ach = new ScheduledAch(1, TransactionType.Disburse, new ModifiedBy(DateTime.Now, new Teller("AAA")), 1.1m, DateOnly.FromDateTime(DateTime.Now), 1);
        
        // Then
        var ex = Assert.Throws<DomainLayerException>(() => ach.CancelAch(new Teller("aaa")));
        ex.Message.Should().Be("Cannot cancel a completed ACH");
    }

}