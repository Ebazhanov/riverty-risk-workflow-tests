using NUnit.Framework;
using Testcontainers.PostgreSql;
using Riverty.RiskWorkflow.Tests.Clients;
using Riverty.RiskWorkflow.Tests.Mocks;

namespace Riverty.RiskWorkflow.Tests.Tests;

[TestFixture]
public abstract class BaseTest
{
    public static PostgreSqlContainer? DbContainer { get; private set; }
    public static ExternalServicesMock? WireMockServer { get; private set; }
    public static HttpClient? HttpClient { get; private set; }

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        await InitializeGlobalStateAsync();
    }

    public static async Task InitializeGlobalStateAsync()
    {
        if (DbContainer is null)
        {
            DbContainer = new PostgreSqlBuilder()
                .WithDatabase("risk_db")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await DbContainer.StartAsync();
        }

        if (WireMockServer is null)
        {
            WireMockServer = new ExternalServicesMock();
            WireMockServer.Start();
        }

        if (HttpClient is null)
        {
            var primaryHandler = new HttpClientHandler();
            var loggingHandler = new AllureLoggingHandler(primaryHandler);

            // Using char '/' satisfies CA1866 (perf) and CA1310 (culture invariance)
            var baseUrl = WireMockServer.Url.EndsWith('/') ? WireMockServer.Url : $"{WireMockServer.Url}/";

            HttpClient = new HttpClient(loggingHandler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        WireMockServer?.Stop();
        HttpClient?.Dispose();

        if (DbContainer is not null)
        {
            await DbContainer.DisposeAsync();
        }
    }
}