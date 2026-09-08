using System.Net.Http.Json;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Clients;

public class RiskDecisionApiClient
{
    private readonly HttpClient _client;

    public RiskDecisionApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> EvaluateRiskAsync(RiskEvaluationRequest request)
    {
        return await _client.PostAsJsonAsync("users", request);
    }
}