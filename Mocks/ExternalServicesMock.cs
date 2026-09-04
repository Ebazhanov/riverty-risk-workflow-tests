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

    /// <summary>
    /// Stubs the Credit Rating endpoint to respond based on the provided credit score.
    /// </summary>
    public void SetupCreditRatingResponse(int score)
    {
        _server.Reset(); // Clear previous mappings before each test setup

        _server
            .Given(Request.Create()
                .WithPath("/v1/credit-rating/*")
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody($"{{\"decisionId\": \"dec_123\", \"status\": \"{(score >= 700 ? "APPROVED" : "DECLINED")}\", \"reasonCode\": \"{(score >= 700 ? null : "CREDIT_SCORE_TOO_LOW")}\"}}"));
    }

    public void Stop()
    {
        _server?.Stop();
        _server?.Dispose();
    }
}