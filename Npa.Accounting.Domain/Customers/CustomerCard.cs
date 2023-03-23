using System;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.Cards;

namespace Npa.Accounting.Domain.Customers
{
    public class CustomerCard : Entity<int>
    {
        public int CustomerId { get; private set; }
        public string Token { get; private set; }
        public LastFour LastFour { get; private set; }
        public Expiration Expiration { get; private set; }
        public DateTime AddedOn { get; private set; }
        public DateTime? DeletedOn { get; private set;}
        public CardInfo CardInfo { get; }

        private CustomerCard()
        {

        }

        public CustomerCard(int id, int customerId, string token, LastFour lastFour, Expiration exp, CardInfo info)
        {
            Id = id;
            CustomerId = customerId;
            Token = token;
            LastFour = lastFour;
            Expiration = exp;
            CardInfo = info;
        }

        public void UpdateCardExpiration(Expiration exp)
        {
            Expiration = exp;
            if (exp.ToDate() < DateTime.Now)
            {
                RemoveCard();
            }
        }

        public void RemoveCard()
        {
            DeletedOn = DateTime.Now;
        }
    }
}