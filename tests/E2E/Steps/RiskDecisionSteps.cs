using System.Net;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using FluentAssertions;
using Reqnroll;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Integration;
using Riverty.RiskWorkflow.Tests.Shared.Models;

namespace Riverty.RiskWorkflow.Tests.E2E.Steps;

[Binding]
[AllureSuite("Risk Decision E2E Suite")]
[AllureFeature("Risk Decisioning Workflow")]
public sealed class RiskDecisionSteps
{
    private RiskDecisionApiClient _apiClient = null!;
    private HttpResponseMessage _response = null!;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        await BaseTest.InitializeGlobalStateAsync();
    }

    [BeforeScenario]
    public void Setup()
    {
        _apiClient = new RiskDecisionApiClient(BaseTest.HttpClient!);
    }

    [Given("an external credit bureau returns a low risk score")]
    public static void GivenAnExternalCreditBureauReturnsALowRiskScore()
    {
        BaseTest.WireMockServer!.SetupCreditBureauApprovedResponse();
    }

    [When("a risk evaluation request is sent for amount {decimal} EUR")]
    public async Task WhenARiskEvaluationRequestIsSentForAmountEur(decimal amount)
    {
        var request = new RiskEvaluationRequest("usr_bdd_123", amount, "EUR", "BNPL");
        _response = await _apiClient.EvaluateRiskAsync(request);
    }

    [Then("the decision status should be {string}")]
    public void ThenTheDecisionStatusShouldBe(string expectedStatus)
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}