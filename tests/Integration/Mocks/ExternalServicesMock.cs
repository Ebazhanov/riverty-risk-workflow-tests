using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Riverty.RiskWorkflow.Tests.Integration.Mocks;

public class ExternalServicesMock
{
    private WireMockServer? _server;

    public string Url => _server?.Url ?? string.Empty;

    public bool IsStarted => _server is { IsStarted: true };

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupCreditBureauApprovedResponse()
    {
        _server?
            .Given(Request.Create().WithPath("/users").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"userId\":\"usr_123\",\"amount\":50.00,\"status\":\"APPROVED\"}"));
    }

    public void SetupCreditBureauRejectedResponse()
    {
        _server?
            .Given(Request.Create().WithPath("/users").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"userId\":\"usr_999\",\"amount\":5000.00,\"status\":\"REJECTED\"}"));
    }

    public void SetupCreditBureauTimeoutResponse()
    {
        _server?
            .Given(Request.Create().WithPath("/users").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(201)
                .WithDelay(TimeSpan.FromSeconds(3))
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"userId\":\"usr_timeout\",\"amount\":100.00,\"status\":\"APPROVED\"}"));
    }

    public void Stop()
    {
        _server?.Stop();
        _server?.Dispose();
        _server = null;
    }
}