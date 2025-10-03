// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Runtime.CompilerServices;
using Google.GenAI.Models;
using Google.GenAI.Types;
using Microsoft.Extensions.AI;

namespace Google.GenAI
{
    /// <summary>
    /// Client for making requests to Google's Generative AI services.
    /// Supports both Gemini Developer API and Vertex AI.
    /// Implements the Microsoft.Extensions.AI.IChatClient interface.
    /// </summary>
    public class GeminiChatClient : IChatClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly bool _vertexAi;
        private readonly string? _projectId;
        private readonly string? _location;
        private readonly ModelsClient _models;
        private readonly string _defaultModelId;
        private bool _disposed;

        /// <summary>
        /// Gets the Models client for model-related operations.
        /// </summary>
        public ModelsClient Models => _models;

        /// <summary>
        /// Gets metadata that describes the IChatClient.
        /// </summary>
        public ChatClientMetadata Metadata { get; }

        /// <summary>
        /// Initializes a new instance of the GeminiChatClient class for Gemini Developer API.
        /// </summary>
        /// <param name="apiKey">The API key for authentication. If null, will attempt to read from environment variable GEMINI_API_KEY or GOOGLE_API_KEY.</param>
        /// <param name="model">The model ID to use (e.g., "gemini-2.0-flash-001").</param>
        /// <param name="httpOptions">Optional HTTP configuration options.</param>
        public GeminiChatClient(string? apiKey, string model, HttpOptions? httpOptions = null)
        {
            _apiKey = apiKey ?? GetApiKeyFromEnvironment();
            _vertexAi = false;
            _defaultModelId = model ?? throw new ArgumentNullException(nameof(model));
            _httpClient = CreateHttpClient(httpOptions);
            _models = new ModelsClient(this, _httpClient, _apiKey, _vertexAi, null, null, _defaultModelId);
            Metadata = new ChatClientMetadata(
                providerName: "Google Gemini",
                providerUri: new Uri("https://generativelanguage.googleapis.com"),
                modelId: _defaultModelId
            );
        }

        /// <summary>
        /// Initializes a new instance of the GeminiChatClient class for Vertex AI.
        /// </summary>
        /// <param name="vertexAi">Set to true to use Vertex AI.</param>
        /// <param name="projectId">The Google Cloud project ID.</param>
        /// <param name="location">The location/region for Vertex AI (e.g., "us-central1").</param>
        /// <param name="model">The model ID to use (e.g., "gemini-2.0-flash-001").</param>
        /// <param name="httpOptions">Optional HTTP configuration options.</param>
        public GeminiChatClient(bool vertexAi, string projectId, string location, string model, HttpOptions? httpOptions = null)
        {
            if (!vertexAi)
                throw new ArgumentException("When using this constructor, vertexAi must be true");

            _vertexAi = true;
            _projectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
            _location = location ?? throw new ArgumentNullException(nameof(location));
            _defaultModelId = model ?? throw new ArgumentNullException(nameof(model));
            _httpClient = CreateHttpClient(httpOptions);
            _models = new ModelsClient(this, _httpClient, null, _vertexAi, _projectId, _location, _defaultModelId);
            Metadata = new ChatClientMetadata(
                providerName: "Google Vertex AI",
                providerUri: new Uri($"https://{_location}-aiplatform.googleapis.com"),
                modelId: _defaultModelId
            );
        }

        private static string? GetApiKeyFromEnvironment()
        {
            return Environment.GetEnvironmentVariable("GOOGLE_API_KEY")
                   ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        }

        private static HttpClient CreateHttpClient(HttpOptions? httpOptions)
        {
            var handler = new HttpClientHandler();
            var client = new HttpClient(handler)
            {
                Timeout = httpOptions?.Timeout ?? TimeSpan.FromMinutes(2)
            };

            return client;
        }

        /// <summary>
        /// Sends chat messages to the model and returns the response messages.
        /// </summary>
        /// <param name="chatMessages">The chat content to send.</param>
        /// <param name="options">The chat options to configure the request.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <returns>The response messages generated by the client.</returns>
        public async Task<ChatCompletion> CompleteAsync(
            IList<ChatMessage> chatMessages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            // Convert ChatMessage to our Content format
            var contents = ConvertChatMessagesToContents(chatMessages);

            // Build config from ChatOptions
            var config = BuildGenerateContentConfig(options);

            // Call our existing generate content method (uses the default model from constructor)
            var response = await _models.GenerateContentAsync(contents, config, cancellationToken);

            // Convert response to ChatCompletion
            return ConvertToChatCompletion(response, _defaultModelId);
        }

