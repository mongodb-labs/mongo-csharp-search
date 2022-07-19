﻿// Copyright 2021-present MongoDB Inc.
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

using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A result set for a search metadata query.
    /// </summary>
    public class SearchMetaResult
    {
        /// <summary>
        /// Gets or sets the count result set.
        /// </summary>
        [BsonElement("count")]
        public SearchMetaCountResult Count { get; set; }
    }

    /// <summary>
    /// A search count result set.
    /// </summary>
    public class SearchMetaCountResult
    {
        /// <summary>
        /// Gets or sets the lower bound for this result set.
        /// </summary>
        [BsonElement("lowerBound")]
        public long? LowerBound { get; set; }

        /// <summary>
        /// Gets or sets the total for this result set.
        /// </summary>
        [BsonElement("total")]
        public long? Total { get; set; }
    }
}
