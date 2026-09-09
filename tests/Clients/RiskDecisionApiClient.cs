using System.Net.Http.Json;
using Riverty.RiskWorkflow.Tests.Models;

using Riverty.RiskWorkflow.Tests.Shared.Models;

public class RiskDecisionApiClient(HttpClient client)
{
    public async Task<HttpResponseMessage> EvaluateRiskAsync(RiskEvaluationRequest request)
    {
        return await client.PostAsJsonAsync("users", request);
    }
}