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
    /// Fluent interface for range searches where only the maximum value can be set.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TValue">The type of the field value.</typeparam>
    public interface IRangeMaxFluent<TDocument, TValue>
        where TValue : struct
    {
        /// <summary>
        /// Creates a less than range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The search definition.</returns>
        SearchDefinition<TDocument> Lt(TValue value);

        /// <summary>
        /// Creates a less than or equal range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The search definition.</returns>
        SearchDefinition<TDocument> Lte(TValue value);
    }

    internal class RangeMaxFluentImpl<TDocument, TValue, TFactory> : IRangeMaxFluent<TDocument, TValue>
        where TValue : struct
        where TFactory : IBsonValueFactory<TValue>, new()
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly ScoreDefinition<TDocument> _score;
        private readonly TValue _min;
        private readonly bool _minInclusive;

        public RangeMaxFluentImpl(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score,
            TValue min,
            bool minInclusive)
        {
            _path = path;
            _score = score;
            _min = min;
            _minInclusive = minInclusive;
        }

        public SearchDefinition<TDocument> Lt(TValue value)
        {
            return new RangeSearchDefinition<TDocument, TValue, TFactory>(_path, _score, _min, _minInclusive, value, false);
        }

        public SearchDefinition<TDocument> Lte(TValue value)
        {
            return new RangeSearchDefinition<TDocument, TValue, TFactory>(_path, _score, _min, _minInclusive, value, true);
        }
    }
}
