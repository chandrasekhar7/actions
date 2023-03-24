using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class PaymentMethodDetail
{
    public string CardBin { get; init; }
    public string CardBrand { get; init; }
    public string GatewayMid { get; init; }
    public string CardLastFour { get; init; }
    public string CardExpiration { get; init; }
    public string CardholderName { get; init; }
}