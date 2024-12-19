using Clinica.Application.Common.Caching;
using Clinica.Application.Common.Services;
using Clinica.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Application.Trials.Queries.GetTrialById;

public record GetTrialByIdQuery(string TrialId) : ICachedQuery<TrialMetadata?>
{
    public string CacheKey => $"get-trial-by-id-query_{TrialId}";
    public TimeSpan? Expiration => TimeSpan.FromSeconds(300);
}

public class GetTrialByIdQueryHandler(IApplicationDbContext database) : 
    IRequestHandler<GetTrialByIdQuery, TrialMetadata?>
{
    public async Task<TrialMetadata?> Handle(GetTrialByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var trial = await database.Trials.FirstOrDefaultAsync(f => f.TrialId == request.TrialId,
            cancellationToken);

        return trial;
    }
}