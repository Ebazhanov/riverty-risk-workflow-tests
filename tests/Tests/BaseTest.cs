using NUnit.Framework;
using Testcontainers.PostgreSql;
using Riverty.RiskWorkflow.Tests.Mocks;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
public abstract class BaseTest
{
    public static PostgreSqlContainer DbContainer { get; set; } = null!;
    public static ExternalServicesMock WireMockServer { get; set; } = null!;
    public static HttpClient HttpClient { get; set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        await InitializeGlobalStateAsync();
    }

    public static async Task InitializeGlobalStateAsync()
    {
        if (DbContainer == null)
        {
            DbContainer = new PostgreSqlBuilder()
                .WithDatabase("risk_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await DbContainer.StartAsync();
        }

        if (WireMockServer == null)
        {
            WireMockServer = new ExternalServicesMock();
            WireMockServer.Start();
        }

        if (HttpClient == null)
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri("https://reqres.in/api/")
            };
        }
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        WireMockServer?.Stop();
        if (DbContainer != null) await DbContainer.DisposeAsync().AsTask();
        HttpClient?.Dispose();
    }
}