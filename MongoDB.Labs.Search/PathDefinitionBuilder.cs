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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a search path.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class PathDefinitionBuilder<TDocument>
    {
        /// <summary>
        /// Creates a search path for a single field.
        /// </summary>
        /// <param name="field">The field definition.</param>
        /// <returns>A single-field search path.</returns>
        public PathDefinition<TDocument> Single(FieldDefinition<TDocument> field)
        {
            return new SinglePathDefinition<TDocument>(field);
        }

        /// <summary>
        /// Creates a search path for a single field.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field definition.</param>
        /// <returns>A single-field search path.</returns>
        public PathDefinition<TDocument> Single<TField>(Expression<Func<TDocument, TField>> field)
        {
            return Single(new ExpressionFieldDefinition<TDocument>(field));
        }

        /// <summary>
        /// Creates a search path for multiple fields.
        /// </summary>
        /// <param name="fields">The collection of field definitions.</param>
        /// <returns>A multi-field search path.</returns>
        public PathDefinition<TDocument> Multi(IEnumerable<FieldDefinition<TDocument>> fields)
        {
            return new MultiPathDefinition<TDocument>(fields);
        }

        /// <summary>
        /// Creates a search path for multiple fields.
        /// </summary>
        /// <param name="fields">The array of field definitions.</param>
        /// <returns>A multi-field search path.</returns>
        public PathDefinition<TDocument> Multi(params FieldDefinition<TDocument>[] fields)
        {
            return Multi((IEnumerable<FieldDefinition<TDocument>>)fields);
        }

        /// <summary>
        /// Creates a search path for multiple fields.
        /// </summary>
        /// <typeparam name="TField">The type of the fields.</typeparam>
        /// <param name="fields">The array of field definitions.</param>
        /// <returns>A multi-field search path.</returns>
        public PathDefinition<TDocument> Multi<TField>(params Expression<Func<TDocument, TField>>[] fields)
        {
            return Multi(fields.Select(x => new ExpressionFieldDefinition<TDocument>(x)));
        }

        /// <summary>
        /// Creates a search path that searches using the specified analyzer.
        /// </summary>
        /// <param name="field">The field definition</param>
        /// <param name="analyzerName">The name of the analyzer.</param>
        /// <returns>An analyzer search path.</returns>
        public PathDefinition<TDocument> Analyzer(FieldDefinition<TDocument> field, string analyzerName)
        {
            return new AnalyzerPathDefinition<TDocument>(field, analyzerName);
        }

        /// <summary>
        /// Creates a search path that searches using the specified analyzer.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="field">The field definition</param>
        /// <param name="analyzerName">The name of the analyzer.</param>
        /// <returns>An analyzer search path.</returns>
        public PathDefinition<TDocument> Analyzer<TField>(Expression<Func<TDocument, TField>> field, string analyzerName)
        {
            return Analyzer(new ExpressionFieldDefinition<TDocument>(field), analyzerName);
        }

        /// <summary>
        /// Creates a search path that uses special characters in the field name
        /// that can match any character.
        /// </summary>
        /// <param name="query">
        /// The wildcard string that the field name must match.
        /// </param>
        /// <returns>A wildcard search path.</returns>
        public PathDefinition<TDocument> Wildcard(string query)
        {
            return new WildcardPathDefinition<TDocument>(query);
        }
    }

    internal sealed class SinglePathDefinition<TDocument> : PathDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _field;

        public SinglePathDefinition(FieldDefinition<TDocument> field)
        {
            _field = Ensure.IsNotNull(field, nameof(field));
        }

        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _field.Render(documentSerializer, serializerRegistry);
            return new BsonString(renderedField.FieldName);
        }
    }

    internal sealed class MultiPathDefinition<TDocument> : PathDefinition<TDocument>
    {
        private readonly IEnumerable<FieldDefinition<TDocument>> _fields;

        public MultiPathDefinition(IEnumerable<FieldDefinition<TDocument>> fields)
        {
            _fields = Ensure.IsNotNull(fields, nameof(fields));
        }

        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return new BsonArray(_fields.Select(field =>
            {
                var renderedField = field.Render(documentSerializer, serializerRegistry);
                return new BsonString(renderedField.FieldName);
            }));
        }
    }

    internal sealed class AnalyzerPathDefinition<TDocument> : PathDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _field;
        private readonly string _analyzerName;

        public AnalyzerPathDefinition(FieldDefinition<TDocument> field, string analyzerName)
        {
            _field = Ensure.IsNotNull(field, nameof(field));
            _analyzerName = Ensure.IsNotNull(analyzerName, nameof(analyzerName));
        }

        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _field.Render(documentSerializer, serializerRegistry);
            return new BsonDocument
            {
                ["value"] = renderedField.FieldName,
                ["multi"] = _analyzerName
            };
        }
    }

    internal sealed class WildcardPathDefinition<TDocument> : PathDefinition<TDocument>
    {
        private readonly string _query;

        public WildcardPathDefinition(string query)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
        }

        public override BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return new BsonDocument("wildcard", _query);
        }
    }
}
