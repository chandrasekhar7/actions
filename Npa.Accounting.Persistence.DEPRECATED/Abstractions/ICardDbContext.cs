using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npa.Accounting.Domain.DEPRECATED.Customers;

namespace Npa.Accounting.Persistence.DEPRECATED.Abstractions
{
    public interface ICardDbContext
    {
        IDbConnection Connection { get; }
        bool HasChanges { get; }
        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        DbSet<CustomerCard> Cards { get; }
    }
}