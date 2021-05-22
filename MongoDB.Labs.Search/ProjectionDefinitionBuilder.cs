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
    public static class ProjectionDefinitionExtensions
    {
        public static ProjectionDefinition<TDocument> MetaSearchScore<TDocument>(
            this ProjectionDefinition<TDocument> projection,
            string field)
        {
            var builder = Builders<TDocument>.Projection;
            return builder.Combine(projection, builder.MetaSearchScore(field));
        }
    }

    public static class ProjectionDefinitionBuilderExtensions
    {
        public static ProjectionDefinition<TSource> MetaSearchScore<TSource>(
            this ProjectionDefinitionBuilder<TSource> builder,
            string field)
        {
            return builder.Meta(field, "searchScore");
        }
    }
}
