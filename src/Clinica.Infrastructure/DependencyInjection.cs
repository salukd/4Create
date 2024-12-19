using Clinica.Application.Common.Services;
using Clinica.Application.Common.Services.Cache;
using Clinica.Application.Common.Services.Validation;
using Clinica.Infrastructure.Persistence;
using Clinica.Infrastructure.Services.Cache;
using Clinica.Infrastructure.Services.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Clinica.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>((_, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.ConfigureWarnings(w => 
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<IJsonSchemaValidationService, JsonSchemaValidationService>();
        
        AddRedisCache(services, configuration);
       
        return services;
    }
    
    private static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
            configuration.GetConnectionString("Cache")!));
    }
}