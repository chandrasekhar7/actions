using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npa.Accounting.Common.Cards;
using Npa.Accounting.Domain.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.DbContexts;

namespace Npa.Accounting.Tests.Persistence.Fixtures
{
    public class CardDbFixture : IDisposable
    {
        protected readonly CardDbContext context;
        protected readonly DbConnection _connection;

        public CardDbContext Context => context;
        
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public CardDbFixture()
        {
            _connection = CreateInMemoryDatabase();
            var options = new DbContextOptionsBuilder<CardDbContext>()
                .UseSqlite(_connection)
                //.UseSqlServer("Data Source=NPATest;Initial Catalog=paydayflex;Persist Security Info=True;User ID=CLIENT1;Password=CLIENT1;Application Name=Lending;")
                .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                .Options;
            context = new CardDbContext(options);
            InitDb();
        }

        private void InitDb()
        {
            context.Database.EnsureCreated();
            //context.Loans.Add(new LoanEntity(1000, 7, 50, 0, LoanType.Installment, DateTime.Now.AddDays(-14), false));

            context.Cards.Add(new CustomerCard(1, 1,1, new LastFour("4567"), new Expiration(12,25),true));
            // Initialization functions
            context.SaveChanges();
        }

        public void Dispose() => Context.Dispose();
    }
}