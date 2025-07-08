using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
   consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services
      .AddMcpServer()
      .WithStdioServerTransport()
      .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
   [McpServerTool, Description("Echoes the input back to the client.")]
   public static string Echo(string message)
   {
      Console.WriteLine($"Echoing: {message}");
      return $"hello from c#: {message}";
   }

   [McpServerTool, Description("Echoes the input back to the client in reverse.")]
   public static string EchoInReverse(string message)
   {
      string reversed = new string(message.Reverse().ToArray());
      Console.WriteLine($"Echoing in reverse: {reversed}");
      return reversed;
   }

}
