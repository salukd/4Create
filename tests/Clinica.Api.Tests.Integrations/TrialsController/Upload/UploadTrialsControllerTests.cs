using System.Net;
using FluentAssertions;

namespace Clinica.Api.Tests.Integrations.TrialsController.Upload;

[Collection("ApiIntegrationTests")]
public class UploadTrialsControllerTests(ClinicaApiFactory apiFactory) : IClassFixture<ClinicaApiFactory>
{
    private readonly HttpClient _client = apiFactory.CreateClient();


    [Fact]
    public async Task Upload_JsonFile_ShouldReturnTrialId_WhenFileIsValid()
    {
        // Arrange
        var formContent = await apiFactory.GetFileAsync("TrialsController/Upload/Files/trial-success.json");

        // Act
        var response = await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("trialId");
    }
    
    
    [Fact]
    public async Task Upload_JsonFile_ShouldReturnValidationError_WhenFileExtensionIsNotJson()
    {
        // Arrange
        var formContent = await apiFactory.GetFileAsync("TrialsController/Upload/Files/trial-invalid-ext.yml");

        // Act
        var response = await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Only JSON files are allowed.");
    }
    
    [Fact]
    public async Task Upload_JsonFile_ShouldReturnValidationError_WhenFileIsTooBig()
    {
        // Arrange
        var formContent = apiFactory.GenerateBigfile();

        // Act
        var response = await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("File size must not exceed 2 MB");
    }
    
    [Fact]
    public async Task Upload_JsonFile_ShouldReturnValidationError_WhenEnumInFileVioletesSchemaRules()
    {
        // Arrange
        var formContent = await apiFactory.GetFileAsync("TrialsController/Upload/Files/trial-invalid-enum.json");

        // Act
        var response = await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("File content does not conform to the required JSON schema.");
    }

   
}