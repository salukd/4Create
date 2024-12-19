using Clinica.Application.Common.Caching;
using Clinica.Application.Common.Services.Cache;
using MediatR;

namespace Clinica.Application.Common.Behaviours;

public sealed class QueryCachingBehavior<TRequest, TResponse>(
    ICacheService cacheService
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        
        return await cacheService.GetOrCreateAsync(
            request.CacheKey,
            async _ => await next(),
            request.Expiration,
            cancellationToken);
    }
}
