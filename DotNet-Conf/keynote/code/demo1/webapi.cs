#:sdk Microsoft.NET.Sdk.Web

//filebased minimal asp api
var builder = WebApplication.CreateBuilder();
//create the app
var app = builder.Build();
//map a get endpoint
app.MapGet("/", () => "Hello from .NET 10 Web API!");

//create an endpoint for SSE events
app.MapGet("/events", async context =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");
    for (var i = 0; i < 5; i++)
    {
        await context.Response.WriteAsync($"data: Event {i} at {DateTime.Now.ToString()}\n");
    }
});

//run the app
app.Run();