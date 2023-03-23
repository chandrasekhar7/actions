using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Npa.Accounting.Application.Behaviors
{
    public class RequestPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {

        private readonly ILogger<TRequest> logger;

        private readonly Stopwatch timer;

        public RequestPerformanceBehavior(ILogger<TRequest> logger)
        {
            timer = new Stopwatch();

            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            timer.Start();

            var response = await next();

            timer.Stop();

            var name = typeof(TRequest).Name;

            if (timer.ElapsedMilliseconds > 500)
            {
                logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    name, timer.ElapsedMilliseconds, request);
            }
            else
            {
                logger.LogInformation("Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    name, timer.ElapsedMilliseconds, request);
            }

            return response;
        }
    }
}