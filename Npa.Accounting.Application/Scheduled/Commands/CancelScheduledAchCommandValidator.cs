using System;
using FluentValidation;

namespace Npa.Accounting.Application.Scheduled.Commands
{
    public class CancelScheduledAchCommandValidator : AbstractValidator<CancelScheduledAchCommand>
    {
        public CancelScheduledAchCommandValidator()
        {
            RuleFor(e => e.ScheduledAchId).Must(e => e > 0);
        }
    }
}