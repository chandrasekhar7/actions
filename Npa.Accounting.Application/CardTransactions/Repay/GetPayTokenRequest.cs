namespace Npa.Accounting.Application.CardTransactions.Repay
{
    public class GetPayTokenRequest
    {
        public string amount { get; set; }
        public string customer_id { get; set; }
        public string card_token { get; set; }
        public string transaction_type { get; set; }
        public string Source { get; set; } = "DirectAPI";
        public string PaymentChannel { get; set; }
        public string ChannelUser { get; set; }

        public GetPayTokenRequest(RepayTokenPaymentRequest paymentRequest)
        {
            this.amount = paymentRequest.amount;
            this.customer_id = paymentRequest.customer_id;
            this.card_token = paymentRequest.card_token;
            this.transaction_type = paymentRequest.transaction_type;
            this.Source = paymentRequest.Source;
            this.PaymentChannel = paymentRequest.PaymentChannel;
            this.ChannelUser = paymentRequest.ChannelUser;
        }
    }
}
