using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using DotNetEnv;

public class McpChatService : IAsyncDisposable
{
    private McpClient? _mcpClient;
    private IChatClient? _chatClient;
    private IList<McpClientTool>? _tools;
    private readonly List<ChatMessage> _messages = new();
    private bool _isInitialized = false;

    string? endpoint;
    string? apiKey;
   
    
    //private const string DeploymentName = "gpt-4o-2"; //gpt-5.1-chat
    private const string DeploymentName = "gpt-5.1-chat";
    /*
      grok-4-fast-reasoning
      gpt-5.1-codex-mini
      gpt-5.1-chat
    */

    public McpChatService()
    {
        _messages.Add(new ChatMessage(ChatRole.System,
            "You are a helpful AI assistant with access to a SQL Server database. " +
            "Use the available tools to help users query and understand their data. " +
            "When you need to query the database, use the provided tools. " +
            "Always explain your results in a clear and user-friendly way."));
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        DotNetEnv.Env.TraversePath().Load();

        endpoint = Environment.GetEnvironmentVariable("AZURE_API_ENDPOINT");
        apiKey = Environment.GetEnvironmentVariable("AZURE_API_KEY");

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Azure OpenAI configuration is missing.");
        }

        // Determine MCP config path
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
            throw new InvalidOperationException("Could not load mcp.json configuration");
        }

        // Create Azure OpenAI client
        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new Azure.AzureKeyCredential(apiKey)
        );

        _chatClient = new ChatClientBuilder(
            azureClient.GetChatClient(DeploymentName).AsIChatClient())
            .UseFunctionInvocation() // Automatic function calling!
            .Build();

        // Create MCP client
        _mcpClient = await McpClient.CreateAsync(
            new StdioClientTransport(new()
            {
                Command = mcpConfig.Command,
                Arguments = mcpConfig.Args.ToArray(),
                EnvironmentVariables = mcpConfig.Env!,
                Name = "MSSQL MCP Server"
            })
        );

        // List available tools
        _tools = await _mcpClient.ListToolsAsync();
        
        _isInitialized = true;
    }

    public async Task<string> ChatAsync(string userInput)
    {
        if (!_isInitialized) await InitializeAsync();
        if (_chatClient == null || _tools == null) throw new InvalidOperationException("Service not initialized properly.");

        _messages.Add(new ChatMessage(ChatRole.User, userInput));

        var responseBuilder = new StringBuilder();

        await foreach (var update in _chatClient.GetStreamingResponseAsync(
            _messages,
            new ChatOptions { Tools = [.. _tools] }))
        {
            if (update.Text != null)
            {
                responseBuilder.Append(update.Text);
            }
        }

        var response = responseBuilder.ToString();
        _messages.Add(new ChatMessage(ChatRole.Assistant, response));
        
        return response;
    }

    public async ValueTask DisposeAsync()
    {
        if (_mcpClient != null)
        {
            await _mcpClient.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }

    private static async Task<McpServerConfig?> LoadMcpConfigAsync(string mcpJsonPath)
    {
        try
        {
            if (!File.Exists(mcpJsonPath))
            {
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
                return null;
            }

            return config.Servers["mssql-mcp"];
        }
        catch
        {
            return null;
        }
    }

    private class McpJsonConfig
    {
        [System.Text.Json.Serialization.JsonPropertyName("servers")]
        public Dictionary<string, McpServerConfig>? Servers { get; set; }
    }

    private class McpServerConfig
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
