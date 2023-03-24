using System;
using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class CardStoreResponse
{
    public int MerchantId { get; init; }
    public string MerchantName { get; init; }
    public string CustomerId { get; init; }
    public DateTime Date { get; init; }
    public string NameOnCard { get; init; }
    public string UserName { get; init; }
    public ResultDetails ResultDetails { get; init; }
    public string PnRef { get; init; }
    public string ReceiptId { get; init; }
    public SavedPaymentMethod? SavedPaymentMethod { get; init; }
}