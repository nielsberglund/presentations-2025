// McpLoggingHandler.cs
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class McpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<McpLoggingHandler> _logger;
    private int _requestCounter = 0;

    public McpLoggingHandler(ILogger<McpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var requestId = Interlocked.Increment(ref _requestCounter);
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");

        // Log request
        _logger.LogInformation(
            "[{Timestamp}] Request #{RequestId}: {Method} {Uri}", 
            timestamp, requestId, request.Method, request.RequestUri);

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation(
                "[{Timestamp}] Request #{RequestId} Body:\n{Content}", 
                timestamp, requestId, FormatJson(content));
        }

        // Execute request
        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        // Log response
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation(
            "[{Timestamp}] Response #{RequestId} ({ElapsedMs}ms): {StatusCode}\n{Content}",
            DateTime.UtcNow.ToString("HH:mm:ss.fff"),
            requestId,
            stopwatch.ElapsedMilliseconds,
            response.StatusCode,
            FormatJson(responseContent));

        return response;
    }

    private string FormatJson(string json)
    {
        try
        {
            var parsed = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(parsed, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
        catch
        {
            return json;
        }
    }
}