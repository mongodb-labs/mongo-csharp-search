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

        /// <summary>
        /// Creates a facet that determines the frequency of numeric values by breaking the search
        /// results into separate ranges of numbers.
        /// </summary>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of numeric values that specify the boundaries for each bucket.
        /// </param>
        /// <param name="default">
        /// The name of an additional bucket that counts documents returned from the operator that
        /// do not fall within the specified boundaries.
        /// </param>
        /// <returns>A number search facet.</returns>
        public SearchFacet<TDocument> Number(
            string name,
            PathDefinition<TDocument> path,
            IEnumerable<BsonValue> boundaries,
            string @default = null)
        {
            return new NumberSearchFacet<TDocument>(name, path, boundaries, @default);
        }

        /// <summary>
        /// Creates a facet that determines the frequency of numeric values by breaking the search
        /// results into separate ranges of numbers.
        /// </summary>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of numeric values that specify the boundaries for each bucket.
        /// </param>
        /// <returns>A number search facet.</returns>
        public SearchFacet<TDocument> Number(
            string name,
            PathDefinition<TDocument> path,
            params BsonValue[] boundaries)
        {
            return Number(name, path, (IEnumerable<BsonValue>)boundaries);
        }

        /// <summary>
        /// Creates a facet that determines the frequency of numeric values by breaking the search
        /// results into separate ranges of numbers.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of numeric values that specify the boundaries for each bucket.
        /// </param>
        /// <param name="default">
        /// The name of an additional bucket that counts documents returned from the operator that
        /// do not fall within the specified boundaries.
        /// </param>
        /// <returns>A number search facet.</returns>
        public SearchFacet<TDocument> Number<TField>(
            string name,
            Expression<Func<TDocument, TField>> path,
            IEnumerable<BsonValue> boundaries,
            string @default = null)
        {
            return Number(name, new ExpressionFieldDefinition<TDocument>(path), boundaries, @default);
        }
        /// <summary>
        /// Creates a facet that determines the frequency of numeric values by breaking the search
        /// results into separate ranges of numbers.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the facet.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of numeric values that specify the boundaries for each bucket.
        /// </param>
        /// <returns>A number search facet.</returns>
        public SearchFacet<TDocument> Number<TField>(
            string name,
            Expression<Func<TDocument, TField>> path,
            params BsonValue[] boundaries)
        {
            return Number(name, new ExpressionFieldDefinition<TDocument>(path), boundaries);
        }

        /// <summary>
        /// Creates a facet that narrows down search result based on a date.
        /// </summary>
        /// <param name="name">The name of the fact.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of date values that specify the boundaries for each bucket.
        /// </param>
        /// <param name="default">
        /// The name of an additional bucket that counts documents returned from the operator that
        /// do not fall within the specified boundaries.
        /// </param>
        /// <returns>A date search facet.</returns>
        public SearchFacet<TDocument> Date(
            string name,
            PathDefinition<TDocument> path,
            IEnumerable<DateTime> boundaries,
            string @default = null)
        {
            return new DateSearchFacet<TDocument>(name, path, boundaries, @default);
        }

        /// <summary>
        /// Creates a facet that narrows down search result based on a date.
        /// </summary>
        /// <param name="name">The name of the fact.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of date values that specify the boundaries for each bucket.
        /// </param>
        /// <returns>A date search facet.</returns>
        public SearchFacet<TDocument> Date(
            string name,
            PathDefinition<TDocument> path,
            params DateTime[] boundaries)
        {
            return Date(name, path, (IEnumerable<DateTime>)boundaries);
        }

        /// <summary>
        /// Creates a facet that narrows down search result based on a date.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the fact.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of date values that specify the boundaries for each bucket.
        /// </param>
        /// <param name="default">
        /// The name of an additional bucket that counts documents returned from the operator that
        /// do not fall within the specified boundaries.
        /// </param>
        /// <returns>A date search facet.</returns>
        public SearchFacet<TDocument> Date<TField>(
            string name,
            Expression<Func<TDocument, TField>> path,
            IEnumerable<DateTime> boundaries,
            string @default = null)
        {
            return Date(name, new ExpressionFieldDefinition<TDocument>(path), boundaries, @default);
        }

        /// <summary>
        /// Creates a facet that narrows down search result based on a date.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="name">The name of the fact.</param>
        /// <param name="path">The field path to facet on.</param>
        /// <param name="boundaries">
        /// A list of date values that specify the boundaries for each bucket.
        /// </param>
        /// <returns>A date search facet.</returns>
        public SearchFacet<TDocument> Date<TField>(
            string name,
            Expression<Func<TDocument, TField>> path,
            params DateTime[] boundaries)
        {
            return Date(name, new ExpressionFieldDefinition<TDocument>(path), boundaries);
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

    internal sealed class NumberSearchFacet<TDocument> : SearchFacet<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly IEnumerable<BsonValue> _boundaries;
        private readonly string _default;

        public NumberSearchFacet(string name, PathDefinition<TDocument> path, IEnumerable<BsonValue> boundaries, string @default)
            : base(name)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _boundaries = Ensure.IsNotNull(boundaries, nameof(boundaries));
            _default = @default;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("type", "number");
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            document.Add("boundaries", new BsonArray(_boundaries));
            if (_default != null)
            {
                document.Add("default", _default);
            }
            return document;
        }
    }

    internal sealed class DateSearchFacet<TDocument> : SearchFacet<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly IEnumerable<DateTime> _boundaries;
        private readonly string _default;

        public DateSearchFacet(string name, PathDefinition<TDocument> path, IEnumerable<DateTime> boundaries, string @default)
            : base(name)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _boundaries = Ensure.IsNotNull(boundaries, nameof(boundaries));
            _default = @default;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("type", "date");
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            document.Add("boundaries", new BsonArray(_boundaries));
            if (_default != null)
            {
                document.Add("default", _default);
            }
            return document;
        }
    }
}
