namespace Riverty.RiskWorkflow.Tests.Shared.Models;

public record RiskEvaluationRequest(
    string UserId,
    decimal Amount,
    string Currency,
    string PaymentMethod
);