using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class SavedPaymentMethod
{
    public string Id { get; set; }
    public string Token { get; set; }
    public bool IsEligibleForDisbursement { get; set; }
}