        /// <summary>
        /// Sends chat messages to the model and streams the response messages.
        /// </summary>
        /// <param name="chatMessages">The chat content to send.</param>
        /// <param name="options">The chat options to configure the request.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <returns>An async enumerable of streaming response messages.</returns>
        public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
            IList<ChatMessage> chatMessages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // For now, return a single completion as a stream
            // Full streaming support can be added later
            var completion = await CompleteAsync(chatMessages, options, cancellationToken);

            yield return new StreamingChatCompletionUpdate
            {
                CompletionId = completion.CompletionId,
                Contents = completion.Message.Contents,
                Role = completion.Message.Role,
                FinishReason = completion.FinishReason
            };
        }

        /// <summary>
        /// Asks the IChatClient for an object of type TService.
        /// </summary>
        /// <typeparam name="TService">The type of the object to be retrieved.</typeparam>
        /// <param name="key">An optional key that may be used to help identify the target service.</param>
        /// <returns>The found object, otherwise null.</returns>
        public TService? GetService<TService>(object? key = null) where TService : class
        {
            if (typeof(TService) == typeof(GeminiChatClient))
            {
                return this as TService;
            }

            return null;
        }

        private List<Content> ConvertChatMessagesToContents(IList<ChatMessage> chatMessages)
        {
            var contents = new List<Content>();

            foreach (var message in chatMessages)
            {
                var content = new Content
                {
                    Role = message.Role.Value.ToLowerInvariant(),
                    Parts = new List<Part>()
                };

                foreach (var item in message.Contents)
                {
                    if (item is TextContent textContent)
                    {
                        content.Parts.Add(Part.FromText(textContent.Text));
                    }
                    else if (item is ImageContent imageContent && imageContent.Uri != null)
                    {
                        content.Parts.Add(Part.FromUri(
                            imageContent.Uri.ToString(),
                            imageContent.MediaType ?? "image/jpeg"
                        ));
                    }
                }

                contents.Add(content);
            }

            return contents;
        }

        private GenerateContentConfig? BuildGenerateContentConfig(ChatOptions? options)
        {
            if (options == null)
                return null;

            return new GenerateContentConfig
            {
                Temperature = options.Temperature,
                MaxOutputTokens = options.MaxOutputTokens,
                TopP = options.TopP,
                ResponseMimeType = options.ResponseFormat == ChatResponseFormat.Json
                    ? "application/json"
                    : null
            };
        }

        private ChatCompletion ConvertToChatCompletion(GenerateContentResponse response, string modelId)
        {
            var chatMessage = new ChatMessage
            {
                Role = ChatRole.Assistant,
                Contents = new List<AIContent>()
            };

            if (response.Candidates.Count > 0 && response.Candidates[0].Content != null)
            {
                var candidate = response.Candidates[0];
                var content = candidate.Content;
                if (content != null)
                {
                    foreach (var part in content.Parts)
                    {
                        if (part.Text != null)
                        {
                            chatMessage.Contents.Add(new TextContent(part.Text));
                        }
                    }
                }
            }

            var completion = new ChatCompletion(chatMessage)
            {
                ModelId = modelId,
                FinishReason = response.Candidates.FirstOrDefault()?.FinishReason switch
                {
                    "STOP" => ChatFinishReason.Stop,
                    "MAX_TOKENS" => ChatFinishReason.Length,
                    _ => null
                }
            };

            return completion;
        }

        /// <summary>
        /// Disposes the client and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Backward compatibility alias for GeminiChatClient.
    /// Use GeminiChatClient for new code.
    /// </summary>
    [Obsolete("Use GeminiChatClient instead. This alias will be removed in a future version.")]
    public class Client : GeminiChatClient
    {
        /// <inheritdoc/>
        public Client(string? apiKey = null, string? modelId = null, HttpOptions? httpOptions = null)
            : base(apiKey, modelId ?? "gemini-2.0-flash-001", httpOptions)
        {
        }

        /// <inheritdoc/>
        public Client(bool vertexAi, string projectId, string location, string? modelId = null, HttpOptions? httpOptions = null)
            : base(vertexAi, projectId, location, modelId ?? "gemini-2.0-flash-001", httpOptions)
        {
        }
    }
}
