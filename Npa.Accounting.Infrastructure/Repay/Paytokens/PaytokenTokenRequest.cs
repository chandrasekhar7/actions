using System;
using Newtonsoft.Json;

namespace Npa.Accounting.Infrastructure.Repay.Paytokens;

public class PaytokenTokenRequest : BasePaytoken
{
    [JsonProperty(PropertyName = "customer_id")]
    public string CustomerId { get; }
    
    [JsonProperty(PropertyName = "amount")]
    public decimal Amount { get; }

    [JsonProperty(PropertyName = "transaction_type")]
    public string TransactionType { get; }
    
    [JsonProperty(PropertyName = "card_token")]
    public int CardToken { get; }

    // static ones

    [JsonProperty(PropertyName = "Source")]
    public string Source => RepayConfig.Source;

    [JsonProperty(PropertyName = "PaymentChannel")]
    public string PaymentChannel => RepayConfig.PaymentChannel;

    [JsonProperty(PropertyName = "ChannelUser")]
    public string ChannelUser => RepayConfig.ChannelUser;
    
    [JsonProperty(PropertyName = "paytoken", NullValueHandling = NullValueHandling.Ignore)]
    public string? PayToken { get; private set; }

    public PaytokenTokenRequest(int customerId, decimal amount, int cardToken, TransactionType transactionType)
    {
        CustomerId = customerId.ToString();
        Amount = amount;
        CardToken = cardToken;
        TransactionType = transactionType switch
        {
            Paytokens.TransactionType.Auth => "auth",
            Paytokens.TransactionType.Sale => "sale",
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
        CheckoutFormKey = transactionType switch
        {
            Paytokens.TransactionType.Sale => CheckoutFormKey.TokenPayment,
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
    }

    public override void SetPayToken(PaytokenResponse r)
    {
        PayToken = r.Paytoken;
    }
}