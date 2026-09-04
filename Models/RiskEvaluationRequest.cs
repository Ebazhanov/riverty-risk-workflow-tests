namespace RivertyRiskApiTests.Models;

public record RiskEvaluationRequest(
    string CustomerId,
    decimal Amount,
    string Currency
);