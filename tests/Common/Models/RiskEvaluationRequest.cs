namespace Riverty.RiskWorkflow.Tests.Common.Models;

public record RiskEvaluationRequest(
    string UserId,
    decimal Amount,
    string Currency,
    string PaymentMethod
);