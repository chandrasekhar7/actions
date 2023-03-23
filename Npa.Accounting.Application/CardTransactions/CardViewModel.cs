
using Npa.Accounting.Common.Addresses;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.People;

namespace Npa.Accounting.Application.CardTransactions
{
    public class CardViewModel
    {
        /// <summary>
        /// Credit card number
        /// </summary>
        /// <example>41111111111111111</example>
        public string Number { get; set; }
        /// <summary>
        /// Customer first name
        /// </summary>
        /// <example>Bob</example>
        public string FirstName { get; set; }
        /// <summary>
        /// Customer last name
        /// </summary>
        /// <example>Jones</example>
        public string LastName { get; set; }
        /// <summary>
        /// Billing street
        /// </summary>
        /// <example>1234 Fraud Ln.</example>
        public string Street { get; set; }
        
        /// <summary>
        /// Billing city
        /// </summary>
        /// <example>FraudsterVille</example>
        public string City { get; set; }
        
        /// <summary>
        /// Billing state
        /// </summary>
        /// <example>KS</example>
        public string State { get; set; }
        
        /// <summary>
        /// Zip code
        /// </summary>
        /// <example>92508</example>
        /// <example>87654</example>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// Number on back of the card
        /// </summary>
        /// <example>999</example>
        public string Cvv { get; set; }
        
        /// <summary>
        /// Expiration in MM/yy format
        /// </summary>
        /// <example>12/28</example>
        public string Expiration { get; set; }

        public Card ToCard() => new Card(new CardNumber(Number), new Name(FirstName, LastName),
            new Address(Street, City, new State(State), new ZipCode(ZipCode)), new Cvv(Cvv), (Expiration) Expiration);
    }
}