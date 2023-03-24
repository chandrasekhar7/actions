using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED.Transactions;

namespace Npa.Accounting.Persistence.DEPRECATED.Abstractions
{
    public interface ITransactionDbContext
    {
        IDbConnection Connection { get; }
        bool HasChanges { get; }
        EntityEntry Entry(object entity);
        
        ChangeTracker ChangeTracker { get; }
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        DbSet<AchTransaction> AchTransactions { get; }
        DbSet<CardTransaction> CardTransactions { get; }
        DbSet<ScheduledAch> ScheduledAchs { get; }
        public DbSet<Transaction> Transactions { get; }
        public DbSet<Loan> Loans { get; }
        public DbSet<Draw> Draws { get; }
    }
}