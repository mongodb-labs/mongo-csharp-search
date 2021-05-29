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

using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace MongoDB.Labs.Search
{
    public sealed class HighlightOptionsBuilder<TDocument>
    {
        public HighlightOptions<TDocument> Options(
            PathDefinition<TDocument> path,
            int? maxCharsToExamine = null,
            int? maxNumPassages = null)
        {
            return new HighlightOptions<TDocument>()
            {
                Path = path,
                MaxCharsToExamine = maxCharsToExamine,
                MaxNumPassages = maxNumPassages
            };
        }

        public HighlightOptions<TDocument> Options<TField>(
            Expression<Func<TDocument, TField>> path,
            int? maxCharsToExamine = null,
            int? maxNumPassages = null)
        {
            return Options(new ExpressionFieldDefinition<TDocument>(path), maxCharsToExamine, maxNumPassages);
        }
    }
}
