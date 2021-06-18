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

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// Extension methods for projections pertaining to Atlas Search.
    /// </summary>
    public static class ProjectionDefinitionExtensions
    {
        /// <summary>
        /// Combines an existing projection with a search highlights projection.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <param name="projection">The projection.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// A combined projection.
        /// </returns>
        public static ProjectionDefinition<TDocument> MetaSearchHighlights<TDocument>(
            this ProjectionDefinition<TDocument> projection,
            string field)
        {
            var builder = Builders<TDocument>.Projection;
            return builder.Combine(projection, builder.MetaSearchHighlights(field));
        }

        /// <summary>
        /// Combines an existing projection with a search score projection.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <param name="projection">The projection.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// A combined projection.
        /// </returns>
        public static ProjectionDefinition<TDocument> MetaSearchScore<TDocument>(
            this ProjectionDefinition<TDocument> projection,
            string field)
        {
            var builder = Builders<TDocument>.Projection;
            return builder.Combine(projection, builder.MetaSearchScore(field));
        }
    }

    /// <summary>
    /// Extension methods for <see cref="ProjectionDefinitionBuilder{TSource}"/> pertaining to
    /// Atlas Search.
    /// </summary>
    public static class ProjectionDefinitionBuilderExtensions
    {
        /// <summary>
        /// Creates a search highlights projection.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="builder">The projection definition builder.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// A search highlights projection.
        /// </returns>
        public static ProjectionDefinition<TSource> MetaSearchHighlights<TSource>(
            this ProjectionDefinitionBuilder<TSource> builder,
            string field)
        {
            return builder.Meta(field, "searchHighlights");
        }

        /// <summary>
        /// Creates a search score projection.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="builder">The projection definition builder.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// A search score projection.
        /// </returns>
        public static ProjectionDefinition<TSource> MetaSearchScore<TSource>(
            this ProjectionDefinitionBuilder<TSource> builder,
            string field)
        {
            return builder.Meta(field, "searchScore");
        }
    }
}
