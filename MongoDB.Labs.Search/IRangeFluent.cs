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
    /// Fluent interface for range searches.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    /// <typeparam name="TValue">The type of the field value.</typeparam>
    public interface IRangeFluent<TDocument, TValue>
        where TValue : struct
    {
        /// <summary>
        /// Creates a less than range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The fluent interface.</returns>
        IRangeMinFluent<TDocument, TValue> Lt(TValue value);

        /// <summary>
        /// Creates a less than or equal range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The fluent interface.</returns>
        IRangeMinFluent<TDocument, TValue> Lte(TValue value);

        /// <summary>
        /// Creates a greater than range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The fluent interface.</returns>
        IRangeMaxFluent<TDocument, TValue> Gt(TValue value);

        /// <summary>
        /// Creates a greater than or equal range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The fluent interface.</returns>
        IRangeMaxFluent<TDocument, TValue> Gte(TValue value);
    }

    internal class RangeFluentImpl<TDocument, TValue, TFactory> : IRangeFluent<TDocument, TValue>
        where TValue : struct
        where TFactory : IBsonValueFactory<TValue>, new()
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly ScoreDefinition<TDocument> _score;

        public RangeFluentImpl(PathDefinition<TDocument> path, ScoreDefinition<TDocument> score)
        {
            _path = path;
            _score = score;
        }

        public IRangeMinFluent<TDocument, TValue> Lt(TValue value)
        {
            return new RangeMinFluentImpl<TDocument, TValue, TFactory>(_path, _score, value, false);
        }

        public IRangeMinFluent<TDocument, TValue> Lte(TValue value)
        {
            return new RangeMinFluentImpl<TDocument, TValue, TFactory>(_path, _score, value, true);
        }

        public IRangeMaxFluent<TDocument, TValue> Gt(TValue value)
        {
            return new RangeMaxFluentImpl<TDocument, TValue, TFactory>(_path, _score, value, false);
        }

        public IRangeMaxFluent<TDocument, TValue> Gte(TValue value)
        {
            return new RangeMaxFluentImpl<TDocument, TValue, TFactory>(_path, _score, value, true);
        }
    }
}
