namespace RivertyRiskApiTests.Models;

public record RiskEvaluationResponse(
    string DecisionId,
    string Status,
    string ReasonCode
);