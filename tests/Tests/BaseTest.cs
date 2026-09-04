using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Mocks;

namespace Riverty.RiskWorkflow.Tests.Tests;

public abstract class BaseTest
{
    protected ExternalServicesMock MockServer { get; private set; } = null!;
    protected RiskDecisionApiClient ApiClient { get; private set; } = null!;
    private HttpClient _httpClient = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        MockServer = new ExternalServicesMock();
        MockServer.Start();

        _httpClient = new HttpClient { BaseAddress = new Uri(MockServer.Url) };
        ApiClient = new RiskDecisionApiClient(_httpClient);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _httpClient.Dispose();
        MockServer.Stop();
    }
}