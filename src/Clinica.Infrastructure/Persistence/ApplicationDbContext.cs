using System.Reflection;
using Clinica.Application.Common.Services;
using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IApplicationDbContext
{
    public DbSet<TrialMetadata> Trials => Set<TrialMetadata>();
    public DbSet<TrialsValidationSchema> ValidationSchemas => Set<TrialsValidationSchema>();
    
    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}