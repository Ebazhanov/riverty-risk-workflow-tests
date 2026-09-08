using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
public class RiskDecisionApiTests : BaseTest
{
    private RiskDecisionApiClient _apiClient = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new RiskDecisionApiClient(HttpClient);
    }

    [Test]
    public async Task EvaluateRisk_LowRiskUser_ShouldApprove()
    {
        WireMockServer.SetupCreditBureauApprovedResponse();

        var request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        var response = await _apiClient.EvaluateRiskAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task EvaluateRisk_HighRiskUser_ShouldReject()
    {
        WireMockServer.SetupCreditBureauRejectedResponse();

        var request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");
        var response = await _apiClient.EvaluateRiskAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}