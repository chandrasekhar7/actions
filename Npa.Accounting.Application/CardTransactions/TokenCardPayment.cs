

namespace Npa.Accounting.Application.CardTransactions
{
    public class TokenCardPayment
    {
        public decimal Amount { get; set; }
        public int MerchantId { get; set; }
        public int CustomerId { get; set; }
        public int Token { get; set; }

        public TokenCardPayment(int merchantId, int customerId, decimal amount, int token)
        {
            Amount = amount;
            MerchantId = merchantId;
            CustomerId = customerId;
            Token = token;
        }
    }
}