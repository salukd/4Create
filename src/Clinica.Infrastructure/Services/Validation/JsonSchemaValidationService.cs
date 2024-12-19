using Clinica.Application.Common.Options;
using Clinica.Application.Common.Services;
using Clinica.Application.Common.Services.Cache;
using Clinica.Application.Common.Services.Validation;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Clinica.Infrastructure.Services.Validation;

public class JsonSchemaValidationService(IApplicationDbContext dbContext, ICacheService cacheService, 
    IOptions<FileValidationOptions> options) : IJsonSchemaValidationService
{
    private readonly string _validationKey = "json-validation-schema";
    public async Task<ErrorOr<Success>> ValidateJsonSchemaAsync(string jsonContent, CancellationToken cancellationToken)
    {
        var schema = await cacheService.GetOrCreateAsync<string?>(_validationKey, async =>
                 GetValidationSchemaAsync(cancellationToken),
            expiration: TimeSpan.FromMinutes(options.Value.SchemaExpirationCacheInMinutes),
            cancellationToken: cancellationToken);
        

        if (string.IsNullOrEmpty(schema))
        {
            return Error.NotFound("ValidationSchema", "No schema found for validation.");
        }

        try
        {
            var jsonSchema = JSchema.Parse(schema);
            var jsonObject = JObject.Parse(jsonContent);

            return jsonObject.IsValid(jsonSchema, out IList<string> errors)
                ? Result.Success
                : Error.Validation("JsonValidationFailed", string.Join(", ", errors));
        }
        catch (Exception ex)
        {
            return Error.Validation("JsonSchemaError", ex.Message);
        }
    }

    private async Task<string?> GetValidationSchemaAsync(CancellationToken cancellationToken)
    {
        return await dbContext.ValidationSchemas
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => v.ValidationSchema)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}