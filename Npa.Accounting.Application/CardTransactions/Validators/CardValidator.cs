using System;
using FluentValidation;
using Npa.Accounting.Common.DateTimes;
using Npa.Accounting.Common.Validations;

namespace Npa.Accounting.Application.CardTransactions.Validators;

public class CardValidator : AbstractValidator<CardViewModel>
{
    public CardValidator()
    {
        RuleFor(e => e.Number).CreditCard();
        RuleFor(e => e.Cvv).Matches(Pattern.Cvv);
        RuleFor(e => e.Expiration).Matches(Pattern.Expiration)
            .Must(v =>
            {
                var exp = DateTimeHelper.FromMMYY(v).AddMonths(1).AddSeconds(-1);
                return exp > DateTime.Now;
            })
            .WithMessage("Cannot use expired card");

        RuleFor(e => e.FirstName).Length(2, 100);
        RuleFor(e => e.LastName).Length(2, 100);
            
        RuleFor(e => e.Street).Length(3, 100);
        RuleFor(e => e.City).Length(3, 100);
        RuleFor(e => e.State).Length(2);
        RuleFor(e => e.ZipCode).Matches(Pattern.ZipCode);
    }
}