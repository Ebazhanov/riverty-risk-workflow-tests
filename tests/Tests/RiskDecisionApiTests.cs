using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
public class RiskDecisionApiTests : BaseTest
{
    [TestCase(750, "APPROVED", HttpStatusCode.OK, TestName = "TC-RISK-001: High credit score returns APPROVED")]
    [TestCase(650, "DECLINED", HttpStatusCode.OK, TestName = "TC-RISK-002: Low credit score returns DECLINED")]
    public async Task EvaluateRisk_ShouldReturnExpectedDecision_BasedOnCreditScore(
        int score,
        string expectedDecision,
        HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var request = new RiskEvaluationRequest
        {
            CustomerId = "usr_test_123",
            Amount = 150.00m,
            Currency = "EUR",
            CreditScore = score
        };

        // Act
        var response = await ApiClient.EvaluateRiskAsync(request);

        // Assert
        response.StatusCode.Should().Be(expectedStatusCode);

        var result = await response.Content.ReadFromJsonAsync<RiskEvaluationResponse>();
        result.Should().NotBeNull();
        result!.Decision.Should().Be(expectedDecision);
        result.RiskScore.Should().Be(score);
    }
}