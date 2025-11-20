using System.Runtime.InteropServices;
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<McpChatService>();

// Configure CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Additional dev-only configuration can go here
}

// app.UseHttpsRedirection();
app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();

// ============================================
// MINIMAL API ENDPOINTS
// ============================================

// 1. Hello World endpoint (Removed to serve index.html)
// app.MapGet("/", () => "Hello World from .NET 10 Minimal API!")
//    .WithName("HelloWorld");


// 2. Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    framework = "net10.0",
    language = "C# 14"
}))
.WithName("HealthCheck");

// 3. Example chat endpoint (placeholder for MCP integration)
app.MapPost("/api/chat", async (ChatRequest request, McpChatService chatService) =>
{
    try 
    {
        var response = await chatService.ChatAsync(request.Message);
        return Results.Ok(new ChatResponse
        {
            Message = response,
            Timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: 500);
    }
})
.WithName("Chat");


app.Run();

// ============================================
// REQUEST/RESPONSE MODELS
// ============================================

record ChatRequest(string Message);

record ChatResponse
{
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}
