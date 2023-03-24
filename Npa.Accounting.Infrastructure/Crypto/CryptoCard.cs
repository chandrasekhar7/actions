

namespace Npa.Accounting.Infrastructure.Crypto;

public class CryptoCard
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Address1 { get; init; }
    public string Address2 { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Zip { get; init; }
    public string ExpirationDate { get; init; }
    public string CardNumber { get; init; }
    
    public string Cvv { get; init; }
}