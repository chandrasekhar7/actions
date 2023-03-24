using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npa.Accounting.Common;
using Npa.Accounting.Infrastructure.Abstractions;

namespace Npa.Accounting.Infrastructure.Crypto;

internal class CryptoRepository : ICryptoRepository
{
    private readonly IDbConnection connection;

    private bool disposedValue = false;

    public CryptoRepository(IConfiguration configuration) =>
        connection = new SqlConnection(configuration.GetConnectionString("Crypto"));

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                connection.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    private Task<int> GetIdAsync(int tokenId, CancellationToken t = default) => connection.QuerySingleAsync<int>(
        "[NPADC1].choosepdf.dbo.USAePayPaymentMethods_GetIDFromToken", new {token = tokenId},
        commandType: CommandType.StoredProcedure);

    //the linked server is available in both live and test
    //do not let the name fool you, 
    //it Is safe to use linked server [NPADC1] in test mode
    //[NPADC1] is just the name of the link, it is actually 
    //linked to the appropriate test/live instance
    //so [NPADC1] in test mode links back to NPALOG
    public async Task<CryptoCard?> GetAsync(int tokenId, Teller teller, CancellationToken t = default) =>
        await connection.QueryFirstOrDefaultAsync<CryptoCard?>(
            "safestore.dbo.GetCardsByUSAEPID", new {id = tokenId, user = teller.ToString()},
            commandType: CommandType.StoredProcedure);
}