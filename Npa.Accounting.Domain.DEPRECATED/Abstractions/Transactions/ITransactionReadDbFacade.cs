using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions
{
    public interface ITransactionReadDbFacade
    {
        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<V>> QueryAsync<T, U, V>(string sql, Func<T, U, V> func, object? param = null,
            IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

        Task<V> QueryFirstOrDefaultAsync<T, U, V>(string sql, Func<T, U, V> func, object? param = null,
            IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
        
        Task<IReadOnlyList<T>> QueryProcAsync<T>(string sql, object? param = null, IDbTransaction? tr = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

        Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

        Task<int> ExecNonQueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
    }
}