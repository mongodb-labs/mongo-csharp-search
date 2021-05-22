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
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using System;
using System.Linq.Expressions;

namespace MongoDB.Labs.Search
{
    public sealed class ScoreDefinitionBuilder<TDocument>
    {
        public ScoreDefinition<TDocument> Boost(double value)
        {
            return new BoostValueScoreDefinition<TDocument>(value);
        }

        public ScoreDefinition<TDocument> Boost(PathDefinition<TDocument> path, double undefined = 0)
        {
            return new BoostPathScoreDefinition<TDocument>(path, undefined);
        }

        public ScoreDefinition<TDocument> Boost(Expression<Func<TDocument, double>> path, double undefined = 0)
        {
            return Boost(new ExpressionFieldDefinition<TDocument>(path), undefined);
        }

        public ScoreDefinition<TDocument> Constant(double value)
        {
            return new ConstantScoreDefinition<TDocument>(value);
        }
    }

    internal class BoostValueScoreDefinition<TDocument> : ScoreDefinition<TDocument>
    {
        private readonly double _value;

        public BoostValueScoreDefinition(double value)
        {
            _value = EnsureExtensions.IsGreaterThanZero(value, nameof(value));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("value", _value);
            return new BsonDocument("boost", document);
        }
    }

    internal class BoostPathScoreDefinition<TDocument> : ScoreDefinition<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly double _undefined;

        public BoostPathScoreDefinition(PathDefinition<TDocument> path, double undefined)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _undefined = undefined;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("path", _path.Render(documentSerializer, serializerRegistry));
            if (_undefined != 0)
            {
                document.Add("undefined", _undefined);
            }
            return new BsonDocument("boost", document);
        }
    }
    internal sealed class ConstantScoreDefinition<TDocument> : ScoreDefinition<TDocument>
    {
        private readonly double _value;

        public ConstantScoreDefinition(double value)
        {
            _value = EnsureExtensions.IsGreaterThanZero(value, nameof(value));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("value", _value);
            return new BsonDocument("constant", document);
        }
    }
}