using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npa.Accounting.Common;
using Npa.Accounting.Infrastructure.Abstractions;

namespace Npa.Accounting.Infrastructure.Crypto;

internal class TestCryptoRepository : ICryptoRepository
{
    private readonly CryptoRepository crypto;

    public TestCryptoRepository(IConfiguration configuration)
    {
        crypto = new CryptoRepository(configuration);
    }

    public async Task<CryptoCard?> GetAsync(int tokenId, Teller teller, CancellationToken t = default)
    {
        var card = await crypto.GetAsync(tokenId, teller, t);

        // return test card
        return new CryptoCard
        {
            CardNumber = "4462030000000000",
            ExpirationDate = "0925",
            FirstName = card?.FirstName ?? "Bob",
            LastName = card?.LastName ?? "Jones",
            Address1 = card?.Address1 ?? "1234 Somewhere",
            Address2 = card?.Address2,
            City = card?.City ?? "Nowhere",
            State = card?.State ?? "TX",
            Zip = card?.Zip ?? "12345",
            Cvv = "123"
        };
    }
}
        