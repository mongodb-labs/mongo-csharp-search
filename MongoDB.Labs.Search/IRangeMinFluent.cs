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
    /// Fluent interface for range searches where only the minimum value can be set.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TValue">The type of the field value.</typeparam>
    public interface IRangeMinFluent<TDocument, TValue>
        where TValue : struct
    {
        /// <summary>
        /// Creates a greater than range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The search definition.</returns>
        SearchDefinition<TDocument> Gt(TValue value);

        /// <summary>
        /// Creates a greater than or equal range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The search definition.</returns>
        SearchDefinition<TDocument> Gte(TValue value);
    }

    internal class RangeMinFluentImpl<TDocument, TValue, TFactory> : IRangeMinFluent<TDocument, TValue>
        where TValue : struct
        where TFactory : IBsonValueFactory<TValue>, new()
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly ScoreDefinition<TDocument> _score;
        private readonly TValue _max;
        private readonly bool _maxInclusive;

        public RangeMinFluentImpl(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score,
            TValue max,
            bool maxInclusive)
        {
            _path = path;
            _score = score;
            _max = max;
            _maxInclusive = maxInclusive;
        }

        public SearchDefinition<TDocument> Gt(TValue value)
        {
            return new RangeSearchDefinition<TDocument, TValue, TFactory>(_path, _score, value, false, _max, _maxInclusive);
        }

        public SearchDefinition<TDocument> Gte(TValue value)
        {
            return new RangeSearchDefinition<TDocument, TValue, TFactory>(_path, _score, value, true, _max, _maxInclusive);
        }
    }
}
