using System;
using Newtonsoft.Json;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Infrastructure.Repay.Paytokens;

public class PaytokenCardRequest : BasePaytoken
{
    private readonly Card card;
    [JsonProperty(PropertyName = "customer_id")]
    public string CustomerId { get; }
    
    [JsonProperty(PropertyName = "amount")]
    public decimal Amount { get; }

    [JsonProperty(PropertyName = "cardholder_name")]
    public string CardholderName => card.Name;
    
    [JsonProperty(PropertyName = "save_payment_method")]
    public bool Save { get; }

    [JsonProperty(PropertyName = "card_number")]
    public string CardNumber => card.Number;

    [JsonProperty(PropertyName = "card_cvc")]
    public string Cvc => card.Cvv;

    [JsonProperty(PropertyName = "card_expiration")]
    public string Expiration => card.Expiration.ToString().Replace("/", "");
    
    [JsonProperty(PropertyName = "transaction_type")]
    public string TransactionType { get; }

    // static ones

    [JsonProperty(PropertyName = "Source")]
    public string Source => RepayConfig.Source;

    [JsonProperty(PropertyName = "PaymentChannel")]
    public string PaymentChannel => RepayConfig.PaymentChannel;

    [JsonProperty(PropertyName = "ChannelUser")]
    public string ChannelUser => RepayConfig.ChannelUser;
    
    [JsonProperty(PropertyName = "paytoken", NullValueHandling = NullValueHandling.Ignore)]
    public string? PayToken { get; private set; }

    public PaytokenCardRequest(int customerId, decimal amount, Card card, bool save, TransactionType transactionType)
    {
        CustomerId = customerId.ToString();
        Amount = amount;
        Save = save;
        this.card = card ?? throw new ArgumentNullException(nameof(card));
        TransactionType = transactionType switch
        {
            Paytokens.TransactionType.Auth => "auth",
            Paytokens.TransactionType.Sale => "sale",
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
        CheckoutFormKey = transactionType switch
        {
            Paytokens.TransactionType.Auth => CheckoutFormKey.CardAuth,
            Paytokens.TransactionType.Sale => CheckoutFormKey.CardPayment,
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
    }

    public override void SetPayToken(PaytokenResponse r)
    {
        PayToken = r.Paytoken;
    }
}