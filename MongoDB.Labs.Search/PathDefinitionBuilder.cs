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

using MongoDB.Driver;
using System;
using System.Linq.Expressions;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a search path.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class PathDefinitionBuilder<TDocument>
    {
        /// <summary>
        /// Creates a search path that searches using the specified analyzer.
        /// </summary>
        /// <param name="field">The field definition</param>
        /// <param name="analyzerName">The name of the analyzer.</param>
        /// <returns>An analyzer search path.</returns>
        public PathDefinition<TDocument> Analyzer(FieldDefinition<TDocument> field, string analyzerName)
        {
            return new AnalyzerPathDefinition<TDocument>(field, analyzerName);
        }

        /// <summary>
        /// Creates a search path that searches using the specified analyzer.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field definition</param>
        /// <param name="analyzerName">The name of the analyzer.</param>
        /// <returns>An analyzer search path.</returns>
        public PathDefinition<TDocument> Analyzer<TField>(Expression<Func<TDocument, TField>> field, string analyzerName)
        {
            return Analyzer(new ExpressionFieldDefinition<TDocument>(field), analyzerName);
        }
    }
}
