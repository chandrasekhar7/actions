using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class ResultDetails
{
    public bool AuthorizationReversed { get; set; }
    public bool DelayedInReporting { get; set; }
    public CardInfo CardInfo { get; set; }
}