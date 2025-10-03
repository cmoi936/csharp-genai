using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;

namespace Google.GenAI
{
    /// <summary>
    /// Client for making requests to Google's Generative AI services.
    /// Supports both Gemini Developer API and Vertex AI.
    /// Implements the Microsoft.Extensions.AI.IChatClient interface for use with Microsoft.Agents.AI.
    /// </summary>
    public class GeminiChatClient : IChatClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _baseUrl;
        private bool _disposed;

        public GeminiChatClient(string apiKey, string model)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                // Try environment variables
                apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") 
                    ?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY")
                    ?? throw new ArgumentException("API key must be provided or set via GEMINI_API_KEY environment variable", nameof(apiKey));
            }

            if (string.IsNullOrEmpty(model))
            {
                throw new ArgumentException("Model name is required", nameof(model));
            }

            _apiKey = apiKey;
            _model = model;
            _baseUrl = "https://generativelanguage.googleapis.com/v1beta";
            _httpClient = new HttpClient();
        }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            var messagesList = messages.ToList();
            if (messagesList.Count == 0)
            {
                throw new ArgumentException("At least one message is required", nameof(messages));
            }

            // Build the request
            var request = BuildGenerateContentRequest(messagesList, options);
            
            // Make the API call
            var url = $"{_baseUrl}/models/{_model}:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<GenerateContentResponse>(cancellationToken);
            
            if (result == null)
            {
                throw new InvalidOperationException("Failed to parse response from API");
            }

            // Convert to ChatResponse
            return ConvertToChatResponse(result);
        }

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages, 
            ChatOptions? options = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // For now, implement streaming by getting the full response and yielding it as a single update
            // TODO: Implement true streaming using streamGenerateContent endpoint
            var response = await GetResponseAsync(messages, options, cancellationToken);
            
            var firstMessage = response.Messages.FirstOrDefault();
            if (firstMessage != null)
            {
                yield return new ChatResponseUpdate(ChatRole.Assistant, firstMessage.Contents);
            }
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            // Return null for services we don't provide
            return null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        private object BuildGenerateContentRequest(List<ChatMessage> messages, ChatOptions? options)
        {
            var contents = new List<object>();
            
            foreach (var message in messages)
            {
                var parts = new List<object>();
                
                // Add text content
                if (!string.IsNullOrEmpty(message.Text))
                {
                    parts.Add(new { text = message.Text });
                }
                
                // Add other content types if present
                if (message.Contents != null)
                {
                    foreach (var content in message.Contents)
                    {
                        if (content is TextContent textContent)
                        {
                            parts.Add(new { text = textContent.Text });
                        }
                        // TODO: Add support for other content types (images, etc.)
                    }
                }
                
                contents.Add(new
                {
                    role = message.Role.Value.ToLowerInvariant() == "assistant" ? "model" : "user",
                    parts = parts
                });
            }
            
            var request = new
            {
                contents = contents,
                generationConfig = options != null ? new
                {
                    temperature = options.Temperature,
                    maxOutputTokens = options.MaxOutputTokens,
                    topP = options.TopP,
                    topK = options.TopK
                } : null
            };
            
            return request;
        }

        private ChatResponse ConvertToChatResponse(GenerateContentResponse apiResponse)
        {
            var response = new ChatResponse
            {
                ResponseId = Guid.NewGuid().ToString(),
                ModelId = _model
            };

            if (apiResponse.Candidates != null && apiResponse.Candidates.Count > 0)
            {
                var candidate = apiResponse.Candidates[0];
                var messageContents = new List<AIContent>();
                
                if (candidate.Content?.Parts != null)
                {
                    foreach (var part in candidate.Content.Parts)
                    {
                        if (part.Text != null)
                        {
                            messageContents.Add(new TextContent(part.Text));
                        }
                    }
                }
                
                var message = new ChatMessage
                {
                    Role = ChatRole.Assistant,
                    Contents = messageContents
                };
                
                response.Messages.Add(message);
            }
            
            return response;
        }

        // Internal classes for API request/response
        private class GenerateContentResponse
        {
            [JsonPropertyName("candidates")]
            public List<Candidate>? Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonPropertyName("content")]
            public Content? Content { get; set; }
            
            [JsonPropertyName("finishReason")]
            public string? FinishReason { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public List<Part>? Parts { get; set; }
            
            [JsonPropertyName("role")]
            public string? Role { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }
    }
}


