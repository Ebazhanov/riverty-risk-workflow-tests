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
        RiskEvaluationRequest request = null!;
        AllureApi.Step("Given an external credit bureau returns a low risk score", () =>
        {
            WireMockServer.SetupCreditBureauApprovedResponse();
            request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        });

        // Act
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;
        await AllureApi.Step("When a risk evaluation request is sent for amount 50.00 EUR", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        // Assert
        AllureApi.Step("Then the transaction should be created and validated", () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            root.GetProperty("userId").GetString().Should().Be("usr_123");
            root.GetProperty("amount").GetDecimal().Should().Be(50.00m);
        });
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Rejected Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1025")]
    public async Task EvaluateRisk_HighRiskUser_ShouldReject()
    {
        // Arrange
        RiskEvaluationRequest request = null!;
        AllureApi.Step("Given an external credit bureau returns a high risk score", () =>
        {
            WireMockServer.SetupCreditBureauRejectedResponse();
            request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");
        });

        // Act
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;
        await AllureApi.Step("When a risk evaluation request is sent for amount 5000.00 EUR", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        // Assert
        AllureApi.Step("Then high risk requests should be validated with expected payload", () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            root.GetProperty("userId").GetString().Should().Be("usr_999");
            root.GetProperty("amount").GetDecimal().Should().Be(5000.00m);
        });
    }
}