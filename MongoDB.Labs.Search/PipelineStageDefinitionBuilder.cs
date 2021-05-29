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

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    public static class PipelineStageDefinitionBuilder
    {
        public static PipelineStageDefinition<TInput, TInput> Search<TInput>(
            SearchDefinition<TInput> query, HighlightOptions<TInput> highlight = null)
        {
            Ensure.IsNotNull(query, nameof(query));

            const string operatorName = "$search";
            var stage = new DelegatedPipelineStageDefinition<TInput, TInput>(
                operatorName,
                (s, sr) =>
                {
                    var renderedQuery = query.Render(s, sr);
                    if (highlight != null)
                    {
                        renderedQuery.Add("highlight", highlight.Render(s, sr));
                    }
                    var document = new BsonDocument(operatorName, renderedQuery);
                    return new RenderedPipelineStageDefinition<TInput>(operatorName, document, s);
                });

            return stage;
        }
    }
}
