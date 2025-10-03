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

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Google.GenAI.Types
{
    /// <summary>
    /// Represents a content message with role and parts.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Gets or sets the role of the content sender (e.g., "user", "model").
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        /// <summary>
        /// Gets or sets the parts of the content.
        /// </summary>
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();

        /// <summary>
        /// Creates a user content with text.
        /// </summary>
        public static Content FromText(string text)
        {
            return new Content
            {
                Role = "user",
                Parts = new List<Part> { Part.FromText(text) }
            };
        }
    }

    /// <summary>
    /// Represents a part of content (text, image, function call, etc.).
    /// </summary>
    public class Part
    {
        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets inline data (for images, etc.).
        /// </summary>
        [JsonPropertyName("inlineData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InlineData? InlineData { get; set; }

        /// <summary>
        /// Gets or sets file data reference.
        /// </summary>
        [JsonPropertyName("fileData")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FileData? FileData { get; set; }

        /// <summary>
        /// Creates a text part.
        /// </summary>
        public static Part FromText(string text)
        {
            return new Part { Text = text };
        }

        /// <summary>
        /// Creates a part from a URI.
        /// </summary>
        public static Part FromUri(string fileUri, string mimeType)
        {
            return new Part
            {
                FileData = new FileData
                {
                    FileUri = fileUri,
                    MimeType = mimeType
                }
            };
        }
    }

    /// <summary>
    /// Represents inline data.
    /// </summary>
    public class InlineData
    {
        /// <summary>
        /// Gets or sets the MIME type.
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }

        /// <summary>
        /// Gets or sets the base64-encoded data.
        /// </summary>
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }

    /// <summary>
    /// Represents file data reference.
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// Gets or sets the file URI.
        /// </summary>
        [JsonPropertyName("fileUri")]
        public string? FileUri { get; set; }

        /// <summary>
        /// Gets or sets the MIME type.
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }
    }
}
