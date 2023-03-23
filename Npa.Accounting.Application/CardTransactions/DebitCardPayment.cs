
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Application.CardTransactions
{
    public class DebitCardPayment
    {
        public decimal Amount { get; set; }
        public int MerchantId { get; set; }
        public int CustomerId { get; set; }
        public Card Card { get; set; }

        public DebitCardPayment(int merchantId, int customerId, decimal amount, Card card)
        {
            Amount = amount;
            MerchantId = merchantId;
            CustomerId = customerId;
            Card = card;
        }
    }
}