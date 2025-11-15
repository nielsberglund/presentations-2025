#!/usr/bin/env dotnet run
#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Serve static files (HTML page)
app.UseDefaultFiles();
app.UseStaticFiles();

// API endpoint returning "Hello World"
app.MapGet("/api/greeting", () => "Hello World");

// SSE endpoint streaming events with message number and timestamp
app.MapGet("/events", async (HttpContext context) =>
{
    context.Response.Headers.ContentType = "text/event-stream";
    context.Response.Headers.CacheControl = "no-cache";
    context.Response.Headers.Connection = "keep-alive";

    var messageNumber = 0;

    try
    {
        while (!context.RequestAborted.IsCancellationRequested)
        {
            messageNumber++;
            var eventData = new
            {
                MessageNumber = messageNumber,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };

            var json = System.Text.Json.JsonSerializer.Serialize(eventData);
            await context.Response.WriteAsync($"data: {json}\n\n");
            await context.Response.Body.FlushAsync();

            await Task.Delay(1000); // Send event every second
        }
    }
    catch (OperationCanceledException)
    {
        // Client disconnected
    }
});

app.Run();
