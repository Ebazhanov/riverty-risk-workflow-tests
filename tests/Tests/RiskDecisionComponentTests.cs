using System.Net;
using System.Net.Http.Json;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using FluentAssertions;
using NUnit.Framework;
using Riverty.RiskWorkflow.Tests.Factories;
using Riverty.RiskWorkflow.Tests.Models;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Risk Decision Component Suite")]
[AllureOwner("Evgenii Bazhanov")]
public sealed class RiskDecisionComponentTests : IDisposable
{
    private static CustomWebApplicationFactory? _factory;
    private static HttpClient? _client;

    [OneTimeSetUp]
    public static void OneTimeSetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Test]
    [AllureFeature("In-Memory Component Testing")]
    [AllureStory("Fast Validation via WebApplicationFactory")]
    [AllureSeverity(SeverityLevel.critical)]
    public async Task PostEvaluateRisk_InMemoryCall_ReturnsCreatedStatus()
    {
        RiskEvaluationRequest request = null!;
        HttpResponseMessage response = null!;

        AllureApi.Step("Given a valid risk evaluation request payload", () =>
        {
            request = new RiskEvaluationRequest("usr_123", 50.00m, "EUR", "BNPL");
        });

        await AllureApi.Step("When the request is sent to the in-memory TestServer", async () =>
        {
            response = await _client!.PostAsJsonAsync("/api/v1/risk/evaluate", request);
        });

        AllureApi.Step("Then the in-memory server responds with HTTP 201 Created", () =>
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        });
    }

    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    public void Dispose()
    {
        // Cleanup per instance if required
    }
}