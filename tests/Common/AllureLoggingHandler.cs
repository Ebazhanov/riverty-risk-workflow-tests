using System.Net.Http.Headers;
using System.Text;
using Allure.Net.Commons;

namespace Riverty.RiskWorkflow.Tests.Common;

/// <summary>
/// HTTP Message Handler that automatically logs API request and response payloads as attachments in Allure reports.
/// </summary>
public class AllureLoggingHandler : DelegatingHandler
{
    public AllureLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

    /// <summary>
    /// Intercepts outgoing HTTP requests and incoming HTTP responses to attach telemetry to Allure steps.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Capture and attach HTTP Request details
        var requestBody = request.Content != null 
            ? await request.Content.ReadAsStringAsync(cancellationToken) 
            : string.Empty;

        var requestLog = $"{request.Method} {request.RequestUri}\n\nHeaders:\n{FormatHeaders(request.Headers)}\n\nBody:\n{requestBody}";
        
        AllureApi.AddAttachment(
            $"Request: {request.Method} {request.RequestUri?.AbsolutePath}", 
            "text/plain", 
            Encoding.UTF8.GetBytes(requestLog), 
            ".txt"
        );

        // 2. Execute the HTTP request
        var response = await base.SendAsync(request, cancellationToken);

        // 3. Capture and attach HTTP Response details
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseLog = $"HTTP {(int)response.StatusCode} {response.StatusCode}\n\nHeaders:\n{FormatHeaders(response.Headers)}\n\nBody:\n{responseBody}";

        AllureApi.AddAttachment(
            $"Response: {(int)response.StatusCode} {request.RequestUri?.AbsolutePath}", 
            "application/json", 
            Encoding.UTF8.GetBytes(responseLog), 
            ".json"
        );

        return response;
    }

    /// <summary>
    /// Formats HTTP headers into a key-value string list.
    /// </summary>
    private static string FormatHeaders(HttpHeaders headers)
    {
        return string.Join("\n", headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));
    }
}