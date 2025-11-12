// File: webapi.cs
#:sdk Microsoft.NET.Sdk.Web

// Create a minimal API
var app = WebApplication.Create();

// Map a simple GET endpoint
app.MapGet("/", () => "Hello, Minimal API!");

// Run the application
app.Run(); 