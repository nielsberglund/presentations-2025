using System.Runtime.InteropServices;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== MCP Client with Azure OpenAI (using SDK) ===\n");

        DotNetEnv.Env.TraversePath().Load();

        var endpoint = Environment.GetEnvironmentVariable("AZURE_API_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");

        Console.WriteLine($"Endpoint: {endpoint}");
        Console.WriteLine($"API Key: {apiKey}");

        var deploymentName = "gpt-5.1-chat";

        /*
          grok-4-fast-reasoning
          gpt-5.1-codex-mini
          gpt-5.1-chat
        */

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: Please set the following environment variables:");
            Console.WriteLine("  - AZURE_OPENAI_ENDPOINT");
            Console.WriteLine("  - AZURE_OPENAI_API_KEY");
            Console.WriteLine("  - AZURE_OPENAI_DEPLOYMENT_NAME (optional, defaults to 'gpt-4o')");
            return;
        }

        // get path to the mcp.json configuration file
        //it may be different based on Windows or Linux/mac environment
        // there are different mcp.json files for Windows and Linux/mac in this repo
        //check the OS and set the path accordingly
        string mcpConfigPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            mcpConfigPath = Path.Combine(AppContext.BaseDirectory, "mcp-win.json");
        }
        else
        {
            mcpConfigPath = Path.Combine(AppContext.BaseDirectory, "mcp-mac.json");
        }

        // Load MCP server configuration
        var mcpConfig = await LoadMcpConfigAsync(mcpConfigPath);
        if (mcpConfig == null)
        {
            Console.WriteLine("Error: Could not load mcp.json configuration");
            return;
        }

        McpClient? mcpClient = null;

        try
        {
            // Create Azure OpenAI client using IChatClient abstraction with function invocation
            Console.WriteLine($"Connecting to Azure OpenAI: {deploymentName}");
            var azureClient = new AzureOpenAIClient(
              new Uri(endpoint),
              new Azure.AzureKeyCredential(apiKey)
              );

            IChatClient chatClient = new ChatClientBuilder(
                azureClient.GetChatClient(deploymentName).AsIChatClient())
                .UseFunctionInvocation() // Automatic function calling!
                .Build();


            // Create MCP client using the SDK
            Console.WriteLine($"\nConnecting to MCP server: {mcpConfig.Command}");
            mcpClient = await McpClient.CreateAsync(
              new StdioClientTransport(new()
              {

                  Command = mcpConfig.Command,
                  Arguments = mcpConfig.Args.ToArray(),
                  EnvironmentVariables = mcpConfig.Env!,
                  Name = "MSSQL MCP Server"
              })
            );

            // List available tools - returns McpClientTool which extends AIFunction
            Console.WriteLine("\nFetching available tools...");
            IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

            Console.WriteLine($"Available tools ({tools.Count}):");
            foreach (var tool in tools)
            {
                Console.WriteLine($"  - {tool.Name}: {tool.Description}");
            }
            // Interactive chat loop
            Console.WriteLine("\n=== Interactive Chat ===");
            Console.WriteLine("Ask questions about your database. Type 'exit' to quit.\n");

            List<ChatMessage> messages = [
                new ChatMessage(ChatRole.System,
                    "You are a helpful AI assistant with access to a SQL Server database. " +
                    "Use the available tools to help users query and understand their data. " +
                    "When you need to query the database, use the provided tools. " +
                    "Always explain your results in a clear and user-friendly way.")
            ];

            while (true)
            {
                Console.Write("You: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                messages.Add(new ChatMessage(ChatRole.User, userInput));

                // Use streaming response with automatic function invocation
                Console.Write("\nAssistant: ");
                var responseBuilder = new System.Text.StringBuilder();

                await foreach (var update in chatClient.GetStreamingResponseAsync(
                    messages,
                    new ChatOptions { Tools = [.. tools] }))
                {
                    if (update.Text != null)
                    {
                        Console.Write(update.Text);
                        responseBuilder.Append(update.Text);
                    }
                }
                Console.WriteLine("\n");

                // Add assistant response to history
                messages.Add(new ChatMessage(ChatRole.Assistant, responseBuilder.ToString()));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            if (mcpClient != null)
            {
                await mcpClient.DisposeAsync();
                //await mcpClient.DisposeAsync();
            }
        }

    }

    /// <summary>
    // Load MCP server configuration from mcp.json
    /// </summary>
    static async Task<McpServerConfig?> LoadMcpConfigAsync(string mcpJsonPath)
    {
        try
        {
            if (!File.Exists(mcpJsonPath))
            {
                Console.WriteLine($"Error: mcp.json not found at {Path.GetFullPath(mcpJsonPath)}");
                return null;
            }

            var json = await File.ReadAllTextAsync(mcpJsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var config = JsonSerializer.Deserialize<McpJsonConfig>(json, options);

            if (config?.Servers == null || !config.Servers.ContainsKey("mssql-mcp"))
            {
                Console.WriteLine("Error: mssql-mcp server not found in mcp.json");
                return null;
            }

            return config.Servers["mssql-mcp"];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mcp.json: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// MCP JSON configuration structure
    /// </summary>
    class McpJsonConfig
    {
        [System.Text.Json.Serialization.JsonPropertyName("servers")]
        public Dictionary<string, McpServerConfig>? Servers { get; set; }
    }

    /// <summary>
    /// MCP Server configuration
    /// </summary>
    class McpServerConfig
    {
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("command")]
        public string Command { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("args")]
        public List<string> Args { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("env")]
        public Dictionary<string, string> Env { get; set; } = new();
    }
}
