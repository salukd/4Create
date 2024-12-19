namespace Clinica.Application.Common.Options;

public class FileValidationOptions
{
    public long FileSizeLimit { get; set; } = 1;
    public string ValidationSchemaFileName { get; set; } = string.Empty;
    public int SchemaExpirationCacheInMinutes { get; set; } = 1;
}
