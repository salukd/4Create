using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Application.Common.Services;

public interface IApplicationDbContext
{
    DbSet<TrialMetadata> Trials { get; }
    DbSet<TrialsValidationSchema> ValidationSchemas { get;}
    
    Task<int> SaveAsync(CancellationToken cancellationToken);
}