using System.Text;
using System.Text.Json;
using Clinica.Api.Rest;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Testcontainers.Redis;
using Testcontainers.SqlEdge;

namespace Clinica.Api.Tests.Integrations;

[CollectionDefinition("ApiIntegrationTests", DisableParallelization = true)]
public class ClinicaApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly SqlEdgeContainer _dbContainer = new SqlEdgeBuilder()
        .WithImage("mcr.microsoft.com/azure-sql-edge:latest")
        .WithPortBinding("1433")
        .WithPassword("Password123!")
        .Build();
    
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:6.0")
        .WithPortBinding("6379")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IConnectionMultiplexer));
            
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
                _redisContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
    
    public MultipartFormDataContent GenerateBigfile()
    {
        var trials = new List<object>();
        for (int i = 0; i < 100000; i++) 
        {
            trials.Add(new
            {
                trialId = Guid.NewGuid().ToString(),
                title = $"Trial {i}",
                startDate = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd"),
                endDate = DateTime.UtcNow.AddDays(i).ToString("yyyy-MM-dd"),
                participants = i,
                status = "Ongoing"
            });
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(trials, options);
        
        var fileContent = Encoding.UTF8.GetBytes(json);
        
        return new MultipartFormDataContent
        {
            { new ByteArrayContent(fileContent), "File", "trial-big.json" }
        };
    }
    
    public async Task<MultipartFormDataContent> GetFileAsync(string filename)
    {
        var filePath = $"{filename}"; 
        var fileContent = await File.ReadAllBytesAsync(filePath);
        var formContent = new MultipartFormDataContent
        {
            { new ByteArrayContent(fileContent), "File", Path.GetFileName(filePath) }
        };
        
        return formContent;
    }
}

