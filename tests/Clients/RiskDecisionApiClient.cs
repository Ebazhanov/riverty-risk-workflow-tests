using System.Net.Http.Json;
using Riverty.RiskWorkflow.Tests.Common.Models;

namespace Riverty.RiskWorkflow.Tests.Clients;

public class RiskDecisionApiClient(HttpClient client)
{
    public async Task<HttpResponseMessage> EvaluateRiskAsync(RiskEvaluationRequest request)
    {
        return await client.PostAsJsonAsync("users", request);
    }
}