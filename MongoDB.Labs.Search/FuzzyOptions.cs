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

using MongoDB.Driver.Core.Misc;
using MongoDB.Bson;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// Options for fuzzy search.
    /// </summary>
    public class FuzzyOptions
    {
        private int? _maxEdits;
        private int? _prefixLength;
        private int? _maxExpansions;

        /// <summary>
        /// Gets or sets the maximum number of single-character edits required to match the
        /// specified search term.
        /// </summary>
        public int? MaxEdits
        {
            get { return _maxEdits; }
            set { _maxEdits = EnsureExtensions.IsNullOrBetween(value, 1, 2, nameof(value)); }
        }

        /// <summary>
        /// Gets or sets the number of characters at the beginning of each term in the result that
        /// must exactly match.
        /// </summary>
        public int? PrefixLength
        {
            get { return _prefixLength; }
            set { _prefixLength = Ensure.IsNullOrGreaterThanOrEqualToZero(value, nameof(value)); }
        }

        /// <summary>
        /// Gets or sets the number of variations to generate and search for.
        /// </summary>
        public int? MaxExpansions
        {
            get { return _maxExpansions; }
            set { _maxExpansions = Ensure.IsNullOrGreaterThanZero(value, nameof(value)); }
        }

        internal BsonDocument Render()
        {
            BsonDocument document = new BsonDocument();
            if (_maxEdits != null)
            {
                document.Add("maxEdits", _maxEdits);
            }
            if (_prefixLength != null)
            {
                document.Add("prefixLength", _prefixLength);
            }
            if (_maxExpansions != null)
            {
                document.Add("maxExpansions", _maxExpansions);
            }
            return document;
        }
    }
}
