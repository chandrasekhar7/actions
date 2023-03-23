using System.Threading;
using System.Threading.Tasks;
using Npa.Accounting.Common;
using Npa.Accounting.Infrastructure.Crypto;

namespace Npa.Accounting.Infrastructure.Abstractions;

public interface ICryptoRepository
{
    Task<CryptoCard?> GetAsync(int tokenId, Teller teller, CancellationToken t = default);
}