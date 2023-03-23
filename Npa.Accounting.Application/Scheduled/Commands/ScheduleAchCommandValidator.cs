using System;
using FluentValidation;

namespace Npa.Accounting.Application.Scheduled.Commands
{
    public class ScheduleAchCommandValidator : AbstractValidator<ScheduleAchCommand>
    {
        public ScheduleAchCommandValidator()
        {
            RuleFor(e => e.LoanId).Must(e => e > 0);
            RuleFor(e => e.Amount).GreaterThan(0).Must(e => Math.Round(e, 2) == e);
        }
    }
}