namespace Riverty.RiskWorkflow.Tests.Models;

public record RiskEvaluationResponse(
    string DecisionId,
    string Status,
    int RiskScore,
    List<string> RejectionReasons
);