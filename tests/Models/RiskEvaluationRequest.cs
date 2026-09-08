namespace Riverty.RiskWorkflow.Tests.Models;

public record RiskEvaluationRequest(
    string UserId,
    decimal Amount,
    string Currency,
    string PaymentMethod
);