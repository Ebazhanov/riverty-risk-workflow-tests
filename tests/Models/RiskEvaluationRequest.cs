using System.Text.Json.Serialization;

namespace Riverty.RiskWorkflow.Tests.Models;

public class RiskEvaluationRequest
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "EUR";

    [JsonPropertyName("creditScore")]
    public int CreditScore { get; set; }
}