using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Riverty.RiskWorkflow.Tests.Factories;

public sealed class CustomWebApplicationFactory : IDisposable
{
    private readonly IHost _host;

    public CustomWebApplicationFactory()
    {
        _host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseTestServer();
                webBuilder.ConfigureServices(services =>
                {
                    // Registers routing infrastructure required for MapPost / UseRouting
                    services.AddRouting();
                });
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapPost("/api/v1/risk/evaluate", async context =>
                        {
                            context.Response.StatusCode = 201;
                            await context.Response.WriteAsJsonAsync(new { status = "APPROVED" });
                        });
                    });
                });
            })
            .Start();
    }

    public HttpClient CreateClient()
    {
        return _host.GetTestServer().CreateClient();
    }

    public void Dispose()
    {
        _host.Dispose();
    }
}