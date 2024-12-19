using ErrorOr;

namespace Clinica.Application.Common.Services.Validation;

public interface IJsonSchemaValidationService
{
    Task<ErrorOr<Success>> ValidateJsonSchemaAsync(string jsonContent, CancellationToken cancellationToken);
}
