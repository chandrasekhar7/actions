using Npa.Accounting.Common.Addresses;
using Npa.Accounting.Common.People;

namespace Npa.Accounting.Common.Cards
{
    public record Card(CardNumber Number, Name Name, Address Address, Cvv Cvv, Expiration Expiration);
}