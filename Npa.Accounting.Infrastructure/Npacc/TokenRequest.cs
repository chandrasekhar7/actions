namespace Npa.Accounting.Infrastructure.Npacc
{
    internal class TokenRequest
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int MerchantId { get; set; }
        public int PowerId { get; set; }
        
        public TokenRequest(int id, decimal amount, int merchantId, int powerId)
        {
            Id = id;
            Amount = amount;
            MerchantId = merchantId;
            PowerId = powerId;
        }
    }
}