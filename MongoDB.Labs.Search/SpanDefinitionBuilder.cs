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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a span clause.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class SpanDefinitionBuilder<TDocument>
    {
        /// <summary>
        /// Creates a span clause that matches near the beginning of the string.
        /// </summary>
        /// <param name="operator">The span operator.</param>
        /// <param name="endPositionLte">The highest position in which to match the query.</param>
        /// <returns>A first span clause.</returns>
        public SpanDefinition<TDocument> First(SpanDefinition<TDocument> @operator, int endPositionLte)
        {
            return new FirstSpanDefinition<TDocument>(@operator, endPositionLte);
        }

        /// <summary>
        /// Creates a span clause that matches multiple string found near each other.
        /// </summary>
        /// <param name="clauses">The clauses.</param>
        /// <param name="slop">The allowable distance between words in the query phrase.</param>
        /// <param name="inOrder">Whether to require that the clauses appear in the specified order.</param>
        /// <returns>A near span clause.</returns>
        public SpanDefinition<TDocument> Near(
            IEnumerable<SpanDefinition<TDocument>> clauses,
            int slop,
            bool inOrder = false)
        {
            return new NearSpanDefinition<TDocument>(clauses, slop, inOrder);
        }

        /// <summary>
        /// Creates a span clause that matches a single term.
        /// </summary>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <returns>A term span clause.</returns>
        public SpanDefinition<TDocument> Term(QueryDefinition query, PathDefinition<TDocument> path)
        {
            return new TermSpanDefinition<TDocument>(query, path);
        }

        /// <summary>
        /// Creates a span clause that matches a single term.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The string or string to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <returns>A term span clause.</returns>
        public SpanDefinition<TDocument> Term<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path)
        {
            return Term(query, new ExpressionFieldDefinition<TDocument>(path));
        }
    }

    internal sealed class FirstSpanDefinition<TDocument> : SpanDefinition<TDocument>
    {
        private readonly SpanDefinition<TDocument> _operator;
        private readonly int _endPositionLte;

        public FirstSpanDefinition(SpanDefinition<TDocument> @operator, int endPositionLte)
        {
            _operator = Ensure.IsNotNull(@operator, nameof(@operator));
            _endPositionLte = endPositionLte;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedOperator = _operator.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument();
            document.Add("operator", renderedOperator);
            document.Add("endPositionLte", _endPositionLte);
            return new BsonDocument("first", document);
        }
    }

    internal sealed class NearSpanDefinition<TDocument> : SpanDefinition<TDocument>
    {
        private readonly List<SpanDefinition<TDocument>> _clauses;
        private readonly int _slop;
        private readonly bool _inOrder;

        public NearSpanDefinition(
            IEnumerable<SpanDefinition<TDocument>> clauses,
            int slop,
            bool inOrder)
        {
            _clauses = Ensure.IsNotNull(clauses, nameof(clauses)).ToList();
            _slop = slop;
            _inOrder = inOrder;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var clauseDocs = _clauses.Select(clause => clause.Render(documentSerializer, serializerRegistry));
            var document = new BsonDocument();
            document.Add("clauses", new BsonArray(clauseDocs));
            document.Add("slop", _slop);
            document.Add("inOrder", _inOrder);
            return new BsonDocument("near", document);
        }
    }

    internal sealed class TermSpanDefinition<TDocument> : SpanDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;

        public TermSpanDefinition(QueryDefinition query, PathDefinition<TDocument> path)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedQuery = _query.Render();
            var renderedPath = _path.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument();
            document.Add("query", renderedQuery);
            document.Add("path", renderedPath);
            return new BsonDocument("term", document);
        }
    }
}
