using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace RivertyRiskApiTests.Mocks;

public class ExternalServicesMock
{
    private WireMockServer _server = null!;

    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetupCreditRatingResponse(string customerId, int score)
    {
        _server
            .Given(Request.Create().WithPath($"/v1/credit-rating/{customerId}").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($"{{\"customerId\": \"{customerId}\", \"score\": {score}}}"));
    }

    public void Stop()
    {
        _server?.Stop();
    }
}