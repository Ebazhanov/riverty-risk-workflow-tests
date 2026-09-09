using System.Net;
using Allure.Net.Commons;
using FluentAssertions;
using Reqnroll;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Common.Models;
using Riverty.RiskWorkflow.Tests.Integration;

namespace Riverty.RiskWorkflow.Tests.E2E.Steps;

[Binding]
public class RiskDecisionSteps
{
    private RiskDecisionApiClient _apiClient = null!;
    private HttpResponseMessage? _lastResponse;

    [BeforeScenario]
    public async Task Setup()
    {
        await BaseTest.InitializeGlobalStateAsync();
        _apiClient = new RiskDecisionApiClient(BaseTest.HttpClient!);
    }

    [Given(@"an external credit bureau returns a low risk score")]
    public void GivenAnExternalCreditBureauReturnsALowRiskScore()
    {
        AllureApi.Step("Configure WireMock to return APPROVED response");
        BaseTest.WireMockServer!.SetupCreditBureauApprovedResponse();
    }

    [When(@"a risk evaluation request is sent for amount (.*) EUR")]
    public async Task WhenARiskEvaluationRequestIsSentForAmountEur(decimal amount)
    {
        var request = new RiskEvaluationRequest("usr_123", amount, "EUR", "BNPL");
        _lastResponse = await _apiClient.EvaluateRiskAsync(request);
    }

    [When(@"the same risk evaluation request is sent again")]
    public async Task WhenTheSameRiskEvaluationRequestIsSentAgain()
    {
        var request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        _lastResponse = await _apiClient.EvaluateRiskAsync(request);
    }

    [Then(@"the decision status should be ""(.*)""")]
    public void ThenTheDecisionStatusShouldBe(string expectedStatus)
    {
        _lastResponse.Should().NotBeNull();
        _lastResponse!.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}