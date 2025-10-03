using Microsoft.Extensions.AI;

namespace Google.GenAI
{
    /// <summary>
    /// Client for making requests to Google's Generative AI services.
    /// Supports both Gemini Developer API and Vertex AI.
    /// Implements the Microsoft.Agents.AI.IChatClient interface.
    /// </summary>
    public class GeminiChatClient : IChatClient
    {
        GeminiChatClient(string apikey, string model)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
