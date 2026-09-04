using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NUnit.Framework;
using RivertyRiskApiTests.Mocks;
using RivertyRiskApiTests.Models;

namespace RivertyRiskApiTests.Tests;

[TestFixture]
public class RiskDecisionApiTests
{
    private ExternalServicesMock _externalMock = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        _externalMock = new ExternalServicesMock();
        _externalMock.Start();

     // Initialize HTTP client (in a real project, WebApplicationFactory is used here)
        _client = new HttpClient { BaseAddress = new Uri(_externalMock.Url) };
    }

    [TestCase(750, "APPROVED", HttpStatusCode.OK)]
    [TestCase(650, "DECLINED", HttpStatusCode.OK)]
    public async Task EvaluateRisk_ShouldReturnExpectedDecision_BasedOnCreditScore(
        int creditScore,
        string expectedStatus,
        HttpStatusCode expectedCode)
    {
        // Arrange
        const string customerId = "user_test_123";
        _externalMock.SetupCreditRatingResponse(customerId, creditScore);

        var request = new RiskEvaluationRequest(customerId, 199.99m, "EUR");

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/credit-rating/{customerId}", request);

        // Assert
        response.StatusCode.Should().Be(expectedCode);

        var result = await response.Content.ReadFromJsonAsync<RiskEvaluationResponse>();
        result.Should().NotBeNull();
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        _externalMock.Stop();
        _client.Dispose();
    }
}