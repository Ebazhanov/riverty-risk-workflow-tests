using System.Text.Json.Serialization;

namespace Riverty.RiskWorkflow.Tests.Models;

public class RiskEvaluationResponse
{
    [JsonPropertyName("decision")]
    public string Decision { get; set; } = string.Empty;

    [JsonPropertyName("riskScore")]
    public int RiskScore { get; set; }

    [JsonPropertyName("reasonCode")]
    public string? ReasonCode { get; set; }
}