# Architecture Overview

This document describes the architecture of the Google Gen AI SDK for C#/.NET.

## Project Structure

```
csharp-genai/
├── src/
│   └── Google.GenAI/              # Main library project
│       ├── Client.cs              # Main client class
│       ├── Models/                # Model-related operations
│       │   └── ModelsClient.cs   # Generate content, etc.
│       └── Types/                 # Type definitions
│           ├── Content.cs         # Content and Part classes
│           ├── GenerateContentConfig.cs
│           ├── GenerateContentResponse.cs
│           └── HttpOptions.cs
├── samples/
│   └── QuickStart/                # Sample console application
│       └── Program.cs
├── Google.GenAI.sln               # Solution file
└── README.md                      # Documentation
```

## Core Components

### 1. GeminiChatClient Class

The `Client` class is the main entry point for the SDK. It:
- Manages HTTP connections
- Supports both Gemini Developer API and Vertex AI
- Provides access to various service modules (Models, Chats, Files, etc.)
- Implements `IDisposable` for proper resource cleanup

```
Client
├── Constructor (API Key for Gemini)
├── Constructor (Vertex AI with Project ID and Location)
├── Models (ModelsClient)
├── Dispose()
└── (Future: Chats, Files, Caches, Tunings, Batches)
```

### 2. ModelsGeminiChatClient Class

Handles all model-related operations:
- Generate content (text, multi-modal)
- (Future: Streaming, token counting, embeddings)

### 3. Type System

The SDK uses a strongly-typed approach:

```
Types/
├── Content                    # Container for user/model messages
│   ├── Role (user/model)
│   └── Parts (List<Part>)
│
├── Part                       # Individual content piece
│   ├── Text
│   ├── InlineData (images)
│   └── FileData (URI references)
│
├── GenerateContentConfig      # Configuration options
│   ├── Temperature
│   ├── MaxOutputTokens
│   ├── TopP, TopK
│   ├── SystemInstruction
│   └── ResponseMimeType
│
└── GenerateContentResponse    # API response
    ├── Candidates
    ├── PromptFeedback
    └── Text (convenience property)
```

## API Flow

### Generate Content Flow

```
┌──────────────┐
│ User Code    │
└──────┬───────┘
       │ GenerateContentAsync()
       │
┌──────▼───────────┐
│ ModelsClient     │
│ - Build URL      │
│ - Build Request  │
│ - Convert Types  │
└──────┬───────────┘
       │ HTTP POST
       │
┌──────▼───────────────────┐
│ API Endpoint             │
│ (Gemini or Vertex AI)    │
└──────┬───────────────────┘
       │ JSON Response
       │
┌──────▼───────────┐
│ ModelsClient     │
│ - Parse Response │
│ - Create Types   │
└──────┬───────────┘
       │
┌──────▼───────────┐
│ User Code        │
│ (Response Object)│
└──────────────────┘
```

## API Endpoint Selection

The SDK supports two API endpoints:

### Gemini Developer API
- Base URL: `https://generativelanguage.googleapis.com/v1beta/`
- Authentication: API key (query parameter or header)
- Simpler setup for developers
- Usage: `new GeminiChatClient(apiKey: "YOUR_KEY")`

### Vertex AI
- Base URL: `https://{location}-aiplatform.googleapis.com/v1/`
- Authentication: Google Cloud authentication
- Enterprise features
- Usage: `new GeminiChatClient(vertexAi: true, projectId: "...", location: "...")`

## Design Principles

1. **Strongly Typed**: Use C# types for compile-time safety
2. **Async First**: Primary API is async, with sync wrappers
3. **Disposable**: Proper resource management with IDisposable
4. **Flexible Input**: Accept strings, Content objects, or lists
5. **Minimal Dependencies**: Only essential NuGet packages
6. **Cross-Platform**: Works on Windows, Linux, and macOS

## Type Conversion

The SDK performs automatic type conversions:

### Input Conversion
```
string              → Content (role: user, text part)
Content             → Content (as-is)
List<string>        → Content (role: user, multiple text parts)
List<Content>       → List<Content> (as-is)
```

### Response Parsing
```
JSON Response → GenerateContentResponse
              ├── Candidates[]
              │   ├── Content
              │   ├── FinishReason
              │   └── SafetyRatings[]
              └── PromptFeedback
```

## Future Enhancements

### Planned Additions

1. **Streaming Support**
   - `GenerateContentStreamAsync()` returning `IAsyncEnumerable<GenerateContentResponse>`

2. **Chat Sessions**
   - `ChatsClient` for multi-turn conversations
   - History management

3. **Function Calling**
   - Automatic function declaration
   - Tool execution
   - Multi-turn function calling

4. **File Management**
   - `FilesClient` for file uploads
   - Support for local and cloud files

5. **Caching**
   - `CachesClient` for content caching
   - Cost optimization

6. **Additional Operations**
   - Token counting
   - Embeddings
   - Batch predictions
   - Tuning jobs

## Technology Stack

- **Language**: C# 12
- **Framework**: .NET 8.0
- **HTTP Client**: System.Net.Http
- **JSON Serialization**: System.Text.Json
- **Authentication**: Google.Apis.Auth

## Error Handling

The SDK uses standard .NET exception handling:
- `HttpRequestException` for network errors
- `JsonException` for parsing errors
- `ArgumentException` for invalid parameters
- `InvalidOperationException` for unexpected states

## Thread Safety

- The `Client` class is thread-safe for concurrent requests
- HttpClient is reused across requests (recommended pattern)
- No shared mutable state between requests

## Performance Considerations

1. **Connection Pooling**: HttpClient reuses connections
2. **Async I/O**: All network operations are async
3. **Minimal Allocations**: Efficient JSON serialization
4. **Lazy Initialization**: Services created on-demand

## Testing Strategy (Planned)

1. **Unit Tests**: Test type conversions and logic
2. **Integration Tests**: Test against real API
3. **Mock Tests**: Test with mocked HTTP responses
4. **Sample Tests**: Ensure examples work correctly

## Comparison with Python SDK

The C# SDK mirrors the Python SDK's API design:

| Python | C# |
|--------|-----|
| `client = genai.Client(api_key=...)` | `var client = new GeminiChatClient(apiKey: ...)` |
| `client.models.generate_content()` | `client.Models.GenerateContentAsync()` |
| `types.Content(...)` | `new Content { ... }` |
| `types.GenerateContentConfig(...)` | `new GenerateContentConfig { ... }` |

Key differences:
- C# uses async/await for async operations
- C# uses properties with PascalCase naming
- C# has explicit type annotations
- C# uses IDisposable pattern for cleanup
