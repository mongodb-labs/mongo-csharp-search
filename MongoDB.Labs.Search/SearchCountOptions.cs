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

using MongoDB.Bson;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// Options for counting the search results.
    /// </summary>
    public class SearchCountOptions
    {
        private SearchCountType _type = SearchCountType.LowerBound;
        private int? _threshold;

        /// <summary>
        /// Gets or sets the type of count of the documents in the result set.
        /// </summary>
        public SearchCountType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Gets or sets the number of documents to include in the exact count if
        /// <see cref="Type"/> is <see cref="SearchCountType.LowerBound"/>.
        /// </summary>
        public int? Threshold
        {
            get { return _threshold; }
            set { _threshold = Ensure.IsNullOrGreaterThanZero(value, nameof(value)); }
        }

        internal BsonDocument Render()
        {
            BsonDocument document = new BsonDocument();
            if (_type == SearchCountType.Total)
            {
                document.Add("type", "total");
            }
            if (_threshold != null)
            {
                document.Add("threshold", _threshold);
            }
            return document;
        }
    }
}
