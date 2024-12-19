using Clinica.Application.Common.Options;
using Clinica.Application.Trials.Commands.ProcessFile;
using Clinica.Application.Trials.Queries.GetFilteredTrials;
using Clinica.Application.Trials.Queries.GetTrialById;
using Clinica.Contracts.Trials;
using Clinica.Domain.Entities;

namespace Clinica.Api.Rest.Controllers;

[ApiVersion("1.0")]
public class TrialMetadataController(ISender mediator, IOptions<FileValidationOptions> options) : ApiController(options)
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadTrial([FromForm] ProcessFileCommand command, CancellationToken cancellationToken)
    {
        var processFileResult = await mediator.Send(command, cancellationToken);
        
        return processFileResult.Match(
            trialId => CreatedAtAction(
                nameof(GetTrialById),
                new {trialId},
                new {trialId}),
            Problem);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? title = null, 
        [FromQuery] int? participants = null, 
        [FromQuery] TrialStatus? status = null, 
        [FromQuery] int? durationInDays = null)
    {
        var trials = await mediator.Send(new GetFilteredTrialsQuery(title, 
            participants, 
            status, 
            durationInDays));

        return trials.Count == 0 ? Ok(Array.Empty<GetTrialResponse>()) 
            : Ok(trials.ConvertAll(MapToGetTrialResponse));
    }

    [HttpGet("{trialId}")]
    public async Task<IActionResult> GetTrialById(string trialId, CancellationToken cancellationToken)
    {
        var trial = await mediator.Send(new GetTrialByIdQuery(trialId), cancellationToken);

        return trial is null ? 
            Problem(Error.NotFound(description: "Trial not found!")) 
            : Ok(MapToGetTrialResponse(trial));
    }
    
    private GetTrialResponse MapToGetTrialResponse(TrialMetadata trial)
    {
        return new GetTrialResponse(
            trial.TrialId,
            trial.Title,
            trial.StartDate,
            trial.EndDate,
            trial.Participants,
            trial.Status.ToString(),
            trial.DurationInDays
        );
    }
    
}


