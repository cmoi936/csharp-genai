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
using System.Linq;
using System.Text.Json.Serialization;

namespace Google.GenAI.Types
{
    /// <summary>
    /// Response from generate content operations.
    /// </summary>
    public class GenerateContentResponse
    {
        /// <summary>
        /// Gets or sets the list of candidate responses.
        /// </summary>
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();

        /// <summary>
        /// Gets or sets the prompt feedback.
        /// </summary>
        [JsonPropertyName("promptFeedback")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PromptFeedback? PromptFeedback { get; set; }

        /// <summary>
        /// Gets the text from the first candidate, if available.
        /// </summary>
        [JsonIgnore]
        public string? Text
        {
            get
            {
                if (Candidates.Count == 0 || Candidates[0].Content?.Parts == null)
                    return null;

                var content = Candidates[0].Content;
                if (content == null) return null;

                return string.Join("", content.Parts
                    .Where(p => p.Text != null)
                    .Select(p => p.Text));
            }
        }
    }

    /// <summary>
    /// Represents a candidate response.
    /// </summary>
    public class Candidate
    {
        /// <summary>
        /// Gets or sets the content of the candidate.
        /// </summary>
        [JsonPropertyName("content")]
        public Content? Content { get; set; }

        /// <summary>
        /// Gets or sets the finish reason.
        /// </summary>
        [JsonPropertyName("finishReason")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FinishReason { get; set; }

        /// <summary>
        /// Gets or sets the safety ratings.
        /// </summary>
        [JsonPropertyName("safetyRatings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SafetyRating>? SafetyRatings { get; set; }
    }

    /// <summary>
    /// Represents a safety rating.
    /// </summary>
    public class SafetyRating
    {
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the probability.
        /// </summary>
        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }

    /// <summary>
    /// Represents prompt feedback.
    /// </summary>
    public class PromptFeedback
    {
        /// <summary>
        /// Gets or sets the block reason.
        /// </summary>
        [JsonPropertyName("blockReason")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BlockReason { get; set; }

        /// <summary>
        /// Gets or sets the safety ratings.
        /// </summary>
        [JsonPropertyName("safetyRatings")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<SafetyRating>? SafetyRatings { get; set; }
    }
}
