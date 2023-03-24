using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Npa.Accounting.Persistence.DEPRECATED.Abstractions;

public interface IDbFacade
{
    Task<T> ExecSingle<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
    Task<T> ExecSingleProc<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    Task ExecProc(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    void ExecProcSync(string sql, object? param = null, IDbTransaction? transaction = null);

    Task<int> ExecNonQuery(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
}