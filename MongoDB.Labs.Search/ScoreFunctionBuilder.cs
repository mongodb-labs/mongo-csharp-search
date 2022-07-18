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
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using System;
using System.Linq.Expressions;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a score function.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class ScoreFunctionBuilder<TDocument>
    {
        /// <summary>
        /// Creates a function that incorporates an indexed numeric field value into the score.
        /// </summary>
        /// <param name="path">The path to the numeric field.</param>
        /// <param name="undefined">
        /// The value to use if the numeric field specified using <paramref name="path"/> is
        /// missing in the document.
        /// </param>
        /// <returns>A path score function.</returns>
        public ScoreFunction<TDocument> Path(PathDefinition<TDocument> path, double undefined = 0)
        {
            return new PathScoreFunction<TDocument>(path, undefined);
        }

        /// <summary>
        /// Creates a function that incorporates an indexed numeric field value into the score.
        /// </summary>
        /// <param name="path">The path to the numeric field.</param>
        /// <param name="undefined">
        /// The value to use if the numeric field specified using <paramref name="path"/> is
        /// missing in the document.
        /// </param>
        /// <returns>A path score function.</returns>
        public ScoreFunction<TDocument> Path(Expression<Func<TDocument, double>> path, double undefined = 0)
        {
            return Path(new ExpressionFieldDefinition<TDocument>(path), undefined);
        }

        /// <summary>
        /// Creates a function that represents the relevance score, which is the score Atlas Search
        /// assigns documents based on relevance.
        /// </summary>
        /// <returns>A relevance score function.</returns>
        public ScoreFunction<TDocument> Relevance()
        {
            return new RelevanceScoreFunction<TDocument>();
        }

        /// <summary>
        /// Creates a function that represents a constant number.
        /// </summary>
        /// <param name="value">Number that indicates a fixed value.</param>
        /// <returns>A constant score function.</returns>
        public ScoreFunction<TDocument> Constant(double value)
        {
            return new ConstantScoreFunction<TDocument>(value);
        }
    }

    internal class PathScoreFunction<TDocument> : ScoreFunction<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly double _undefined;

        public PathScoreFunction(PathDefinition<TDocument> path, double undefined)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _undefined = undefined;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegister)
        {
            BsonValue renderedPath = _path.Render(documentSerializer, serializerRegister);
            if (_undefined == 0)
            {
                return new BsonDocument("path", renderedPath);
            }

            var document = new BsonDocument();
            document.Add("value", renderedPath);
            document.Add("undefined", _undefined);
            return new BsonDocument("path", document);
        }
    }

    internal class RelevanceScoreFunction<TDocument> : ScoreFunction<TDocument>
    {
        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegister)
        {
            return new BsonDocument("score", "relevance");
        }
    }

    internal class ConstantScoreFunction<TDocument> : ScoreFunction<TDocument>
    {
        private readonly double _value;

        public ConstantScoreFunction(double value)
        {
            _value = value;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegister)
        {
            return new BsonDocument("constant", _value);
        }
    }
}
