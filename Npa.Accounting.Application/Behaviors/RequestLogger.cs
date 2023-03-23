using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Npa.Accounting.Application.Behaviors
{
    public class RequestLogger<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {

        private readonly ILogger logger;

        public RequestLogger(ILogger<TRequest> logger)
        {
            this.logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var name = typeof(TRequest).Name;

            logger.LogInformation("Request: {Name} {@Request}",
                name, request);

            return next();
        }
    }
}