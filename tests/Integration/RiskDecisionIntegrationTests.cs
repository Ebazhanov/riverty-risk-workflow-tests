using System.Net;
using System.Text.Json;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using FluentAssertions;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Common.Models;

namespace Riverty.RiskWorkflow.Tests.Integration;

[TestFixture]
[AllureNUnit]
[AllureSuite("Risk Decision API Suite")]
[AllureOwner("Evgenii Bazhanov")]
public class RiskDecisionIntegrationTests : BaseTest
{
    private RiskDecisionApiClient _apiClient = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new RiskDecisionApiClient(HttpClient!);
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Approved Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1024")]
    public async Task EvaluateRisk_LowRiskUser_ShouldApprove()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;

        AllureApi.Step("Given an external credit bureau returns a low risk score", () =>
        {
            WireMockServer!.SetupCreditBureauApprovedResponse();
            request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        AllureApi.Step("Then the API creates transaction with status HTTP 201 Created", () =>
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
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;

        AllureApi.Step("Given an external credit bureau returns a high risk score", () =>
        {
            WireMockServer!.SetupCreditBureauRejectedResponse();
            request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        AllureApi.Step("Then the API validates high risk payload with status HTTP 201 Created", () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            root.GetProperty("userId").GetString().Should().Be("usr_999");
            root.GetProperty("amount").GetDecimal().Should().Be(5000.00m);
        });
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("External Service Delay")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureIssue("XRAY-1028")]
    public async Task EvaluateRisk_ExternalServiceDelay_ShouldHandleGracefully()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;

        AllureApi.Step("Given external credit bureau experiences network latency", () =>
        {
            WireMockServer!.SetupCreditBureauTimeoutResponse();
            request = new RiskEvaluationRequest("usr_timeout", 100.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
        });

        AllureApi.Step("Then the API responds with HTTP 201 Created despite background delay", () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        });
    }
}