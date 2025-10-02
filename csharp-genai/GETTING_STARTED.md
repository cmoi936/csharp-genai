# Getting Started with Google Gen AI SDK for C#/.NET

This guide will help you get started with the Google Gen AI SDK for C#/.NET.

## Prerequisites

- .NET 8.0 SDK or later
- A Google Cloud Platform account (for Vertex AI) or a Gemini API key (for Gemini Developer API)

## Installation

### Option 1: Build from Source

1. Clone the repository:
```bash
git clone https://github.com/googleapis/python-genai.git
cd python-genai/csharp-genai
```

2. Build the solution:
```bash
dotnet build
```

3. Add a reference to the library in your project:
```bash
dotnet add reference path/to/Google.GenAI.csproj
```

### Option 2: Use NuGet (Coming Soon)

```bash
dotnet add package Google.GenAI
```

## Quick Start

### 1. Get Your API Key

#### For Gemini Developer API:

1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Create a new API key
3. Set it as an environment variable:

```bash
export GEMINI_API_KEY='your-api-key-here'
```

#### For Vertex AI:

1. Set up a Google Cloud Platform project
2. Enable the Vertex AI API
3. Set up authentication (e.g., using a service account)
4. Note your project ID and preferred location (e.g., us-central1)

### 2. Create Your First Application

Create a new console application:

```bash
dotnet new console -n MyGenAIApp
cd MyGenAIApp
dotnet add reference path/to/Google.GenAI.csproj
```

Edit `Program.cs`:

```csharp
using Google.GenAI;
using Google.GenAI.Types;

// Create a client
using var client = new Client();

// Generate content
var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Explain quantum computing in simple terms"
);

Console.WriteLine(response.Text);
```

Run your application:

```bash
dotnet run
```

## Examples

### Basic Text Generation

```csharp
using Google.GenAI;

using var client = new Client(apiKey: "your-api-key");

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Write a haiku about programming"
);

Console.WriteLine(response.Text);
```

### Using Configuration Options

```csharp
using Google.GenAI;
using Google.GenAI.Types;

using var client = new Client();

var config = new GenerateContentConfig
{
    Temperature = 0.9,          // Higher = more creative
    MaxOutputTokens = 256,      // Maximum length
    TopP = 0.95,                // Nucleus sampling
    TopK = 40,                  // Top-k sampling
    SystemInstruction = "You are a creative writer."
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Write a short story about a robot",
    config: config
);

Console.WriteLine(response.Text);
```

### Multi-Part Content (Text + Image)

```csharp
using Google.GenAI;
using Google.GenAI.Types;

using var client = new Client();

var content = new Content
{
    Role = "user",
    Parts = new List<Part>
    {
        Part.FromText("What's in this image?"),
        Part.FromUri(
            fileUri: "gs://your-bucket/image.jpg",
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

### JSON Output

```csharp
using Google.GenAI;
using Google.GenAI.Types;

using var client = new Client();

var config = new GenerateContentConfig
{
    ResponseMimeType = "application/json"
};

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "List 5 planets with their distance from the sun",
    config: config
);

Console.WriteLine(response.Text);
```

### Synchronous API

If you prefer synchronous code:

```csharp
using Google.GenAI;

using var client = new Client();

// Synchronous call
var response = client.Models.GenerateContent(
    model: "gemini-2.0-flash-001",
    contents: "Hello, Gemini!"
);

Console.WriteLine(response.Text);
```

### Using Vertex AI

```csharp
using Google.GenAI;

using var client = new Client(
    vertexAi: true,
    projectId: "your-gcp-project-id",
    location: "us-central1"
);

var response = await client.Models.GenerateContentAsync(
    model: "gemini-2.0-flash-001",
    contents: "Hello from Vertex AI!"
);

Console.WriteLine(response.Text);
```

## Common Patterns

### Using Statements for Resource Management

Always use `using` statements or manually dispose the client:

```csharp
using (var client = new Client())
{
    var response = await client.Models.GenerateContentAsync(
        model: "gemini-2.0-flash-001",
        contents: "Hello!"
    );
    Console.WriteLine(response.Text);
}
// Client is automatically disposed here
```

### Error Handling

```csharp
using Google.GenAI;

try
{
    using var client = new Client();
    var response = await client.Models.GenerateContentAsync(
        model: "invalid-model-name",
        contents: "Hello!"
    );
    Console.WriteLine(response.Text);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Request failed: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Running the Sample

The SDK includes a sample application in `samples/QuickStart`:

```bash
cd samples/QuickStart
export GEMINI_API_KEY='your-api-key'
dotnet run
```

## Next Steps

- Read the [full documentation](README.md)
- Explore the [sample code](samples/QuickStart/Program.cs)
- Check out the [Python SDK documentation](../README.md) for API design reference
- Visit [Google AI Studio](https://makersuite.google.com/) for more information about the Gemini API

## Supported Models

The SDK works with all Gemini models, including:

- `gemini-2.0-flash-001` - Fast and efficient
- `gemini-1.5-pro` - Most capable model
- `gemini-1.5-flash` - Fast performance
- And more...

Check the [Gemini API documentation](https://ai.google.dev/gemini-api/docs/models) for the full list of available models.

## Need Help?

- File issues on [GitHub](https://github.com/googleapis/python-genai/issues)
- Check the [API documentation](https://ai.google.dev/gemini-api/docs)
- Visit [Google Cloud documentation](https://cloud.google.com/vertex-ai/docs) for Vertex AI

## License

Apache 2.0 - See [LICENSE](../LICENSE) for more information.
