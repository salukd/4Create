using System.Text.Json;
using System.Text.Json.Serialization;
using Clinica.Application.Common.Services.Cache;
using StackExchange.Redis;

namespace Clinica.Infrastructure.Services.Cache;

public class CacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, 
        TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();
        var cachedValue = await db.StringGetAsync(key);
        
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return JsonSerializer.Deserialize<T>(cachedValue!, _jsonSerializerOptions)!;
        }

        var result = await factory(cancellationToken);

        if (result is not null)
        {
            await db.StringSetAsync(key, JsonSerializer.Serialize(result, _jsonSerializerOptions),
                expiration ?? DefaultExpiration);
        }

        return result;
    }
}
