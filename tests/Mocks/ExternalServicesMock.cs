using System.Text.Json;
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

        _server
            .Given(Request.Create().WithPath("/v1/credit-rating/evaluate").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithCallback(request =>
                {
                    int score = 0;
                    var bodyString = request.BodyData?.BodyAsString;

                    if (!string.IsNullOrEmpty(bodyString))
                    {
                        using var doc = JsonDocument.Parse(bodyString);
                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            if (prop.NameEquals("creditScore") || prop.NameEquals("CreditScore"))
                            {
                                score = prop.Value.GetInt32();
                            }
                        }
                    }

                    string decision = score >= 700 ? "APPROVED" : "DECLINED";

                    return new WireMock.ResponseMessage
                    {
                        StatusCode = 200,
                        BodyData = new WireMock.Util.BodyData
                        {
                            DetectedBodyType = WireMock.Types.BodyType.String,
                            BodyAsString = $"{{\"decision\": \"{decision}\", \"riskScore\": {score}}}"
                        }
                    };
                }));
    }

    public void Stop()
    {
        _server?.Stop();
        _server?.Dispose();
    }
}