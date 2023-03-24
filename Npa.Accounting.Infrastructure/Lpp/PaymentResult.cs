namespace Npa.Accounting.Infrastructure.Lpp;

public class PaymentResult : PaymentMessage
{
    /// <summary>
    /// The authorization code created when an authorization is placed on a payment method.
    /// </summary>
    public string AuthCode { get; set; }

    /// <summary>
    /// The unique identifier for the current transaction.
    /// </summary>
    public string TransactionId { get; set; }
    
    public string RefNum { get; set; }
}