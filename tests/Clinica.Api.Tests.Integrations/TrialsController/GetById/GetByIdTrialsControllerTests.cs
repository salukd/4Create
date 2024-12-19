using System.Net;
using Clinica.Contracts.Trials;
using FluentAssertions;
using Newtonsoft.Json;

namespace Clinica.Api.Tests.Integrations.TrialsController.GetById;

[Collection("ApiIntegrationTests")]
public class GetByIdTrialsControllerTests(ClinicaApiFactory apiFactory) : IClassFixture<ClinicaApiFactory>
{
    private readonly HttpClient _client = apiFactory.CreateClient();
    
    [Fact]
    public async Task GetById_ShouldReturnTrial_WhenExists()
    {
        // Arrange
        var formContent = await apiFactory.GetFileAsync("TrialsController/GetById/Files/trial-getbyid.json");
        await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);
        
        // Act
        var response = await _client.GetAsync($"/api/v1.0/TrialMetadata/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        var trial = JsonConvert.DeserializeObject<GetTrialResponse>(responseContent);
        
        trial.Should().NotBeNull();
        trial!.TrialId.Should().Be("1");
        trial.Title.Should().Be("Clinical Trial 57");
        trial.StartDate.Should().Be(DateTime.Parse("2024-12-17"));
        trial.EndDate.Should().Be(DateTime.Parse("2025-06-15"));
        (trial.EndDate - trial.StartDate)!.Value.Days.Should().Be(180);
        trial.DurationInDays.Should().Be(180);
        trial.Participants.Should().Be(123);
        trial.Status.Should().Be("Ongoing");
        
    }
    
    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTrialNotExists()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"/api/v1.0/TrialMetadata/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Trial not found!");
    }
}