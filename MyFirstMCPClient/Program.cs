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
using Microsoft.VisualBasic;

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
var stdioClientTransport = new StdioClientTransport(new() { Name = "MyMCP Server", Command = cmd, Arguments = cmdArgs });
// Set up MCP tool server (SSE transport)
var sseTransport = new SseClientTransport(new SseClientTransportOptions() { Endpoint = new Uri("https://gitmcp.io/n4sser77/mcp-explore") });

await using var stdioMcpClient = await McpClientFactory.CreateAsync(stdioClientTransport);
await using var sseMcpClient = await McpClientFactory.CreateAsync(sseTransport);


MyGlobalToolStore.Tools = (await stdioMcpClient.ListToolsAsync()).ToList();
// MyGlobalToolStore.Tools.Add( new GitMcpConnector()); // Add the GitMcpConnector tool
MyGlobalToolStore.Tools = MyGlobalToolStore.Tools.Concat(await sseMcpClient.ListToolsAsync()).ToList();

Console.WriteLine($"MCP Server tools: {string.Join(", ", MyGlobalToolStore.Tools.Select(t => t.Name))}");

// Get chat client
using var chatClient = builder.Services.BuildServiceProvider().GetRequiredService<IChatClient>();

var systemPrompt = """
You are a helpful assistant running in a terminal test environment.

You have access to tools and must use them to answer questions and perform tasks. 
This is a prototype app — your main goal is to test that the tools work properly.

Always follow these rules:
- Use the available tools. Don’t guess or make up answers.
- If you're unsure how to use a tool, check its description.
- When you call a tool, display the **exact output** it returns.
- Extract what’s relevant from the tool's output `content`, and show it clearly.
- Format your response for **beautiful, easy-to-read terminal output** — use spacing, line breaks, and clear labels.

Keep your responses clean, helpful, and easy to follow. This is a debugging-friendly assistant, not a chatbot.
""";

// Initialize message list with system prompt
var messages = new List<ChatMessage>
{
    new ChatMessage(ChatRole.System, systemPrompt),
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
         Tools = MyGlobalToolStore.Tools.ToArray(),
         ToolMode = ChatToolMode.RequireAny,
         AllowMultipleToolCalls = true,


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





static class MyGlobalToolStore
{
   public static List<McpClientTool> Tools { get; set; } = new List<McpClientTool>();
}