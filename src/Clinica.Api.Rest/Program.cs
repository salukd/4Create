using Clinica.Api.Rest.Infrastructure;
using Clinica.Application;
using Clinica.Application.Common.Options;
using Clinica.Infrastructure;
using Clinica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOptions<FileValidationOptions>()
    .Bind(config: builder.Configuration.GetSection(
        key: "FileValidationOptions"));

builder.Services.AddPresentation()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!,
        healthQuery: "select 1",
        name: "SQL Server",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "live", "critical" })
    .AddRedis(builder.Configuration.GetConnectionString("Cache")!,
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ready", "optional" });

var app = builder.Build();

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

await app.InitialiseDatabaseAsync();

app.UseHsts();
app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

try
{
    Log.Information("Starting application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}