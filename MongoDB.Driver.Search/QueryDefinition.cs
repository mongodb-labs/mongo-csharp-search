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
using MongoDB.Driver.Core.Misc;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Labs.Search
{
    public abstract class QueryDefinition
    {
        public abstract BsonValue Render();

        public static implicit operator QueryDefinition(string query)
        {
            return new SingleQueryDefinition(query);
        }

        public static implicit operator QueryDefinition(string[] queries)
        {
            return new MultiQueryDefinition(queries);
        }

        public static implicit operator QueryDefinition(List<string> queries)
        {
            return new MultiQueryDefinition(queries);
        }
    }

    public sealed class SingleQueryDefinition : QueryDefinition
    {
        private readonly string _query;

        public SingleQueryDefinition(string query)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
        }

        public override BsonValue Render()
        {
            return new BsonString(_query);
        }
    }

    public sealed class MultiQueryDefinition : QueryDefinition
    {
        private readonly IEnumerable<string> _queries;

        public MultiQueryDefinition(IEnumerable<string> queries)
        {
            _queries = Ensure.IsNotNull(queries, nameof(queries));
        }

        public override BsonValue Render()
        {
            return new BsonArray(_queries.Select(query => new BsonString(query)));
        }
    }
}
