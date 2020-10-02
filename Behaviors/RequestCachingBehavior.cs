/*using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using KibeberiUniversity.Utils;


namespace KibeberiUniversity.Behaviors
{
    public class RequestCachingBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly IByteSerializer _byteSerializer;
        private readonly IDistributedCache _cache;

        public RequestCachingBehavior(ILogger<TRequest> logger, IByteSerializer byteSerializer, IDistributedCache cache)
        {
            _logger=logger;
            _byteSerializer=byteSerializer;
            _cache=cache;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if(request is ICacheableQuery cacheableQuery)
            {
                TResponse response;
                async Task<TResponse> GetResponseAndAddToCache()
                {
                    response=await next();
                    await _cache.SetAsync(cacheableQuery.CacheKEy, _byteSerializer.Serialize(response),
                                          options,cancellationToken);
                    return response;
                }

                if(cacheableQuery.ReplaceCachedEntry)
                {
                    _logger.LogInformation($"Replacing cache entry for key '{cacheableQuery.CacheKey}'.");
                    response=await GetResponseAndAddToCache();
                }
                else
                {
                    var cachedResponse=await _cache.GetAsync(cacheableQuery.CacheKey, cancellationToken);
                    if(cachedResponse != null)
                    {
                        _logger.LogInformation($"Cache hit for key  '{cacheableQuery.CacheKey}'.");
                        response=_byteSerializer.Deserialize<TResponse>(cachedResponse);
                    }
                    else
                    {
                        _logger.LogInformation($"Cache miss for key '{cacheableQuery.CacheKey}' .");
                        response=await GetResponseAndAddToCache();
                    }
                }

                if(cacheableQuery.RefreshCachedEntry)
                {
                    _logger.LogInformation($"Cache refreshed for key '{cacheableQuery.CacheKey}'.");
                    await _cache.RefreshAsync(cacheableQuery.CacheKey, cancellationToken);
                }

                return response;
            }
            else
            {
                return await next;
            }
        }
    }
}*/