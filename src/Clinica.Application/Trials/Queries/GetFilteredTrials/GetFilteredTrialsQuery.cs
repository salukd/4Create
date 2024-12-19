using Clinica.Application.Common.Services;
using Clinica.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Application.Trials.Queries.GetFilteredTrials;

public record GetFilteredTrialsQuery(string? Title, int? Participants, 
    TrialStatus? Status, int? MinDurationInDays) : IRequest<List<TrialMetadata>>;

public class GetFilteredTrialsQueryHandler(IApplicationDbContext database)
    : IRequestHandler<GetFilteredTrialsQuery, List<TrialMetadata>>
{
    public async Task<List<TrialMetadata>> Handle(GetFilteredTrialsQuery request, 
        CancellationToken cancellationToken)
    {
        var trials =  await GetTrialsAsync(request, cancellationToken);

        return trials;
    }

    private async Task<List<TrialMetadata>> GetTrialsAsync(GetFilteredTrialsQuery query, 
        CancellationToken cancellationToken)
    {
        return await database.Trials.Where(w =>
            (string.IsNullOrWhiteSpace(query.Title) || w.Title.Contains(query.Title)) &&
            (!query.Participants.HasValue || w.Participants == query.Participants.Value) &&
            (!query.Status.HasValue || w.Status == query.Status.Value) &&
            (!query.MinDurationInDays.HasValue || w.DurationInDays == query.MinDurationInDays.Value)
        ).ToListAsync(cancellationToken);
    }
}
    