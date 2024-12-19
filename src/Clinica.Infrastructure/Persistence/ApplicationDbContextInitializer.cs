using Clinica.Application.Common.Exceptions;
using Clinica.Application.Common.Options;
using Clinica.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Clinica.Infrastructure.Persistence;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();

        await initializer.SeedAsync();
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    IOptions<FileValidationOptions> options)
{
    private readonly string _validationFileName =
        options.Value.ValidationSchemaFileName
        ?? throw new ClinicaApplicationException("Validation schema file name not found");
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!context.ValidationSchemas.Any())
        {
            await context.ValidationSchemas.AddAsync(new TrialsValidationSchema
            {
                ValidationSchema = await GetValidationFileAsync()
            });

            await context.SaveChangesAsync();
        }
    }

    private async Task<string> GetValidationFileAsync()
    {
        if (File.Exists(_validationFileName))
        {
            return await File.ReadAllTextAsync(_validationFileName);
        }

        throw new ClinicaApplicationException("Validation schema file not found");
    }
}