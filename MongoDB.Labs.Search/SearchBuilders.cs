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

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A static helper class containing various builders pertaining to Atlas Search.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public static class SearchBuilders<TDocument>
    {
        private static readonly PathDefinitionBuilder<TDocument> __path = new PathDefinitionBuilder<TDocument>();
        private static readonly HighlightOptionsBuilder<TDocument> __highlight = new HighlightOptionsBuilder<TDocument>();
        private static readonly ScoreDefinitionBuilder<TDocument> __score = new ScoreDefinitionBuilder<TDocument>();
        private static readonly SearchDefinitionBuilder<TDocument> __search = new SearchDefinitionBuilder<TDocument>();
        private static readonly SpanDefinitionBuilder<TDocument> __span = new SpanDefinitionBuilder<TDocument>();

        /// <summary>
        /// Gets a <see cref="PathDefinition{TDocument}"/>.
        /// </summary>
        public static PathDefinitionBuilder<TDocument> Path
        {
            get { return __path; }
        }

        /// <summary>
        /// Gets a <see cref="HighlightOptionsBuilder{TDocument}"/>.
        /// </summary>
        public static HighlightOptionsBuilder<TDocument> Highlight
        {
            get { return __highlight; }
        }

        /// <summary>
        /// Gets a <see cref="ScoreDefinitionBuilder{TDocument}"/>.
        /// </summary>
        public static ScoreDefinitionBuilder<TDocument> Score
        {
            get { return __score; }
        }

        /// <summary>
        /// Gets a <see cref="SearchDefinitionBuilder{TDocument}"/>.
        /// </summary>
        public static SearchDefinitionBuilder<TDocument> Search
        {
            get { return __search; }
        }

        /// <summary>
        /// Gets a <see cref="SpanDefinitionBuilder{TDocument}"/>.
        /// </summary>
        public static SpanDefinitionBuilder<TDocument> Span
        {
            get { return __span; }
        }
    }
}
