using System.Text.Json;

namespace McpDemo
{
  public class McpConfig
  {
    public static async Task<McpServerConfig?> LoadMcpConfigAsync(string mcpJsonPath)
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
    
  }

  /// <summary>
    /// MCP JSON configuration structure
    /// </summary>
    public class McpJsonConfig
    {
        [System.Text.Json.Serialization.JsonPropertyName("servers")]
        public Dictionary<string, McpServerConfig>? Servers { get; set; }
    }

    /// <summary>
    /// MCP Server configuration
    /// </summary>
    public class McpServerConfig
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