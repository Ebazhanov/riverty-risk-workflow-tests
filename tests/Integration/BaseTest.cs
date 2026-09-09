using NUnit.Framework;
using Testcontainers.PostgreSql;
using Riverty.RiskWorkflow.Tests.Common;
using Riverty.RiskWorkflow.Tests.Integration.Mocks;

namespace Riverty.RiskWorkflow.Tests.Integration;

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

        // Start WireMock if missing or stopped
        if (WireMockServer is null || !WireMockServer.IsStarted)
        {
            WireMockServer = new ExternalServicesMock();
            WireMockServer.Start();
            RecreateHttpClient();
        }
        else if (HttpClient is null)
        {
            RecreateHttpClient();
        }
    }

    private static void RecreateHttpClient()
    {
        HttpClient?.Dispose();

        var primaryHandler = new HttpClientHandler();
        var loggingHandler = new AllureLoggingHandler(primaryHandler);

        var baseUrl = WireMockServer!.Url.EndsWith('/') ? WireMockServer.Url : $"{WireMockServer.Url}/";

        HttpClient = new HttpClient(loggingHandler)
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        WireMockServer?.Stop();
        WireMockServer = null; // Forces re-creation on subsequent test suite runs

        HttpClient?.Dispose();
        HttpClient = null;

        if (DbContainer is not null)
        {
            await DbContainer.DisposeAsync();
            DbContainer = null;
        }
    }
}