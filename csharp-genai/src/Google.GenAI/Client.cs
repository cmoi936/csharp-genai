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
using System.Net.Http;
using Google.GenAI.Models;
using Google.GenAI.Types;

namespace Google.GenAI
{
    /// <summary>
    /// Client for making requests to Google's Generative AI services.
    /// Supports both Gemini Developer API and Vertex AI.
    /// </summary>
    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly bool _vertexAi;
        private readonly string? _projectId;
        private readonly string? _location;
        private readonly ModelsClient _models;
        private bool _disposed;

        /// <summary>
        /// Gets the Models client for model-related operations.
        /// </summary>
        public ModelsClient Models => _models;

        /// <summary>
        /// Initializes a new instance of the Client class for Gemini Developer API.
        /// </summary>
        /// <param name="apiKey">The API key for authentication. If null, will attempt to read from environment variable GEMINI_API_KEY or GOOGLE_API_KEY.</param>
        /// <param name="httpOptions">Optional HTTP configuration options.</param>
        public Client(string? apiKey = null, HttpOptions? httpOptions = null)
        {
            _apiKey = apiKey ?? GetApiKeyFromEnvironment();
            _vertexAi = false;
            _httpClient = CreateHttpClient(httpOptions);
            _models = new ModelsClient(this, _httpClient, _apiKey, _vertexAi, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the Client class for Vertex AI.
        /// </summary>
        /// <param name="vertexAi">Set to true to use Vertex AI.</param>
        /// <param name="projectId">The Google Cloud project ID.</param>
        /// <param name="location">The location/region for Vertex AI (e.g., "us-central1").</param>
        /// <param name="httpOptions">Optional HTTP configuration options.</param>
        public Client(bool vertexAi, string projectId, string location, HttpOptions? httpOptions = null)
        {
            if (!vertexAi)
                throw new ArgumentException("When using this constructor, vertexAi must be true");
            
            _vertexAi = true;
            _projectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
            _location = location ?? throw new ArgumentNullException(nameof(location));
            _httpClient = CreateHttpClient(httpOptions);
            _models = new ModelsClient(this, _httpClient, null, _vertexAi, _projectId, _location);
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
}
