using OllamaSharp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Linq;

var builder = Host.CreateApplicationBuilder(args);

// Configuration & Logging
builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

builder.Logging.AddConsole();

// Set up Ollama client
var ollama = new OllamaApiClient(new Uri(builder.Configuration["OLLAMA_HOST"] ?? "http://localhost:11434"));
ollama.SelectedModel = builder.Configuration["OLLAMA_MODEL"] ?? "llama3.1";

builder.Services.AddSingleton<IChatClient>(sp =>
    new ChatClientBuilder(ollama)
        .UseFunctionInvocation()
        .Build());

// Set up MCP tool server (stdio transport)
var (cmd, cmdArgs) = ("dotnet", new[] { "run", "--project", "C:\\Users\\nasse\\Desktop\\code\\mcp-explore\\MyFirstMCP\\MyFirstMCP.csproj" });
var transport = new StdioClientTransport(new() { Name = "MyMCP Server", Command = cmd, Arguments = cmdArgs });

await using var mcpClient = await McpClientFactory.CreateAsync(transport);
var tools = (await mcpClient.ListToolsAsync()).ToArray();
Console.WriteLine($"MCP Server tools: {string.Join(", ", tools.Select(t => t.Name))}");

// Get chat client
using var chatClient = builder.Services.BuildServiceProvider().GetRequiredService<IChatClient>();

// Initialize message list with system prompt
var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant that can answer questions and should always invoke tools for this prototy.")
};

Console.WriteLine("MyFirstMCPClient started. Type 'exit' to quit.");
Prompt();

while (Console.ReadLine() is string input && !input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
{
   if (!string.IsNullOrWhiteSpace(input))
   {
      messages.Add(new ChatMessage(ChatRole.User, input));

      string assistantReply = "";
      await foreach (var chunk in chatClient.GetStreamingResponseAsync(messages, new ChatOptions
      {
         MaxOutputTokens = 1000,
         Tools = tools,
         ToolMode = ChatToolMode.Auto
      }))
      {
         Console.Write(chunk);
         assistantReply += chunk;
      }

      messages.Add(new ChatMessage(ChatRole.Assistant, assistantReply));
      Console.WriteLine();
   }

   Prompt();
}

static void Prompt() => Console.Write("> ");
