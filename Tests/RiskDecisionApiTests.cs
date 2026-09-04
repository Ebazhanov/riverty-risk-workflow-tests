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
        const string customerId = "usr_berlin_892";
        _externalMock.SetupCreditRatingResponse(creditScore);

        var request = new RiskEvaluationRequest(customerId, 100.00m, "EUR");

        // Act
        var response = await _client.PostAsJsonAsync($"/v1/credit-rating/{customerId}", request);

        // Assert
        response.StatusCode.Should().Be(expectedCode);

        var result = await response.Content.ReadFromJsonAsync<RiskEvaluationResponse>();
        result.Should().NotBeNull();
        result!.Status.Should().Be(expectedStatus);
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        _externalMock.Stop();
        _client.Dispose();
    }
}