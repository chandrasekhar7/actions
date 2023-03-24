using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class CustomField
{
    public int CustomFieldId { get; init; }
    public string CustomFieldName { get; init; }
    public string Description { get; init; }
    public int MerchantId { get; init; }
    public string FieldValue { get; init; }
    public int TransactionId { get; init; }
    public bool DisplayOnReceipt { get; init; }
}