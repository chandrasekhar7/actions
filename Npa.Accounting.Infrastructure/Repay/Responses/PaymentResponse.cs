using System;
using System.Collections.Generic;
using Npa.Accounting.Infrastructure.Attributes;

namespace Npa.Accounting.Infrastructure.Repay.Responses;

[SnakeCased]
public class PaymentResponse
{
    public string EntryMethod { get; init; }
    public decimal RequestedAuthAmount { get; init; }
    public decimal AuthAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal TipAmount { get; init; }
    public decimal? ConvenienceAmount { get; init; }
    public List<CustomField> CustomFields { get; init; }
    public int MerchantId { get; init; }
    public string MerchantName { get; init; }
    public string CustomerId { get; init; }
    public int? InvoiceId { get; init; }
    public DateTime Date { get; init; }
    public string PaymentTypeId { get; init; }
    public string CardBin { get; init; }
    public string Last4 { get; init; }
    public string NameOnCard { get; init; }
    public string TransTypeId { get; init; }
    public string UserName { get; init; }
    public string LastBatchNumber { get; init; }
    public string? BatchNumber { get; init; }
    public int? BatchId { get; init; }
    public string Result { get; init; }
    public string ResultText { get; init; }
    public ResultDetails ResultDetails { get; init; }
    public bool Voided { get; init; }
    public bool Reversed { get; init; }
    public string HostRefNum { get; init; }
    public string AvsResponse { get; init; }
    public string AvsResponseCode { get; init; }
    public string CvResponse { get; init; }
    public string CvResponseCode { get; init; }
    public string SettleFlag { get; init; }
    public string CardToken { get; init; }
    public int RetryCount { get; init; }
    public string CurrencyId { get; init; }
    public string Street { get; init; }
    public string Zip { get; init; }
    public string ProcessorId { get; init; }
    public string ExpDate { get; init; }
    public string PnRef { get; init; }
    public string AuthCode { get; init; }
    public PaymentMethodDetail PaymentMethodDetail { get; init; }
    public SavedPaymentMethod? SavedPaymentMethod { get; init; }
    public string ReceiptId { get; init; }
}



