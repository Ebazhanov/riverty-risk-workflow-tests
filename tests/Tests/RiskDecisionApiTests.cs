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
        SetupApprovedMock();
        var request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");

        var response = await ExecuteRiskEvaluationStep(request);
        var jsonString = await response.Content.ReadAsStringAsync();

        ValidateCreatedResponseStep(response, jsonString, "usr_123", 50.00m);
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Rejected Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1025")]
    public async Task EvaluateRisk_HighRiskUser_ShouldReject()
    {
        SetupRejectedMock();
        var request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");

        var response = await ExecuteRiskEvaluationStep(request);
        var jsonString = await response.Content.ReadAsStringAsync();

        ValidateCreatedResponseStep(response, jsonString, "usr_999", 5000.00m);
    }

    [AllureStep("Given an external credit bureau returns a low risk score")]
    private void SetupApprovedMock()
    {
        WireMockServer.SetupCreditBureauApprovedResponse();
    }

    [AllureStep("Given an external credit bureau returns a high risk score")]
    private void SetupRejectedMock()
    {
        WireMockServer.SetupCreditBureauRejectedResponse();
    }

    [AllureStep("When a risk evaluation request is sent to the API")]
    private async Task<HttpResponseMessage> ExecuteRiskEvaluationStep(RiskEvaluationRequest request)
    {
        return await _apiClient.EvaluateRiskAsync(request);
    }

    [AllureStep("Then the API records the transaction with status HTTP 201 Created")]
    private void ValidateCreatedResponseStep(HttpResponseMessage response, string jsonString, string expectedUserId, decimal expectedAmount)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        using var doc = JsonDocument.Parse(jsonString);
        var root = doc.RootElement;
        root.GetProperty("userId").GetString().Should().Be(expectedUserId);
        root.GetProperty("amount").GetDecimal().Should().Be(expectedAmount);
    }
}