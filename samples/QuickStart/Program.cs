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
using Microsoft.Agents.AI;

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
    using IChatClient chatClient = new GeminiChatClient(apiKey: apiKey, model: "gemini-2.0-flash-001");

    Console.WriteLine("✓ Client created successfully\n");

    // Create chat messages
    var messages = new List<ChatMessage>
    {
        new ChatMessage
        {
            Role = "user",
            Text = "Write a story about a programmer who discovers a hidden feature in their favorite programming language."
        }
    };

    // Get response
    var response = await chatClient.GetResponseAsync(messages);

    Console.WriteLine(response.Text);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
