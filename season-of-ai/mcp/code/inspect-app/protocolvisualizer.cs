// ProtocolVisualizer.cs
using System.Text.Json;

namespace McpDemo
{
    public class ProtocolVisualizer
    {
        private static int _indent = 0;
        private static readonly object _lock = new object();

        public static void LogPhase(string phase, ConsoleColor color = ConsoleColor.White)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"\n{"═".PadLeft(80, '═')}");
                Console.WriteLine($"  {phase}");
                Console.WriteLine($"{"═".PadLeft(80, '═')}\n");
                Console.ResetColor();
            }
        }

        public static void LogRequest(string from, string to, string message, object? data = null)
        {
            lock (_lock)
            {
                var indent = new string(' ', _indent * 2);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{indent}📤 {from} → {to}: {message}");
                
                if (data != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                    foreach (var line in json.Split('\n'))
                    {
                        Console.WriteLine($"{indent}   {line}");
                    }
                }
                Console.ResetColor();
            }
        }

        public static void LogResponse(string from, string to, string message, object? data = null)
        {
            lock (_lock)
            {
                var indent = new string(' ', _indent * 2);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{indent}📥 {from} → {to}: {message}");
                
                if (data != null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                    foreach (var line in json.Split('\n'))
                    {
                        Console.WriteLine($"{indent}   {line}");
                    }
                }
                Console.ResetColor();
            }
        }

        public static void Indent() => _indent++;
        public static void Outdent() => _indent = Math.Max(0, _indent - 1);

        public static void LogError(string message, Exception? ex = null)
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ ERROR: {message}");
                if (ex != null)
                {
                    Console.WriteLine($"   {ex.Message}");
                }
                Console.ResetColor();
            }
        }
    }
}