namespace Riverty.RiskWorkflow.Tests.Common.Models;

public record RiskDecisionRecord(
    int Id,
    string UserId,
    decimal Amount,
    string Status,
    DateTime CreatedAt
);