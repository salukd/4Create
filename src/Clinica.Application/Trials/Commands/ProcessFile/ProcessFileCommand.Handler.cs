using Clinica.Application.Common.Extensions;
using Clinica.Application.Common.Services;
using Clinica.Domain.Entities;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Clinica.Application.Trials.Commands.ProcessFile;

public class ProcessFileCommandHandler(
    IApplicationDbContext database,
    ILogger<ProcessFileCommandHandler> logger)
    : IRequestHandler<ProcessFileCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(ProcessFileCommand request, CancellationToken cancellationToken)
    {
        var trialMetadata = await GetTrialMetadataFromFileAsync(request);
        if (trialMetadata.IsError)
        {
            return trialMetadata.FirstError;
        }

        if (await CheckForDuplicateTrialAsync(trialMetadata.Value, cancellationToken))
        {
            return ProcessFileCommandErrors.DuplicateTrialId;
        }

        return await SaveTrialAsync(trialMetadata.Value, cancellationToken);
    }

    private static async Task<ErrorOr<TrialMetadata>> GetTrialMetadataFromFileAsync(ProcessFileCommand request)
    {
        try
        {
            var fileContent = await request.File.ReadContentAsync();
            var trialMetadata = fileContent.Deserialize<TrialMetadata>();

            if (trialMetadata is null)
            {
                return ProcessFileCommandErrors.CannotDeserializeContent;
            }
            
            return trialMetadata;
        }
        catch (Exception e)
        {
            return ProcessFileCommandErrors.CannotDeserializeContent;
        }
    }

    private async Task<bool> CheckForDuplicateTrialAsync(
        TrialMetadata trialMetadata, 
        CancellationToken cancellationToken)
    {
        var isDuplicate = await database.Trials
            .AsNoTracking()
            .AnyAsync(t => t.TrialId == trialMetadata.TrialId, cancellationToken);

        return isDuplicate;
    }

    private async Task<ErrorOr<string>> SaveTrialAsync(TrialMetadata trial, 
        CancellationToken cancellationToken)
    {
        try
        {
            trial.CalculateDuration();
            
            await database.Trials.AddAsync(trial, cancellationToken);
            await database.SaveAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error saving trial to database");
            
            return ProcessFileCommandErrors.DatabaseSaveFailed;
        }
        
        return trial.TrialId;
    }
}