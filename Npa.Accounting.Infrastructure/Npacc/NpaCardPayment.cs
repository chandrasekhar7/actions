using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Infrastructure.Npacc
{
    internal class NpaCardPayment
    {
        public decimal Amount { get; init; }
        public int? MerchantId { get; init; }
        public int PowerId { get; init; }
        public string Number { get; init; }
        public string Cvv { get; init; }
        public string Exp { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string Zip { get; init; }
        
        private NpaCardPayment() {}

        public static NpaCardPayment Create(int merchantId, int customerId, Card card, decimal amount) => new NpaCardPayment()
        {
            Amount = amount,
            MerchantId = merchantId,
            PowerId = customerId,
            Number = card.Number.ToString(),
            Cvv = card.Cvv.ToString(),
            Exp = card.Expiration.ToString(),
            FirstName = card.Name.First,
            LastName = card.Name.Last,
            Street = card.Address.Street1,
            City = card.Address.City,
            State = card.Address.State,
            Zip = card.Address.ZipCode.ToFiveDigit()
        };
    }
}