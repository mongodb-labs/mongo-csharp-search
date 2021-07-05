// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Labs.Search.ObjectModel.Serializers;

namespace MongoDB.Labs.Search.ObjectModel
{
    /// <summary>
    /// Represents a result of highlighting.
    /// </summary>
    public class Highlight
    {
        /// <summary>
        /// Gets or sets the document field which returned a match.
        /// </summary>
        [BsonElement("path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets one or more objects containing the matching text and the surrounding text
        /// (if any).
        /// </summary>
        [BsonElement("texts")]
        public List<HighlightText> Texts { get; set; }

        /// <summary>
        /// Gets or sets the score assigned to this result.
        /// </summary>
        [BsonElement("score")]
        public double Score { get; set; }
    }

    /// <summary>
    /// Represents the matching text or the surrounding text of a highlighting result.
    /// </summary>
    public class HighlightText
    {
        /// <summary>
        /// Gets or sets the text from the field which returned a match.
        /// </summary>
        [BsonElement("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of text, matching or surrounding.
        /// </summary>
        [BsonElement("type")]
        [BsonSerializer(typeof(HighlightTextTypeSerializer))]
        public HighlightTextType Type { get; set; }
    }

    /// <summary>
    /// Represents the type of text in a highlighting result, matching or surrounding.
    /// </summary>
    public enum HighlightTextType
    {
        /// <summary>
        /// Indicates that the text contains a match.
        /// </summary>
        Hit,

        /// <summary>
        /// Indicates that the text contains the text content adjacent to a matching string.
        /// </summary>
        Text
    }
}
