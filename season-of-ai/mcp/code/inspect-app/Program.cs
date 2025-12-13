using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using McpDemo;
using ModelContextProtocol.Client;
using System.Runtime.InteropServices;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Azure.Identity;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Linq;
using System.Reflection;
using ModelContextProtocol;
using System.Data;


class Program
{
  static async Task Main(string[] args)
  {
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    bool isDebug = false;
    
    // read command line args
    var arguments = ParseArguments(args);

    if (arguments.ContainsKey("--debug"))
    {
      isDebug = true;
    }

    // In your Program.cs or startup
    ProtocolVisualizer.LogPhase("STARTING APPLICATION", ConsoleColor.Yellow);

    // get environment variables
    DotNetEnv.Env.TraversePath().Load();

    var endpoint = Environment.GetEnvironmentVariable("AZURE_API_ENDPOINT");
    var apiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");
    var deploymentName = Environment.GetEnvironmentVariable("AZURE_AI_MODEL") ?? "gpt-5.1-chat";

    //set up path for MCP config file
    string mcpConfigPath;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      mcpConfigPath = Path.Combine(AppContext.BaseDirectory, "mcp-win.json");
    }
    else
    {
      mcpConfigPath = Path.Combine(AppContext.BaseDirectory, "mcp-mac.json");
    }

    ProtocolVisualizer.LogPhase("LOADING MCP CONFIGURATION", ConsoleColor.Yellow);

    //load the config
    var mcpConfig = await McpConfig.LoadMcpConfigAsync(mcpConfigPath);
    if (mcpConfig == null)
    {
      Console.WriteLine("Error: Could not load mcp.json configuration");
      return;
    }

    ProtocolVisualizer.LogPhase("PHASE 1: INITIATING CONNECTION", ConsoleColor.Cyan);

    McpClient? mcpClient = null;
    //initialise the client and connect       

    // mcpClient = await McpClient.CreateAsync(
    //       new StdioClientTransport(new()
    //       {
    //         Command = mcpConfig.Command,
    //         Arguments = mcpConfig.Args.ToArray(),
    //         EnvironmentVariables = mcpConfig.Env!,
    //         Name = "MSSQL MCP Server"
    //       })
    //     );

    if(isDebug)
    {
      Console.WriteLine("DEBUG MODE: About to connect to interceptor. Click any key to continue ...");
      Console.ReadLine();
    }

    mcpClient = await McpClient.CreateAsync(
      new StdioClientTransport(new()
      {
        Command = "node",
        Arguments = ["/Users/nielsb/repos/presentations/presentations-2025/MssqlMcp/Node/dist/interceptor-mac.cjs"],
        Name = "MCP Interceptor"
      })
    );

    if(isDebug)
    {
      Console.WriteLine("DEBUG MODE: Connected to interceptor. Click any key to continue  ...");
      Console.ReadLine();
    }

    ProtocolVisualizer.LogPhase("PHASE 2: DISCOVERING TOOLS", ConsoleColor.Cyan);

    IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

    ProtocolVisualizer.LogResponse("MCP Server", "C# App", "Available tools", tools);

    foreach (var tool in tools)
    {
      Console.WriteLine($"  - {tool.Name}: {tool.Description}");
    }

    if(isDebug)
    {
      Console.WriteLine("DEBUG MODE: Listed tools. Click any key to continue  ...");
      Console.ReadLine();
    }

    ProtocolVisualizer.LogPhase("PHASE 3: CONFIGURING AZURE AI", ConsoleColor.Magenta);

    var azureClient = new AzureOpenAIClient(
        new Uri(endpoint!),
        new Azure.AzureKeyCredential(apiKey!)
    );

    IChatClient chatClient = new ChatClientBuilder(
      azureClient.GetChatClient(deploymentName).AsIChatClient())
      .UseFunctionInvocation() // Automatic function calling!
      .Build();

    List<ChatMessage> messages = [
        new ChatMessage(ChatRole.System,
            "You are a helpful AI assistant with access to a SQL Server database. " +
            "Use the available tools to help users query and understand their data. " +
            "When you need to query the database, use the provided tools. " +
            "Always explain your results in a clear and user-friendly way.")
    ];

    while (true)
    {
      ProtocolVisualizer.LogPhase("PHASE 4: USER PROMPT", ConsoleColor.Yellow);

      Console.Write("User: ");
      var userInput = Console.ReadLine();
      if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
      {
        Console.WriteLine("Goodbye!");
        break;
      }

      ProtocolVisualizer.LogRequest("User (Niels)", "Azure AI", userInput);

      messages.Add(new ChatMessage(ChatRole.User, userInput!));
     
      var responseBuilder = new System.Text.StringBuilder();

      var response = await chatClient.GetResponseAsync(
        messages, new ChatOptions { Tools = [.. tools] });

      //print the responses
      PrintResponses(response);

      if(response.Text!= null)
      {
        ProtocolVisualizer.LogPhase("FINAL RESPONSE", ConsoleColor.Cyan);
        Console.WriteLine();
        Console.WriteLine(response.Text);
        Console.WriteLine(); 
        ProtocolVisualizer.LogPhase("END FINAL RESPONSE", ConsoleColor.Cyan);    
      }
      messages.Add(new ChatMessage(ChatRole.Assistant, responseBuilder.ToString()));
        

    }

    // var builder = Host.CreateApplicationBuilder(args);
    // builder.Services.AddTransient<McpLoggingHandler>();
    // builder.Services.AddHttpClient("AzureAI")
    //     .AddHttpMessageHandler<McpLoggingHandler>();

    // using IHost host = builder.Build();
    // await host.RunAsync();

    ProtocolVisualizer.LogPhase("APPLICATION STOPPED. CLICK ANY KEY TO EXIT", ConsoleColor.Yellow);
    Console.ReadLine();
  }

  static void PrintResponses(ChatResponse response)
  {
    var chatMessages = response.Messages;

      foreach (var chatMessage in chatMessages)
      {
        if (chatMessage.Role == ChatRole.Assistant)
        {
          if (chatMessage.Contents.OfType<FunctionCallContent>().Any())
          {
            foreach (var functionCall in chatMessage.Contents.OfType<FunctionCallContent>())
            {
              ProtocolVisualizer.LogRequest("Azure AI", "MCP Server", $"I want to call function: {functionCall.Name}", chatMessage);

            }
          }
          else
          {
            ProtocolVisualizer.LogResponse("Azure AI", "C# App", "AI Response", chatMessage);
          }
        }
        else if (chatMessage.Role == ChatRole.Tool)
        {
          ProtocolVisualizer.LogResponse("MCP Server", "Azure AI", "MCP Response", chatMessage);
        }
        else if (chatMessage.Role == ChatRole.User)
        {
          ProtocolVisualizer.LogResponse("Azure AI", "C# App", "AI Response", chatMessage);
        }
      }
   
  }

  static Dictionary<string, string> ParseArguments(string[] args)
  {
    var arguments = new Dictionary<string, string>();

    foreach (var arg in args)
    {
      // Split the argument by '=' to handle key/value pairs
      string[] parts = arg.Split('=');

      // Check if the argument is in the format "key=value"
      if (parts.Length == 2)
      {
        arguments[parts[0]] = parts[1];
      }
      // If not, assume it's just a named argument without a value
      else
      {
        arguments[arg] = null;
      }
    }

    return arguments;
  }
}

