using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npa.Accounting.Common;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Common.Transactions;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Domain.DEPRECATED.Loans;
using Npa.Accounting.Domain.DEPRECATED.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.DbContexts;

namespace Npa.Accounting.Tests.Persistence.Fixtures
{
    public class TransactionDbFixture : IDisposable
    {
        protected readonly TransactionDbContext context;
        protected readonly DbConnection _connection;

        public TransactionDbContext Context => context;

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public TransactionDbFixture()
        {
            _connection = CreateInMemoryDatabase();
            var options = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseSqlite(_connection)
                //.UseSqlServer("Data Source=NPATest;Initial Catalog=paydayflex;Persist Security Info=True;User ID=CLIENT1;Password=CLIENT1;Application Name=Lending;")
                .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                .Options;
            context = new TransactionDbContext(options);
            
            InitDb();
        }

        private void InitDb()
        {
            context.Database.EnsureCreated();
            context.Loans.Add(new Loan(1000, 1, new LoanInfo(Location.California, new Credit(500,400), 100, true)));
            var card = new Transaction(1000, 20, DateTime.Now, TransactionType.Debit, new Teller("ILM"), new CardTransaction(
                new CustomerCard(1, 1, 1, new LastFour("1234"), new Expiration(12, 25), false), new Merchant(702)));
            context.Transactions.Add(card);
            // Initialization functions
            context.SaveChanges();
        }

        public void Dispose() => Context.Dispose();
    }
}