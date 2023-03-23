namespace Npa.Accounting.Application.CardTransactions
{
    public class RepayTokenPaymentRequest
    {
        public string amount { get; init; }
        public string customer_id { get; init; }
        public string card_token { get; init; }
        public string transaction_type { get; init; }
        public string Source { get; init; } = "DirectAPI";
        public string PaymentChannel { get; init; }
        public string ChannelUser { get; init; }
        public string? paytoken { get; set; }
    }
}
