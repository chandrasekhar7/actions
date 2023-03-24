using System;
using Npa.Accounting.Common.Cards;

namespace Npa.Accounting.Domain.DEPRECATED.Customers
{
    public class CustomerCard : Entity<int>
    {
        public string Btid { get; private set; }
        public int PowerId { get; private set; }
        public int CardToken { get; private set; }
        public LastFour LastFour { get; private set; }
        public Expiration Expiration { get; private set; }
        public string? CardType { get; private set; }
        public bool? CanDisburse { get; private set; }
        public bool Deleted { get; private set; }
        public DateTime? DeletedOn { get; private set;}

        private CustomerCard()
        {

        }

        public CustomerCard(int cardToken, int btid, int powerId, LastFour lastFour, Expiration exp, bool? canDisburse = true)
        {
            Btid = btid.ToString();
            PowerId = powerId;
            CardToken = cardToken;
            LastFour = lastFour;
            Expiration = exp;
            CanDisburse = canDisburse ?? true;
            CardType = String.Empty;
        }

        public void UpdateCard(Expiration exp, bool? canDisburse = null)
        {
            Expiration = exp;
            CanDisburse = canDisburse ?? CanDisburse;
            if (exp.ToDate() < DateTime.Now)
            {
                RemoveCard();
            }
        }

        public void RemoveCard()
        {
            Deleted = true;
            DeletedOn = DateTime.Now;
        }
    }
}