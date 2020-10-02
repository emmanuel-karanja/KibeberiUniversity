using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KibeberiUniversity.Behaviors
{
    public class LoggingBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
       private readonly ILogger<TRequest> _logger; 

       public LoggingBehavior(ILogger<TRequest> logger)=> _logger=logger;

       public async Task<TResponse> Handle( TRequest request, 
                                            CancellationToken cancellationToken,
                                            RequestHandlerDelegate<TResponse> next)
       {
           using (_logger.BeginScope(request))
           {
               _logger.LogInformation($"Handling {typeof(TRequest).Name}");
               var response=await next();
               _logger.LogInformation($"Handled {typeof(TResponse).Name} with result {0}",response);
               return response;
           }
       }
    }
}