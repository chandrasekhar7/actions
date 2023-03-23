using FluentValidation.TestHelper;
using FluentAssertions;
using Xunit;
using Npa.Accounting.Application.Scheduled.Commands;

namespace Npa.Accounting.Tests.Application.Cards.Fixtures;


public class CancelScheduledAchValidatorTests 
{    
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
    public void ThrowsForInvalidAchId(int id){
        var validator = new CancelScheduledAchCommandValidator();
        var command = new CancelScheduledAchCommand(id);
        var result = validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor( x => x.ScheduledAchId);
    }
        [Fact]

    public void PassesValidationWithValidId(){
        var validator = new CancelScheduledAchCommandValidator();
        var command = new CancelScheduledAchCommand(1);
        var result = validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }


}
