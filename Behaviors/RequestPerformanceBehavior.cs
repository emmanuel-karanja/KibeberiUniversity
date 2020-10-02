using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace KibeberiUniversity.Behaviors
{
    public class RequestPerformanceBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly Stopwatch _timer;

        public RequestPerformanceBehavior(ILogger<TRequest> logger)
        {
            _timer=new Stopwatch();
            _logger=logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();
            var response= await next();

            _timer.Stop();

            var name=typeof(TRequest).Name;

            if(_timer.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("Long running request {Name} took ({ElapsedMilliseconds} milliseconds) {@Request}",
                name,_timer.ElapsedMilliseconds,request);

            }
            else
            {
                _logger.LogWarning("Request : {Name} took ({ElapsedMilliseconds} milliseconds) {@Request}",
                name, _timer.ElapsedMilliseconds,request);
            }

            return response;
        }
    }
}