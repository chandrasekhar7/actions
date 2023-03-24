using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.DbContexts
{
    public class TransactionReadDbFacade : ITransactionReadDbFacade, IDisposable
    {
        private readonly IDbConnection connection;

        private bool disposedValue = false;

        public TransactionReadDbFacade(IConfiguration configuration) => connection = new SqlConnection(configuration.GetConnectionString("Transaction"));

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        
        public async Task<IReadOnlyList<V>> QueryAsync<T,U,V>(string sql, Func<T,U,V> func, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => (await connection.QueryAsync<T,U,V>(sql, func, param, transaction)).AsList();

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
                            => (await connection.QueryAsync<T>(sql, param, transaction)).AsList();
        
        public async Task<IReadOnlyList<T>> QueryProcAsync<T>(string sql, object? param = null, IDbTransaction? tr = null)
        {
            return (await connection.QueryAsync<T>(sql, param, tr, commandType: CommandType.StoredProcedure)).AsList();
        }
        
        public async Task<V> QueryFirstOrDefaultAsync<T,U,V>(string sql, Func<T,U,V> func, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => (await connection.QueryAsync<T,U,V>(sql, func, param, transaction)).FirstOrDefault()!;

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => await connection.QuerySingleAsync<T>(sql, param, transaction);

        public async Task<int> ExecNonQueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => await connection.ExecuteAsync(sql, param, transaction);

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
    }
}