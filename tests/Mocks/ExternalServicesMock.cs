using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Riverty.RiskWorkflow.Tests.Mocks;

public class ExternalServicesMock
{
    private WireMockServer? _server;
    public string Url => _server?.Url ?? string.Empty;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupCreditBureauApprovedResponse()
    {
        _server?
            .Given(Request.Create().WithPath("/v1/credit-check").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"score\": 10, \"blacklisted\": false}"));
    }

    public void SetupCreditBureauRejectedResponse()
    {
        _server?
            .Given(Request.Create().WithPath("/v1/credit-check").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"score\": 90, \"blacklisted\": true}"));
    }

    public void Stop()
    {
        _server?.Stop();
        _server?.Dispose();
    }
}