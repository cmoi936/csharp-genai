# Changelog

All notable changes to the Google Gen AI SDK for C#/.NET will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-02

### Added
- Initial release of Google Gen AI SDK for C#/.NET
- Client initialization for Gemini Developer API and Vertex AI
- Generate content with text and multi-modal inputs
- System instructions and generation configuration support
- Configuration options:
  - Temperature
  - MaxOutputTokens
  - TopP
  - TopK
  - CandidateCount
  - ResponseMimeType
- Support for JSON response mode
- Support for images via URI references
- Both synchronous and asynchronous API methods
- Proper IDisposable implementation for resource management
- Comprehensive README documentation
- Getting Started guide
- Sample QuickStart console application
- .NET 8.0 target framework
- NuGet package configuration

### Dependencies
- Google.Apis.Auth (v1.68.0)
- System.Net.Http.Json (v8.0.1)
- System.Text.Json (v8.0.5)

### Known Limitations
This is the initial release and includes core functionality only. The following features from the Python SDK are planned for future releases:
- Streaming responses
- Chat sessions
- Function calling and automatic function calling
- File uploads and management
- Content caching
- Batch operations
- Token counting (remote and local)
- Embeddings
- Image generation (Imagen)
- Video generation (Veo)
- Tuning operations
- Safety settings
- Model listing

## [Unreleased]

### Planned Features
- Streaming support for generate content
- Chat session management
- Function calling capabilities
- File upload and management
- Content caching
- Token counting
- Embedding support
- Safety settings configuration
- Model information and listing
- Batch prediction operations
- More comprehensive examples
- Unit tests
- Integration tests
- XML documentation for IntelliSense
- NuGet package publication

---

[1.0.0]: https://github.com/googleapis/python-genai/releases/tag/csharp-v1.0.0
