#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.OpenApi@10.*-*


var app = WebApplication.Create();
//var builder = WebApplication.CreateBuilder();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


//var app = builder.Build();
//app.UseSwagger();


app.MapGet("/", () => "Hello, Minimal API!");

app.Run();