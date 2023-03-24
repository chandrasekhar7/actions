namespace Npa.Accounting.Infrastructure.Repay
{
    public class RetrievePaytokenTokenized
    {
        public string customer_id { get; set; }
        public string amount { get; set; }
        public string card_token { get; set; }

        // static ones
        public string transaction_type { get; set; }
        public string Source { get; set; }
        public string PaymentChannel { get; set; }
        public string ChannelUser { get; set; }
    }
}