using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Npa.Accounting.Application.Behaviors;
using Npa.Accounting.Application.Transactions;

namespace Npa.Accounting.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services,bool addValidation = false, bool addRequestLogging = false)
        {
            if (addValidation)
            {
                services.AddValidatorsFromAssemblyContaining<ApplicationLayerException>();
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            }

            if (addRequestLogging)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));
            }

            return services;
        }
    }
}