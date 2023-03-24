using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Cards;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Communications;
using Npa.Accounting.Domain.DEPRECATED.Abstractions.Users;
using Npa.Accounting.Infrastructure.Abstractions;
using Npa.Accounting.Infrastructure.Authentication;
using Npa.Accounting.Infrastructure.Crypto;
using Npa.Accounting.Infrastructure.Customers;
using Npa.Accounting.Infrastructure.ErrorHandling;
using Npa.Accounting.Infrastructure.Loans;
using Npa.Accounting.Infrastructure.Lpp;
using Npa.Accounting.Infrastructure.Npacc;
using Npa.Accounting.Infrastructure.Repay;
using Npa.Accounting.Infrastructure.Services;
using Npa.Accounting.Infrastructure.Users;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions;
using Npa.Accounting.Persistence.DEPRECATED.Abstractions.Loans;
using Polly;
using Polly.Extensions.Http;

namespace Npa.Accounting.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {

            services.AddCustomAuthentication(configuration);
            services.AddCustomAuthorization();
            services.AddHttpClient<ILoanService, LoanService>(c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("LoanManagement:Uri"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.GetValue<string>("LoanManagement:Auth"));
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
            services.AddProcessors(configuration, env);
            
          
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICommunicationsService, CommunicationsService>();
            services.AddHttpClient<ICustomerInfoRepository, CustomerInfoRepository>(c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("CustomerApi:Uri"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.GetValue<string>("CustomerApi:Auth"));
            });
            return services;
        }

        private static IServiceCollection AddProcessors(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            // var repayOptions = new RepayOptions();
            // configuration.Bind(RepayOptions.Key, repayOptions);
            services.Configure<RepayOptions>(configuration.GetSection(RepayOptions.Key));
            //services.AddSingleton(repayOptions);
            services.AddHttpClient<IRepayService, RepayService>();
            
            services.AddHttpClient<LegacyRepayService>(c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("RepayApiShim:Uri"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration.GetValue<string>("RepayApiShim:Auth"));
            });
            services.AddHttpClient<ILppService, LppService>(c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("Lpp:Uri"));
            });
            
            services.Configure<LppOptions>(configuration.GetSection("Lpp"));

            if (env.IsProduction())
            {
                services.AddScoped<ICryptoRepository, CryptoRepository>();
            }
            else
            {
                // Use the card number replacer
                services.AddScoped<ICryptoRepository, TestCryptoRepository>();
            }
            
            services.AddScoped<ICardTransactionServiceDeprecated, CardTransactionServiceDeprecated>();

            return services;
        }
        
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }

        public static void UseCustomErrors(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomErrorHandlingMiddleware>();
        }
    }
}