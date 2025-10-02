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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.GenAI.Types;

namespace Google.GenAI.Models
{
    /// <summary>
    /// Client for model-related operations (generate content, count tokens, etc.).
    /// </summary>
    public class ModelsClient
    {
        private readonly GeminiChatClient _client;
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly bool _vertexAi;
        private readonly string? _projectId;
        private readonly string? _location;

        internal ModelsClient(GeminiChatClient client, HttpClient httpClient, string? apiKey, bool vertexAi, string? projectId, string? location)
        {
            _client = client;
            _httpClient = httpClient;
            _apiKey = apiKey;
            _vertexAi = vertexAi;
            _projectId = projectId;
            _location = location;
        }

        /// <summary>
        /// Generates content using the specified model.
        /// </summary>
        /// <param name="model">The model ID (e.g., "gemini-2.0-flash-001").</param>
        /// <param name="contents">The content to send to the model (string or Content object).</param>
        /// <param name="config">Optional configuration for generation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The generated content response.</returns>
        public async Task<GenerateContentResponse> GenerateContentAsync(
            string model,
            object contents,
            GenerateContentConfig? config = null,
            CancellationToken cancellationToken = default)
        {
            var url = BuildUrl(model, "generateContent");
            var request = BuildRequest(contents, config);

            var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GenerateContentResponse>(
                cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException("Failed to deserialize response");
        }

        /// <summary>
        /// Generates content using the specified model (synchronous).
        /// </summary>
        public GenerateContentResponse GenerateContent(
            string model,
            object contents,
            GenerateContentConfig? config = null)
        {
            return GenerateContentAsync(model, contents, config).GetAwaiter().GetResult();
        }

        private string BuildUrl(string model, string operation)
        {
            if (_vertexAi)
            {
                // Vertex AI endpoint
                return $"https://{_location}-aiplatform.googleapis.com/v1/projects/{_projectId}/locations/{_location}/publishers/google/models/{model}:{operation}";
            }
            else
            {
                // Gemini Developer API endpoint
                var apiKeyParam = string.IsNullOrEmpty(_apiKey) ? "" : $"?key={_apiKey}";
                return $"https://generativelanguage.googleapis.com/v1beta/models/{model}:{operation}{apiKeyParam}";
            }
        }

        private static object BuildRequest(object contents, GenerateContentConfig? config)
        {
            var contentsList = new List<Content>();

            // Convert contents to List<Content>
            if (contents is string text)
            {
                contentsList.Add(Content.FromText(text));
            }
            else if (contents is Content content)
            {
                contentsList.Add(content);
            }
            else if (contents is IEnumerable<string> textList)
            {
                var parts = textList.Select(t => Part.FromText(t)).ToList();
                contentsList.Add(new Content { Role = "user", Parts = parts });
            }
            else if (contents is IEnumerable<Content> contentList)
            {
                contentsList.AddRange(contentList);
            }
            else
            {
                throw new ArgumentException("Unsupported contents type", nameof(contents));
            }

            var request = new Dictionary<string, object>
            {
                ["contents"] = contentsList
            };

            if (config != null)
            {
                var generationConfig = new Dictionary<string, object>();

                if (config.Temperature.HasValue)
                    generationConfig["temperature"] = config.Temperature.Value;
                if (config.MaxOutputTokens.HasValue)
                    generationConfig["maxOutputTokens"] = config.MaxOutputTokens.Value;
                if (config.TopP.HasValue)
                    generationConfig["topP"] = config.TopP.Value;
                if (config.TopK.HasValue)
                    generationConfig["topK"] = config.TopK.Value;
                if (config.CandidateCount.HasValue)
                    generationConfig["candidateCount"] = config.CandidateCount.Value;
                if (config.ResponseMimeType != null)
                    generationConfig["responseMimeType"] = config.ResponseMimeType;

                if (generationConfig.Count > 0)
                    request["generationConfig"] = generationConfig;

                if (config.SystemInstruction != null)
                {
                    request["systemInstruction"] = new
                    {
                        parts = new[] { new { text = config.SystemInstruction } }
                    };
                }
            }

            return request;
        }
    }
}
