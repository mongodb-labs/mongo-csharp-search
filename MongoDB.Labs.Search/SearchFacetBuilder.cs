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
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a search facet.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class SearchFacetBuilder<TDocument>
    {
        /// <summary>
        /// Creates a facet that narrows down Atlas Search results based on the most frequent
        /// string values in the specified string field.
        /// </summary>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="numBuckets">
        /// The maximum number of facet categories to return in the results.
        /// </param>
        /// <returns>A string search facet.</returns>
        public SearchFacet<TDocument> String(string name, PathDefinition<TDocument> path, int numBuckets = 10)
        {
            return new StringSearchFacet<TDocument>(name, path, numBuckets);
        }

        /// <summary>
        /// Creates a facet that narrows down Atlas Search result based on the most frequent
        /// string values in the specified string field.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="numBuckets">
        /// The maximum number of facet categories to return in the results.
        /// </param>
        /// <returns>A string search facet.</returns>
        public SearchFacet<TDocument> String<TField>(string name, Expression<Func<TDocument, TField>> path, int numBuckets = 10)
        {
            return String(name, new ExpressionFieldDefinition<TDocument>(path), numBuckets);
        }
    }

    internal sealed class StringSearchFacet<TDocument> : SearchFacet<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly int _numBuckets;

        public StringSearchFacet(string name, PathDefinition<TDocument> path, int numBuckets)
            : base(name)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _numBuckets = Ensure.IsBetween(numBuckets, 1, 1000, nameof(numBuckets));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("type", "string");
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            if (_numBuckets != 10)
            {
                document.Add("numBuckets", _numBuckets);
            }
            return document;
        }
    }
}