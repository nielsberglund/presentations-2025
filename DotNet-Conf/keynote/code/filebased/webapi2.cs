// File: webapi2.cs
#:sdk Microsoft.NET.Sdk.Web

// Create a minimal API
var app = WebApplication.Create();

// Serve the HTML file at the root
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("index.html");
});

// Map a simple GET endpoint
app.MapGet("/sse", async context=>
{
    context.Response.ContentType = "text/event-stream";
    for (var i = 0; i < 5; i++)
    {
        await context.Response.WriteAsync($"data: Message {i}\n\n");
        await context.Response.Body.FlushAsync();
        await Task.Delay(1000);
    }
});

// Run the application
app.Run(); 