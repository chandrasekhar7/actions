namespace Npa.Accounting.Infrastructure.Npacc
{
    internal class NpaTokenPayment
    {
        public int Id { get; init; }
        public decimal Amount { get; init; }
        public int MerchantId { get; init; }
        public int PowerId { get; init; }

        public NpaTokenPayment(int id, int merchantId, int customerId, decimal amount)
        {
            Id = id;
            MerchantId = merchantId;
            PowerId = customerId;
            Amount = amount;
        }
    }
}