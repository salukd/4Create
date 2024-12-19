using Clinica.Application.Common.Extensions;
using Clinica.Application.Common.Options;
using Clinica.Application.Common.Services.Validation;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Clinica.Application.Trials.Commands.ProcessFile;

public class ProcessFileCommandValidator: AbstractValidator<ProcessFileCommand>
{
    public ProcessFileCommandValidator(IJsonSchemaValidationService schemaValidationService, 
        IOptions<FileValidationOptions> options)
    {
        const int mb = 1024 * 1024;
        
        var maxFileSize = options.Value.FileSizeLimit * mb;
        
        RuleFor(command => command.File)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("File is required.")
            .DependentRules(() =>
            {
                RuleFor(command => command.File.Length)
                    .LessThanOrEqualTo(maxFileSize)
                    .WithMessage($"File size must not exceed {options.Value.FileSizeLimit} MB.");

                RuleFor(command => command.File.FileName)
                    .Must(fileName => fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Only JSON files are allowed.");
            })
            .DependentRules(() =>
            {
                RuleFor(x => x.File)
                    .MustAsync(async (file, cancellationToken) =>
                    {
                        var content = await file.ReadContentAsync();
                        var validationResult = await schemaValidationService.ValidateJsonSchemaAsync(content, cancellationToken);
                        return !validationResult.IsError;
                    }).WithMessage("File content does not conform to the required JSON schema.");
            });
    }
}