using System.Data;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Domain.DEPRECATED.Draws;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Scheduled;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Configurations;

namespace Npa.Accounting.Persistence.DEPRECATED.DbContexts
{
    public class TransactionDbContext : DbContext, ITransactionDbContext
    {
        public IDbConnection Connection { get; set; }
        public bool HasChanges { get; set; }
        public DbSet<AchTransaction> AchTransactions { get; set; }
        public DbSet<CardTransaction> CardTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        public DbSet<ScheduledAch> ScheduledAchs { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Draw> Draws { get; set; }

        public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly,
                type => type != typeof(CustomerCardEntityConfiguration));
        }
    }
}