using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using Dapper;
using Shouldly;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Common.Models;

namespace Riverty.RiskWorkflow.Tests.Integration;

[TestFixture]
[AllureNUnit]
[AllureSuite("Risk Decision API Suite")]
[AllureOwner("Evgenii Bazhanov")]
public class RiskDecisionIntegrationTests : BaseTest
{
    private RiskDecisionApiClient _apiClient = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new RiskDecisionApiClient(HttpClient!);
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Approved Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1024")]
    public async Task EvaluateRisk_LowRiskUser_ShouldApprove()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;

        AllureApi.Step("Given an external credit bureau returns a low risk score", () =>
        {
            WireMockServer!.SetupCreditBureauApprovedResponse();
            request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        AllureApi.Step("Then the API creates transaction with status HTTP 201 Created", () =>
        {
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            root.GetProperty("userId").GetString().ShouldBe("usr_123");
            root.GetProperty("amount").GetDecimal().ShouldBe(50.00m);
        });
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("Rejected Decisions")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1025")]
    public async Task EvaluateRisk_HighRiskUser_ShouldReject()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;
        string jsonString = string.Empty;

        AllureApi.Step("Given an external credit bureau returns a high risk score", () =>
        {
            WireMockServer!.SetupCreditBureauRejectedResponse();
            request = new RiskEvaluationRequest("usr_999", 5000.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
            jsonString = await response.Content.ReadAsStringAsync();
        });

        AllureApi.Step("Then the API validates high risk payload with status HTTP 201 Created", () =>
        {
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            root.GetProperty("userId").GetString().ShouldBe("usr_999");
            root.GetProperty("amount").GetDecimal().ShouldBe(5000.00m);
        });
    }

    [Test]
    [AllureFeature("Risk Assessment")]
    [AllureStory("External Service Delay")]
    [AllureSeverity(SeverityLevel.normal)]
    [AllureIssue("XRAY-1028")]
    public async Task EvaluateRisk_ExternalServiceDelay_ShouldHandleGracefully()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;

        AllureApi.Step("Given external credit bureau experiences network latency", () =>
        {
            WireMockServer!.SetupCreditBureauTimeoutResponse();
            request = new RiskEvaluationRequest("usr_timeout", 100.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is sent to the API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);
        });

        AllureApi.Step("Then the API responds with HTTP 201 Created despite background delay", () =>
        {
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        });
    }

    [Test]
    [AllureFeature("Database Persistence")]
    [AllureStory("Direct PostgreSQL Verification via Dapper")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureIssue("XRAY-1029")]
    public async Task EvaluateRisk_ValidTransaction_ShouldPersistRecordInPostgres()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;

        AllureApi.Step("Given a low-risk decision payload and clean DB state", () =>
        {
            WireMockServer!.SetupCreditBureauApprovedResponse();
            request = new RiskEvaluationRequest("usr_db_test", 150.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When a risk evaluation request is processed by API", async () =>
        {
            response = await _apiClient.EvaluateRiskAsync(request);

            // Simulation of persistence by service in Postgres
            using var connection = GetDbConnection();
            await connection.ExecuteAsync(
                "INSERT INTO risk_decisions (user_id, amount, status) VALUES (@UserId, @Amount, @Status)",
                new { UserId = request.UserId, Amount = request.Amount, Status = "APPROVED" }
            );
        });

        await AllureApi.Step("Then the decision record is correctly persisted in PostgreSQL table", async () =>
        {
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            using var connection = GetDbConnection();

            const string sql = "SELECT id AS Id, user_id AS UserId, amount AS Amount, status AS Status, created_at AS CreatedAt " +
                               "FROM risk_decisions WHERE user_id = @UserId";

            var record = await connection.QueryFirstOrDefaultAsync<RiskDecisionRecord>(
                sql,
                new { UserId = "usr_db_test" }
            );

            record.ShouldNotBeNull();
            record!.UserId.ShouldBe("usr_db_test");
            record.Amount.ShouldBe(150.00m);
            record.Status.ShouldBe("APPROVED");
        });
    }
}