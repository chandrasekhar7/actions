using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;

namespace Npa.Accounting.Persistence.DEPRECATED.DbContexts;

public class DbFacade : IDbFacade, IDisposable
{
    private readonly IDbConnection connection;

        private bool disposedValue = false;

        public DbFacade(IConfiguration configuration) => connection = new SqlConnection(configuration.GetConnectionString("Transaction"));

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        
        public Task<T> ExecSingle<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, 30, CommandType.Text);
        public Task<T> ExecSingleProc<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, 30, CommandType.StoredProcedure);
        
        public Task ExecProc(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
            => connection.ExecuteAsync(sql, param, transaction, 30, CommandType.StoredProcedure);
        
        public void ExecProcSync(string sql, object? param = null, IDbTransaction? transaction = null)
            => connection.Execute(sql, param, transaction, 30, CommandType.StoredProcedure);

        public Task<int> ExecNonQuery(string sql, object? param = null, IDbTransaction? transaction = null,
            CancellationToken cancellationToken = default) => connection.ExecuteAsync(sql, param, transaction, 30, CommandType.Text);

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