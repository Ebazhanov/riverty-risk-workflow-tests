using System.Net.Http.Json;
using System.Text.Json;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Clients;

public class RiskDecisionApiClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RiskDecisionApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> EvaluateRiskAsync(RiskEvaluationRequest request)
    {
        return await _client.PostAsJsonAsync("/v1/credit-rating/evaluate", request, JsonOptions);
    }
}