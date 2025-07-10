# 🧠 MCP Client + Ollama Tool Calling Prototype

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
<!-- ![License](https://img.shields.io/badge/License-MIT-green) -->
![Ollama Model](https://img.shields.io/badge/Model-Llama3.1-yellow)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux-blue)

This project is a prototype that demonstrates how to build a **C# console-based AI assistant** using:

- 🧰 **MCP (Model Context Protocol)** for local tool calling
- 🦙 **Ollama** as a local LLM engine
- 💬 **Microsoft.Extensions.AI** for chat orchestration and tool integration
- ⚙️ **ToolServer (MyFirstMCP)** exposing callable functions like `echo` and `echo_in_reverse`

The assistant uses **local LLMs** that support tool calling (like `llama3.1`), and can automatically invoke functions via the MCP tool server to perform tasks.

---

## 🚀 What It Does

- Starts an **MCP tool server** (another `.NET` project) with local tools
- Connects with gitmcp to get context of the current repo
- Starts a **chat session** with a local model running via Ollama
- Accepts user input from the terminal
- LLM decides **what tool to call**
- Returns tool output or a natural response via streaming

---

## 🛠️ Tech Stack

| Component                 | Description |
|--------------------------|-------------|
| [.NET 9](https://dotnet.microsoft.com) | Main runtime |
| [OllamaSharp](https://github.com/ollama/ollama-dotnet) | .NET SDK for Ollama |
| [Microsoft.Extensions.AI](https://github.com/dotnet/semantic-kernel) | Experimental chat tooling for .NET |
| [Model Context Protocol (MCP)](https://github.com/modelcontext/protocol) | Tool calling protocol |
| [Ollama](https://ollama.com) | Local model runner with support for Mistral, LLaMA3, DeepSeek, etc |

---

## 🧪 Example Terminal Session

```bash
MCP Server tools: echo, echo_in_reverse
MyFirstMCPClient started. Type 'exit' to quit.
> hello world
The response from the tool call is:

"hello from c#: hello world"
````

---

## 🧠 What I Learned

This repo is part of my deep-dive into understanding:

- How to use **tool calling** in AI assistants via MCP
- How **LLMs can delegate tasks** to local programs and format responses
- Streaming chat and **message history management**
- How Ollama supports **function invocation locally**
- Try setting up a minimal production-ready **LLM tool ecosystem in C#**
- Adding better **context** to any AI modal

---

## 🧩 Project Structure

```text
mcp-explore/
│
├── MyFirstMCP/                # MCP tool server (runs echo tool)
│   └── ...                    # Contains implementation of tools
│
├── MyFirstMCPClient/          # Main console app that uses chat + tool calling
│   ├── Program.cs
│   └── ...
│
├── README.md
```

---

## 🧬 Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Ollama installed](https://ollama.com/download) and running locally
- A model pulled and ready to use (example: `ollama run llama3.1`)
- Optional: VSCode or Rider for development

---

## ⚙️ Run It

```bash
# 1. Start Ollama (if not already running)
ollama run llama3.1
 - also make sure the modal supports tools, such as llama3.1

# 2. Run the MCP tool server project, (you can skip this since the client will start this process)
cd MyFirstMCP
dotnet run

# 3. In another terminal, run the chat client
cd MyFirstMCPClient
dotnet run
```

---

## 🧩 Tools Implemented in MCP Server

| Tool Name         | Description                |
| ----------------- | -------------------------- |
| `echo`            | Repeats the message back   |
| `reverse_echo`    | Repeats message in reverse |

More tools can be added by extending the MCP project.
such as api requsts, fetching documents etc.
If using a multimodal ai even files and images can be fetched.
Tools can also be actions such as writing to a file I assume, I will try this soon.

---

## 🧭 Roadmap (Next Steps)

- [ ] Add more complex tools (math, file access, time)
- [ ] Web UI using Blazor or Minimal API
- [ ] Tool selection controls (manual/auto mode)
- [ ] Persist conversation history
- [ ] Experiment with `ToolMode.AnyRequired` vs `Auto`

---

## 📜 License

MIT — you're free to use, modify, and extend.

---

## 🙋‍♂️ Maintained by

**Naser Al-Asbahi**
Developer & student exploring full-stack dev, AI, Arabic language & Islamic thought.

> *"Learning by building. Understanding by breaking."*

---

```text

Let me know if you'd like:
- A badge section (e.g., .NET version, license, etc.)
- Automatic README generation from code comments
- A section for GitHub Actions / CI setup
- Screenshots or GIFs added to visually explain behavior

Would you like me to write a `.gitignore` and an initial commit command too?
```
