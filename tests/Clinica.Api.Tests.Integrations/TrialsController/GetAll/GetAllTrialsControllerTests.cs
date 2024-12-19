using System.Net;
using Clinica.Contracts.Trials;
using FluentAssertions;
using Newtonsoft.Json;

namespace Clinica.Api.Tests.Integrations.TrialsController.GetAll;

[Collection("ApiIntegrationTests")]
public class GetAllTrialsControllerTests(ClinicaApiFactory apiFactory) : IClassFixture<ClinicaApiFactory>
{
    private readonly HttpClient _client = apiFactory.CreateClient();
    
    [Fact]
    public async Task GetAll_ShouldReturnTrials_WhenExists()
    {
        // Arrange
        var formContent = await apiFactory.GetFileAsync("TrialsController/GetAll/Files/trial-valid.json");
        await _client.PostAsync("/api/v1/TrialMetadata/upload", formContent);
        
        // Act
        var response = await _client.GetAsync($"/api/v1.0/TrialMetadata?status=Ongoing&participants=123&durationDays=180");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        var trials = JsonConvert.DeserializeObject<List<GetTrialResponse>>(responseContent);
        
        
        trials[0].TrialId.Should().Be("1");
        trials[0].Title.Should().Be("Clinical Trial 57");
        trials[0].StartDate.Should().Be(DateTime.Parse("2024-12-17"));
        trials[0].EndDate.Should().Be(DateTime.Parse("2025-06-15"));
        (trials[0].EndDate - trials[0].StartDate)!.Value.Days.Should().Be(180);
        trials[0].DurationInDays.Should().Be(180);
        trials[0].Participants.Should().Be(123);
        trials[0].Status.Should().Be("Ongoing");
        
    }
    
    [Fact]
    public async Task GetById_ShouldReturnEmptyList_WhenTrialsNotExists()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"/api/v1.0/TrialMetadata?title=test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var trials = JsonConvert.DeserializeObject<List<GetTrialResponse>>(responseContent);

        trials.Count.Should().Be(0);
    }
}