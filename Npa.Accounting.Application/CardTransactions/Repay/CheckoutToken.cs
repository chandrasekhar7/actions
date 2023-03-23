namespace Npa.Accounting.Application.CardTransactions.Repay
{
    public class CheckoutToken
    {
        public string payment_method { get; set; }
        public string transaction_type { get; set; }
        public string Source { get; set; }
    }
}