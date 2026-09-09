using System.Text.Json.Serialization;

namespace Riverty.RiskWorkflow.Tests.Models;

public record RiskEvaluationResponse(
    [property: JsonPropertyName("status")] string? Status = null,
    [property: JsonPropertyName("reason")] string? Reason = null,
    [property: JsonPropertyName("decision")] string? Decision = null
);