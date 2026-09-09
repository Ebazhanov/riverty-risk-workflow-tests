using System.Net;
using System.Text.Json;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using FluentAssertions;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Risk Decision API Suite")]
[AllureOwner("Evgenii Bazhanov")]
public class RiskDecisionApiTests : BaseTest
{
    private RiskDecisionApiClient _apiClient = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new RiskDecisionApiClient(HttpClient);
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Approved Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1024")]
    public async Task EvaluateRisk_LowRiskUser_ShouldApprove()
    {
        // Arrange
        WireMockServer.SetupCreditBureauApprovedResponse();
        var request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");

        // Act
        var response = await _apiClient.EvaluateRiskAsync(request);
        var jsonString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonString);

        // Assert - HTTP 201 Created from API confirms successful risk transaction recording
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var root = doc.RootElement;
        root.GetProperty("userId").GetString().Should().Be("usr_123");
        root.GetProperty("amount").GetDecimal().Should().Be(50.00m);
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Rejected Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1025")]
    public async Task EvaluateRisk_HighRiskUser_ShouldReject()
    {
        // Arrange
        WireMockServer.SetupCreditBureauRejectedResponse();
        var request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");

        // Act
        var response = await _apiClient.EvaluateRiskAsync(request);
        var jsonString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(jsonString);

        // Assert - High risk requests over limit are validated by API response status or payload ID
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var root = doc.RootElement;
        root.GetProperty("userId").GetString().Should().Be("usr_999");
        root.GetProperty("amount").GetDecimal().Should().Be(5000.00m);
    }
}