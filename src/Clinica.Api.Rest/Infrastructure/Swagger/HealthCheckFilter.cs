namespace Clinica.Api.Rest.Infrastructure.Swagger;

public class HealthCheckFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathItem = new OpenApiPathItem();

        var operation = new OpenApiOperation();
        operation.Tags.Add(new OpenApiTag { Name = "Health" });
        operation.Summary = "Health check endpoint";
        operation.Description = "Returns the health status of the API and its dependencies";
        operation.Responses.Add("200", new OpenApiResponse { Description = "Healthy" });
        operation.Responses.Add("503", new OpenApiResponse { Description = "Unhealthy" });

        pathItem.AddOperation(OperationType.Get, operation);

        swaggerDoc.Paths.Add("/health", pathItem);
    }
}