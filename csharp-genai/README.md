# Google Gen AI SDK for C#/.NET

[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue)](https://www.nuget.org/)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

--------
**Documentation:** https://googleapis.github.io/python-genai/

-----

Google Gen AI .NET SDK provides an interface for developers to integrate
Google's generative models into their C#/.NET applications. It supports the
[Gemini Developer API](https://ai.google.dev/gemini-api/docs) and
[Vertex AI](https://cloud.google.com/vertex-ai/generative-ai/docs/learn/overview)
APIs.

## Installation

Using .NET CLI:

```sh
dotnet add package Google.GenAI
```

Using Package Manager:

```powershell
Install-Package Google.GenAI
```

## Quick Start

### Using Gemini Developer API

```csharp
using Google.GenAI;
using Google.GenAI.Types;

// Create a client with API key
var client = new Client(apiKey: "YOUR_GEMINI_API_KEY");

// Generate content
var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Why is the sky blue?"
);

Console.WriteLine(response.Text);
```

### Using Vertex AI

```csharp
using Google.GenAI;
using Google.GenAI.Types;

// Create a client for Vertex AI
var client = new Client(
    vertexAi: true,
    projectId: "your-project-id",
    location: "us-central1"
);

// Generate content
var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Tell me a story"
);

Console.WriteLine(response.Text);
```

### Using Environment Variables

Set the `GEMINI_API_KEY` or `GOOGLE_API_KEY` environment variable:

```bash
export GEMINI_API_KEY='your-api-key'
```

Then create a client without explicitly passing the API key:

```csharp
var client = new Client();
```

## Advanced Usage

### System Instructions and Configuration

```csharp
using Google.GenAI;
using Google.GenAI.Types;

var client = new Client();

var config = new GenerateContentConfig
{
    SystemInstruction = "I say high, you say low",
    MaxOutputTokens = 100,
    Temperature = 0.3,
    TopP = 0.95,
    TopK = 20
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "high",
    config: config
);

Console.WriteLine(response.Text);
```

### Working with Images

```csharp
using Google.GenAI;
using Google.GenAI.Types;

var client = new Client();

var content = new Content
{
    Role = "user",
    Parts = new List<Part>
    {
        Part.FromText("What is in this image?"),
        Part.FromUri(
            fileUri: "gs://generativeai-downloads/images/scones.jpg",
            mimeType: "image/jpeg"
        )
    }
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: content
);

Console.WriteLine(response.Text);
```

### JSON Response Mode

```csharp
using Google.GenAI;
using Google.GenAI.Types;

var client = new Client();

var config = new GenerateContentConfig
{
    ResponseMimeType = "application/json"
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Give me a list of 3 popular programming languages",
    config: config
);

Console.WriteLine(response.Text);
```

### Synchronous API

All async methods have synchronous counterparts:

```csharp
using Google.GenAI;

var client = new Client();

// Synchronous call
var response = client.Models.GenerateContent(
    model: "gemini-2.0-flash-001",
    contents: "Hello, world!"
);

Console.WriteLine(response.Text);
```

## Disposing the Client

The client implements `IDisposable` and should be disposed when no longer needed:

```csharp
using (var client = new Client())
{
    var response = await client.Models.GenerateContentAsync(
        model: "gemini-2.0-flash-001",
        contents: "Hello!"
    );
    Console.WriteLine(response.Text);
}
```

Or manually:

```csharp
var client = new Client();
try
{
    var response = await client.Models.GenerateContentAsync(
        model: "gemini-2.0-flash-001",
        contents: "Hello!"
    );
    Console.WriteLine(response.Text);
}
finally
{
    client.Dispose();
}
```

## Building from Source

```sh
cd csharp-genai/src/Google.GenAI
dotnet build
```

## Running Tests

```sh
cd csharp-genai/src/Google.GenAI
dotnet test
```

## Features

This initial version of the C# SDK includes:

- ✅ Client initialization for Gemini Developer API and Vertex AI
- ✅ Generate content (text and multi-modal)
- ✅ System instructions and generation configuration
- ✅ Support for temperature, top-p, top-k, and other parameters
- ✅ JSON response mode
- ✅ Both synchronous and asynchronous APIs
- ✅ Proper resource disposal

### Coming Soon

- 🔄 Streaming responses
- 🔄 Chat sessions
- 🔄 Function calling
- 🔄 File uploads
- 🔄 Caching
- 🔄 Batch operations
- 🔄 Token counting
- 🔄 Embeddings
- 🔄 Image generation (Imagen)
- 🔄 Video generation (Veo)
- 🔄 Tuning operations

## License

Apache 2.0 - See [LICENSE](../LICENSE) for more information.

## Contributing

See [CONTRIBUTING.md](../CONTRIBUTING.md) for details.

## Support

For issues and questions, please file an issue on the [GitHub repository](https://github.com/googleapis/python-genai/issues).
