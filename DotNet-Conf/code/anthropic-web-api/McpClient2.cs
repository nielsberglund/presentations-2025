using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Anthropic.SDK;
using Anthropic.SDK.Common;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

public class McpClient2 : IAsyncDisposable
{
    private AnthropicClient? _anthropicClient;
    private McpClient? _mcpClient;
    private IList<McpClientTool>? _mcpTools;
    private List<Anthropic.SDK.Common.Tool>? _anthropicTools;
    private readonly List<Message> _messages = new();
    private bool _isInitialized = false;

    private const string ApiKey = "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";
    private const string ModelId = "claude-sonnet-4-5-20250929";
    private const string SystemPrompt = "You are a helpful AI assistant with access to a SQL Server database. " +
                "Use the available tools to help users query and understand their data. " +
                "When you need to query the database, use the provided tools. " +
                "Always explain your results in a clear and user-friendly way.";

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        // Get path to the mcp.json configuration file
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

        // Create Anthropic client
        _anthropicClient = new AnthropicClient(ApiKey);

        // Create MCP client using the SDK
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
        _mcpTools = await _mcpClient.ListToolsAsync();

        // Convert MCP tools to Anthropic tools
        _anthropicTools = ConvertMcpToolsToAnthropic(_mcpTools);

        _isInitialized = true;
    }

    public async Task<string> ChatAsync(string userInput)
    {
        if (!_isInitialized) await InitializeAsync();
        if (_anthropicClient == null || _mcpTools == null || _anthropicTools == null) 
            throw new InvalidOperationException("Service not initialized properly.");

        _messages.Add(new Message
        {
            Role = RoleType.User,
            Content = new List<ContentBase> { new Anthropic.SDK.Messaging.TextContent { Text = userInput } }
        });

        var response = await GetClaudeResponseWithTools(
            _anthropicClient,
            ModelId,
            SystemPrompt,
            _messages,
            _anthropicTools,
            _mcpTools
        );

        _messages.Add(new Message
        {
            Role = RoleType.Assistant,
            Content = new List<ContentBase> { new Anthropic.SDK.Messaging.TextContent { Text = response } }
        });

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

    private async Task<string> GetClaudeResponseWithTools(
        AnthropicClient client,
        string modelId,
        string systemPrompt,
        List<Message> messages,
        List<Anthropic.SDK.Common.Tool> tools,
        IList<McpClientTool> mcpTools)
    {
        var parameters = new MessageParameters
        {
            Model = modelId,
            MaxTokens = 4096,
            System = new List<SystemMessage> { new SystemMessage(systemPrompt) },
            Messages = messages,
            Tools = tools
        };

        var response = await client.Messages.GetClaudeMessageAsync(parameters);

        // Handle tool use loop
        while (response.StopReason == "tool_use")
        {
            var toolUseBlocks = response.Content.OfType<ToolUseContent>().ToList();
            var toolResults = new List<ToolResultContent>();

            foreach (var toolUse in toolUseBlocks)
            {
                var toolName = toolUse.Name;
                var toolInput = toolUse.Input;

                // Find the MCP tool and invoke it
                var mcpTool = mcpTools.FirstOrDefault(t => t.Name == toolName);
                if (mcpTool != null)
                {
                    try
                    {
                        // Convert input to AIFunctionArguments for MCP tool invocation
                        var arguments = new AIFunctionArguments();
                        var inputJson = JsonSerializer.Serialize(toolInput);
                        var inputElement = JsonDocument.Parse(inputJson).RootElement;
                        if (inputElement.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var prop in inputElement.EnumerateObject())
                            {
                                arguments[prop.Name] = prop.Value.ValueKind switch
                                {
                                    JsonValueKind.String => prop.Value.GetString(),
                                    JsonValueKind.Number => prop.Value.GetDouble(),
                                    JsonValueKind.True => true,
                                    JsonValueKind.False => false,
                                    _ => prop.Value.ToString()
                                };
                            }
                        }

                        var result = await mcpTool.InvokeAsync(arguments);
                        var resultText = result?.ToString() ?? "No result";

                        toolResults.Add(new ToolResultContent
                        {
                            ToolUseId = toolUse.Id,
                            Content = new List<ContentBase> { new Anthropic.SDK.Messaging.TextContent { Text = resultText } }
                        });
                    }
                    catch (Exception ex)
                    {
                        toolResults.Add(new ToolResultContent
                        {
                            ToolUseId = toolUse.Id,
                            Content = new List<ContentBase> { new Anthropic.SDK.Messaging.TextContent { Text = $"Error: {ex.Message}" } },
                            IsError = true
                        });
                    }
                }
                else
                {
                    toolResults.Add(new ToolResultContent
                    {
                        ToolUseId = toolUse.Id,
                        Content = new List<ContentBase> { new Anthropic.SDK.Messaging.TextContent { Text = $"Tool '{toolName}' not found" } },
                        IsError = true
                    });
                }
            }

            // Add assistant's tool use response and tool results to messages
            messages.Add(new Message
            {
                Role = RoleType.Assistant,
                Content = response.Content.Select(c => (ContentBase)c).ToList()
            });

            messages.Add(new Message
            {
                Role = RoleType.User,
                Content = toolResults.Select(tr => (ContentBase)tr).ToList()
            });

            // Get next response from Claude
            parameters.Messages = messages;
            response = await client.Messages.GetClaudeMessageAsync(parameters);
        }

        // Extract and return text response
        var textContent = response.Content.OfType<Anthropic.SDK.Messaging.TextContent>().FirstOrDefault();
        return textContent?.Text ?? "No response";
    }

    private List<Anthropic.SDK.Common.Tool> ConvertMcpToolsToAnthropic(IList<McpClientTool> mcpTools)
    {
        var tools = new List<Anthropic.SDK.Common.Tool>();

        foreach (var mcpTool in mcpTools)
        {
            var inputSchema = new InputSchema
            {
                Type = "object",
                Properties = new Dictionary<string, Property>(),
                Required = new List<string>()
            };

            // Convert JSON schema to Anthropic InputSchema
            var schemaElement = mcpTool.JsonSchema;
            if (schemaElement.ValueKind == JsonValueKind.Object)
            {
                // Extract properties
                if (schemaElement.TryGetProperty("properties", out var propertiesElement) &&
                    propertiesElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in propertiesElement.EnumerateObject())
                    {
                        var propType = "string";
                        var propDesc = "";

                        if (prop.Value.TryGetProperty("type", out var typeElement))
                        {
                            propType = typeElement.GetString() ?? "string";
                        }
                        if (prop.Value.TryGetProperty("description", out var descElement))
                        {
                            propDesc = descElement.GetString() ?? "";
                        }

                        inputSchema.Properties[prop.Name] = new Property
                        {
                            Type = propType,
                            Description = propDesc
                        };
                    }
                }

                // Extract required fields
                if (schemaElement.TryGetProperty("required", out var requiredElement) &&
                    requiredElement.ValueKind == JsonValueKind.Array)
                {
                    inputSchema.Required = requiredElement.EnumerateArray()
                        .Select(r => r.GetString() ?? "")
                        .Where(r => !string.IsNullOrEmpty(r))
                        .ToList();
                }
            }

            // Create tool manually since Tool is a sealed class and we need to set the schema
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
            };
            var schemaJson = JsonSerializer.Serialize(inputSchema, jsonOptions);
            var parametersNode = JsonNode.Parse(schemaJson);

            var function = new Anthropic.SDK.Common.Function(
                mcpTool.Name, 
                mcpTool.Description ?? "", 
                parametersNode
            );

            var tool = new Anthropic.SDK.Common.Tool(function);
            tools.Add(tool);
        }

        return tools;
    }

    private async Task<McpServerConfig?> LoadMcpConfigAsync(string mcpJsonPath)
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
