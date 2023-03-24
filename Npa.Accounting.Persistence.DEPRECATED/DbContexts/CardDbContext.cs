using System.Data;
using Microsoft.EntityFrameworkCore;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Configurations;

namespace Npa.Accounting.Persistence.DEPRECATED.DbContexts;

public class CardDbContext : DbContext, ICardDbContext
{
    public IDbConnection Connection { get; set; }
    public bool HasChanges { get; set; }
    public DbSet<CustomerCard> Cards { get; set; }

    public CardDbContext(DbContextOptions<CardDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new CustomerCardEntityConfiguration());
}