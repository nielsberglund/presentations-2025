#:sdk Microsoft.NET.Sdk.Web
#:property EnableRequestDelegateGenerator=true
#:property SuppressTrimAnalysisWarnings=true

var builder = WebApplication.CreateBuilder();

//create my api app
var app = builder.Build();

//set up a default endpoint 
app.MapGet("/", () => "Hello World!");

// set up an endpoint returning SSE data
app.MapGet("/sse", async context =>
{
    context.Response.ContentType = "text/event-stream";
    for (var i = 0; i < 5; i++)
    {
        await context.Response.WriteAsync($"data: Message {i}\n\n");
        await context.Response.Body.FlushAsync();
        await Task.Delay(1000);
    }
});

app.Run();