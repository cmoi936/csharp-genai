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

using Google.GenAI;
using Google.GenAI.Types;

Console.WriteLine("Google Gen AI SDK for C# - Quick Start");
Console.WriteLine("========================================\n");

// Check for API key in environment
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")
             ?? Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("⚠️  No API key found!");
    Console.WriteLine("Please set either GEMINI_API_KEY or GOOGLE_API_KEY environment variable.");
    Console.WriteLine("\nExample:");
    Console.WriteLine("  export GEMINI_API_KEY='your-api-key'");
    return;
}

try
{
    // Create client with API key and model
    using var client = new GeminiChatClient(apiKey: apiKey, model: "gemini-2.0-flash-001");

    Console.WriteLine("✓ Client created successfully\n");

    // Example 1: Simple text generation
    Console.WriteLine("Example 1: Simple Text Generation");
    Console.WriteLine("----------------------------------");
    var response1 = await client.Models.GenerateContentAsync(
        contents: "Why is the sky blue? Answer in one sentence."
    );
    Console.WriteLine($"Response: {response1.Text}\n");

    // Example 2: Using configuration
    Console.WriteLine("Example 2: With Configuration");
    Console.WriteLine("-----------------------------");
    var config = new GenerateContentConfig
    {
        Temperature = 0.7,
        MaxOutputTokens = 100,
        SystemInstruction = "You are a helpful assistant. Keep answers concise."
    };

    var response2 = await client.Models.GenerateContentAsync(
        contents: "What are the three primary colors?",
        config: config
    );
    Console.WriteLine($"Response: {response2.Text}\n");

    // Example 3: JSON response
    Console.WriteLine("Example 3: JSON Response");
    Console.WriteLine("------------------------");
    var jsonConfig = new GenerateContentConfig
    {
        ResponseMimeType = "application/json"
    };

    var response3 = await client.Models.GenerateContentAsync(
        contents: "List 3 programming languages with their year of creation as a JSON array",
        config: jsonConfig
    );
    Console.WriteLine($"Response: {response3.Text}\n");

    Console.WriteLine("✓ All examples completed successfully!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
