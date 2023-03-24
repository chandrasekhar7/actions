using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class CardInfo
{
    public string? Country { get; set; }
    public string BankName { get; set; }
    public string Brand { get; set; }
    public string? Category { get; set; }
    public string Type { get; set; }
}