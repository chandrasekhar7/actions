using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Customers;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Transactions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Customers;
using Npa.Accounting.Persistence.DEPRECATED.DbContexts;
using Npa.Accounting.Persistence.DEPRECATED.Loans;
using Npa.Accounting.Persistence.DEPRECATED.Transactions;

[assembly: InternalsVisibleTo("Npa.Accounting.Tests")]
namespace Npa.Accounting.Persistence.DEPRECATED
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddPersistenceLayer(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ITransactionDbContext,TransactionDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Transaction")));
            services.AddScoped<ITransactionReadDbFacade, TransactionReadDbFacade>();
            services.AddScoped<IFraudDetectionRepository, FraudDetectionRepository>();
            services.AddScoped<IScheduleTransactionRepository, ScheduleTransactionRepository>();
            

            services.AddDbContext<ICardDbContext, CardDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("CardStore")));
            //services.AddScoped<PaymentDbContext>(provider => provider.GetService<PaymentDbContext>() ?? throw new PersistenceLayerException("Could not get DB context."));
            services.AddScoped<ICardStoreRepository, CardStoreRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ILoanLockRepository, LoanLockRepository>();
            services.AddScoped<IAchTransactionRepository, AchTransactionRepository>();
            services.AddTransient<IDbFacade, DbFacade>();

            return services;
        }
    }
}