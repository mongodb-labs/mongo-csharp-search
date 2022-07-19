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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MongoDB.Labs.Search
{
    /// <summary>
    /// A builder for a search definition.
    /// </summary>
    /// <typeparam name="TDocument">The type of the document.</typeparam>
    public sealed class SearchDefinitionBuilder<TDocument>
    {
        /// <summary>
        /// Creates a search definition that performs a search for a word or phrase that contains
        /// a sequence of characters from an incomplete input string.
        /// </summary>
        /// <param name="query">The query definition specifying the string or strings to search for.</param>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="tokenOrder">The order in which to search for tokens.</param>
        /// <param name="fuzzy">The options for fuzzy search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An autocomplete search definition.</returns>
        public SearchDefinition<TDocument> Autocomplete(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return new AutocompleteSearchDefinition<TDocument>(query, path, tokenOrder, fuzzy, score);
        }

        /// <summary>
        /// Creates a search definition that performs a search for a word or phrase that contains
        /// a sequence of characters from an incomplete search string.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The query definition specifying the string or strings to search for.</param>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="tokenOrder">The order in which to search for tokens.</param>
        /// <param name="fuzzy">The options for fuzzy search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An autocomplete search definition.</returns>
        public SearchDefinition<TDocument> Autocomplete<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return Autocomplete(query, new ExpressionFieldDefinition<TDocument>(path), tokenOrder, fuzzy, score);
        }

        /// <summary>
        /// Creates a search definition that combines two or more operators into a single query.
        /// </summary>
        /// <returns></returns>
        public CompoundFluent<TDocument> Compound()
        {
            return new CompoundFluentImpl<TDocument>();
        }

        /// <summary>
        /// Creates a search definition that queries for documents where an indexed field is equal
        /// to the specified value.
        /// </summary>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="value">The value to query for.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An equality search definition.</returns>
        public SearchDefinition<TDocument> Eq(
            FieldDefinition<TDocument, bool> path,
            bool value,
            ScoreDefinition<TDocument> score = null)
        {
            return new EqSearchDefinition<TDocument>(path, new BsonBoolean(value), score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where an indexed field is equal
        /// to the specified value.
        /// </summary>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="value">The value to query for.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An equality search definition.</returns>
        public SearchDefinition<TDocument> Eq(
            FieldDefinition<TDocument, ObjectId> path,
            ObjectId value,
            ScoreDefinition<TDocument> score = null)
        {
            return new EqSearchDefinition<TDocument>(path, value, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where an indexed field is equal
        /// to the specified value.
        /// </summary>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="value">The value to query for.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An equality search definition.</returns>
        public SearchDefinition<TDocument> Eq(
            Expression<Func<TDocument, bool>> path,
            bool value,
            ScoreDefinition<TDocument> score = null)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, bool>(path), value, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where an indexed field is equal
        /// to the specified value.
        /// </summary>
        /// <param name="path">The indexed field to search.</param>
        /// <param name="value">The value to query for.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>An equality search definition.</returns>
        public SearchDefinition<TDocument> Eq(
            Expression<Func<TDocument, ObjectId>> path,
            ObjectId value,
            ScoreDefinition<TDocument> score = null)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, ObjectId>(path), value, score);
        }

        /// <summary>
        /// Creates a search definition that tests if a path to a specified indexed field name
        /// exists in a document.
        /// </summary>
        /// <param name="path">The field to test for.</param>
        /// <returns>An existence search definition.</returns>
        public SearchDefinition<TDocument> Exists(FieldDefinition<TDocument> path)
        {
            return new ExistsSearchDefinition<TDocument>(path);
        }

        /// <summary>
        /// Creates a search definition that tests if a path to a specified indexed field name
        /// exists in a document.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="path">The field to test for.</param>
        /// <returns>An existence search definition.</returns>
        public SearchDefinition<TDocument> Exists<TField>(Expression<Func<TDocument, TField>> path)
        {
            return Exists(new ExpressionFieldDefinition<TDocument>(path));
        }

        /// <summary>
        /// Creates a search definition that groups results by values or ranges in the specified
        /// faceted fields and returns the count for each of those groups.
        /// </summary>
        /// <param name="operator">The operator to use to perform the facet over.</param>
        /// <param name="facets">Information for bucketing the data for each facet.</param>
        /// <returns>A facet search definition.</returns>
        public SearchDefinition<TDocument> Facet(
            SearchDefinition<TDocument> @operator,
            IEnumerable<SearchFacet<TDocument>> facets)
        {
            return new FacetSearchDefinition<TDocument>(@operator, facets);
        }

        /// <summary>
        /// Creates a search definition that groups results by values or ranges in the specified
        /// faceted fields and returns the count for each of those groups.
        /// </summary>
        /// <param name="operator">The operator to use to perform the facet over.</param>
        /// <param name="facets">Information for bucketing the data for each facet.</param>
        /// <returns>A facet search definition.</returns>
        public SearchDefinition<TDocument> Facet(
            SearchDefinition<TDocument> @operator,
            params SearchFacet<TDocument>[] facets)
        {
            return Facet(@operator, (IEnumerable<SearchFacet<TDocument>>)facets);
        }

        /// <summary>
        /// Creates a search definition that queries for shapes with a given geometry.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <param name="geometry">
        /// GeoJSON object specifying the Polygon, MultiPolygon, or LineString shape or point
        /// to search.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="relation">
        /// Relation of the query shape geometry to the indexed field geometry.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo shape search definition.</returns>
        public SearchDefinition<TDocument> GeoShape<TCoordinates>(
            GeoJsonGeometry<TCoordinates> geometry,
            PathDefinition<TDocument> path,
            GeoShapeRelation relation,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return new GeoShapeSearchDefinition<TDocument, TCoordinates>(geometry, path, relation, score);
        }

        /// <summary>
        /// Creates a search definition that queries for shapes with a given geometry.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="geometry">
        /// GeoJSON object specifying the Polygon, MultiPolygon, or LineString shape or point
        /// to search.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="relation">
        /// Relation of the query shape geometry to the indexed field geometry.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo shape search definition.</returns>
        public SearchDefinition<TDocument> GeoShape<TCoordinates, TField>(
            GeoJsonGeometry<TCoordinates> geometry,
            Expression<Func<TDocument, TField>> path,
            GeoShapeRelation relation,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return GeoShape(
                geometry,
                new ExpressionFieldDefinition<TDocument>(path),
                relation,
                score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// geometry.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <param name="geometry">
        /// GeoJSON object specifying the MultiPolygon or Polygon to search within.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates>(
            GeoJsonGeometry<TCoordinates> geometry,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return new GeoWithinSearchDefinition<TDocument, TCoordinates>(geometry, path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// geometry.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="geometry">
        /// GeoJSON object specifying the MultiPolygon or Polygon to search within.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates, TField>(
            GeoJsonGeometry<TCoordinates> geometry,
            Expression<Func<TDocument, TField>> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return GeoWithin(geometry, new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// box.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <param name="box">
        /// Object that specifies the bottom left and top right GeoJSON points of a box
        /// to search within.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates>(
            GeoWithinBox<TCoordinates> box,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return new GeoWithinSearchDefinition<TDocument, TCoordinates>(box, path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// box.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="box">
        /// Object that specifies the bottom left and top right GeoJSON points of a box
        /// to search within.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates, TField>(
            GeoWithinBox<TCoordinates> box,
            Expression<Func<TDocument, TField>> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return GeoWithin(box, new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// circle.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <param name="circle">
        /// Object that specifies the center point and the radius in meters to search
        /// within.
        /// </param>
        /// <param name="path">Indexed geo type field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates>(
            GeoWithinCircle<TCoordinates> circle,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return new GeoWithinSearchDefinition<TDocument, TCoordinates>(circle, path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for geographic points within a given
        /// circle.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="circle">
        /// Object that specifies the center point and the radius in meters to search
        /// within.
        /// </param>
        /// <param name="path">Indexed geo type field or field to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A geo within search definition.</returns>
        public SearchDefinition<TDocument> GeoWithin<TCoordinates, TField>(
            GeoWithinCircle<TCoordinates> circle,
            Expression<Func<TDocument, TField>> path,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return GeoWithin(circle, new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            double origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDouble(origin), new BsonDouble(pivot), score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            double origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            int origin,
            int pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt32(origin), new BsonInt32(pivot), score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            int origin,
            int pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            long origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt64(origin), new BsonInt64(pivot), score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            long origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            DateTime origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDateTime(origin), new BsonInt64(pivot), score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            DateTime origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to use to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TCoordinates>(
            PathDefinition<TDocument> path,
            GeoJsonPoint<TCoordinates> origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return new NearSearchDefinition<TDocument>(path, origin.ToBsonDocument(), pivot, score);
        }

        /// <summary>
        /// Creates a search definition that supports querying and scoring numeric and date values.
        /// </summary>
        /// <typeparam name="TCoordinates">The type of the coordinates</typeparam>
        /// <typeparam name="TField">The type of the fields.</typeparam>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="origin">The number, date, or geographic point to search near.</param>
        /// <param name="pivot">The value to user to calculate scores of result documents.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A near search definition.</returns>
        public SearchDefinition<TDocument> Near<TCoordinates, TField>(
            Expression<Func<TDocument, TField>> path,
            GeoJsonPoint<TCoordinates> origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
            where TCoordinates : GeoJsonCoordinates
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        /// <summary>
        /// Creates a search definition that performs search for documents containing an ordered
        /// sequence of terms.
        /// </summary>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="slop">The allowable distance between words in the query phrase.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A phrase search definition.</returns>
        public SearchDefinition<TDocument> Phrase(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            int slop = 0,
            ScoreDefinition<TDocument> score = null)
        {
            return new PhraseSearchDefinition<TDocument>(query, path, slop, score);
        }

        /// <summary>
        /// Creates a search definition that performs search for documents containing an ordered
        /// sequence of terms.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="slop">The allowable distance between words in the query phrase.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A phrase search definition.</returns>
        public SearchDefinition<TDocument> Phrase<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            int slop = 0,
            ScoreDefinition<TDocument> score = null)
        {
            return Phrase(query, new ExpressionFieldDefinition<TDocument>(path), slop, score);
        }

        /// <summary>
        /// Creates a search definition that queries a combination of indexed fields and values.
        /// </summary>
        /// <param name="defaultPath">The indexed field to search by default.</param>
        /// <param name="query">One or more indexed fields and values to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A query string search definition.</returns>
        public SearchDefinition<TDocument> QueryString(
            FieldDefinition<TDocument> defaultPath,
            string query,
            ScoreDefinition<TDocument> score = null)
        {
            return new QueryStringSearchDefinition<TDocument>(defaultPath, query, score);
        }

        /// <summary>
        /// Creates a search definition that queries a combination of indexed fields and values.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="defaultPath">The indexed field to search by default.</param>
        /// <param name="query">One or more indexed fields and values to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A query string search definition.</returns>
        public SearchDefinition<TDocument> QueryString<TField>(
            Expression<Func<TDocument, TField>> defaultPath,
            string query,
            ScoreDefinition<TDocument> score = null)
        {
            return QueryString(new ExpressionFieldDefinition<TDocument>(defaultPath), query, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a date/time field is in
        /// the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, DateTime> RangeDateTime(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
        {
            return new RangeFluentImpl<TDocument, DateTime, DateTimeBsonValueFactory>(path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a date/time field is in
        /// the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, DateTime> RangeDateTime(
            Expression<Func<TDocument, DateTime>> path,
            ScoreDefinition<TDocument> score = null)
        {
            return RangeDateTime(new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a floating-point
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, double> RangeDouble(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
        {
            return new RangeFluentImpl<TDocument, double, DoubleBsonValueFactory>(path, score);
        }


        /// <summary>
        /// Creates a search definition that queries for documents where a floating-point
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, double> RangeDouble(
            Expression<Func<TDocument, double>> path,
            ScoreDefinition<TDocument> score = null)
        {
            return RangeDouble(new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a 32-bit integer
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, int> RangeInt32(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
        {
            return new RangeFluentImpl<TDocument, int, Int32BsonValueFactory>(path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a 32-bit integer
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, int> RangeInt32(
            Expression<Func<TDocument, int>> path,
            ScoreDefinition<TDocument> score = null)
        {
            return RangeInt32(new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a 64-bit integer
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, long> RangeInt64(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
        {
            return new RangeFluentImpl<TDocument, long, Int64BsonValueFactory>(path, score);
        }

        /// <summary>
        /// Creates a search definition that queries for documents where a 64-bit integer
        /// field is in the specified range.
        /// </summary>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A fluent range interface.</returns>
        public IRangeFluent<TDocument, long> RangeInt64(
            Expression<Func<TDocument, long>> path,
            ScoreDefinition<TDocument> score = null)
        {
            return RangeInt64(new ExpressionFieldDefinition<TDocument>(path), score);
        }

        /// <summary>
        /// Creates a search definition that interprets the query as a regular expression.
        /// </summary>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="allowAnalyzedField">
        /// Must be set to true if the query is run against an analyzed field.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A regular expression search definition.</returns>
        public SearchDefinition<TDocument> Regex(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return new RegexSearchDefinition<TDocument>(query, path, allowAnalyzedField, score);
        }

        /// <summary>
        /// Creates a search definition that interprets the query as a regular expression.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="allowAnalyzedField">
        /// Must be set to true if the query is run against an analyzed field.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A regular expression search definition.</returns>
        public SearchDefinition<TDocument> Regex<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return Regex(query, new ExpressionFieldDefinition<TDocument>(path), allowAnalyzedField, score);
        }

        /// <summary>
        /// Creates a search definition that finds text search matches within regions of a text
        /// field.
        /// </summary>
        /// <param name="clause">The span clause.</param>
        /// <returns>A span search definition.</returns>
        public SearchDefinition<TDocument> Span(SpanDefinition<TDocument> clause)
        {
            return new SpanSearchDefinition<TDocument>(clause);
        }

        /// <summary>
        /// Creates a search definition that performs full-text search using the analyzer specified
        /// in the index configuration.
        /// </summary>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="fuzzy">The options for fuzzy search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A text search definition.</returns>
        public SearchDefinition<TDocument> Text(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return new TextSearchDefinition<TDocument>(query, path, fuzzy, score);
        }

        /// <summary>
        /// Creates a search definition that performs full-text search using the analyzer specified
        /// in the index configuration.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or field to search.</param>
        /// <param name="fuzzy">The options for fuzzy search.</param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A text search definition.</returns>
        public SearchDefinition<TDocument> Text<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument>(path), fuzzy, score);
        }

        /// <summary>
        /// Creates a search definition that uses special characters in the search string that can
        /// match any character.
        /// </summary>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="allowAnalyzedField">
        /// Must be set to true if the query is run against an analyzed field.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A wildcard search definition.</returns>
        public SearchDefinition<TDocument> Wildcard(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return new WildcardSearchDefinition<TDocument>(query, path, allowAnalyzedField, score);
        }

        /// <summary>
        /// Creates a search definition that uses special characters in the search string that can
        /// match any character.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="query">The string or strings to search for.</param>
        /// <param name="path">The indexed field or fields to search.</param>
        /// <param name="allowAnalyzedField">
        /// Must be set to true if the query is run against an analyzed field.
        /// </param>
        /// <param name="score">The score modifier.</param>
        /// <returns>A wildcard search definition.</returns>
        public SearchDefinition<TDocument> Wildcard<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return Wildcard(query, new ExpressionFieldDefinition<TDocument>(path), allowAnalyzedField, score);
        }
    }

    internal sealed class AutocompleteSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly AutocompleteTokenOrder _tokenOrder;
        private readonly FuzzyOptions _fuzzy;
        private readonly ScoreDefinition<TDocument> _score;

        public AutocompleteSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder,
            FuzzyOptions fuzzy,
            ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _tokenOrder = tokenOrder;
            _fuzzy = fuzzy;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["query"] = _query.Render(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_tokenOrder == AutocompleteTokenOrder.Sequential)
            {
                document.Add("tokenOrder", "sequential");
            }
            if (_fuzzy != null)
            {
                document.Add("fuzzy", _fuzzy.Render());
            }
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("autocomplete", document);
        }
    }

    internal sealed class CompoundSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<SearchDefinition<TDocument>> _must;
        private readonly List<SearchDefinition<TDocument>> _mustNot;
        private readonly List<SearchDefinition<TDocument>> _should;
        private readonly List<SearchDefinition<TDocument>> _filter;
        private readonly int _minimumShouldMatch;

        public CompoundSearchDefinition(
            List<SearchDefinition<TDocument>> must,
            List<SearchDefinition<TDocument>> mustNot,
            List<SearchDefinition<TDocument>> should,
            List<SearchDefinition<TDocument>> filter,
            int minimumShouldMatch)
        {
            // This constructor should always be called from a fluent interface that
            // ensures that the parameters are not null and copies the lists, so there is
            // no need to do any of that here.
            _must = must;
            _mustNot = mustNot;
            _should = should;
            _filter = filter;
            _minimumShouldMatch = minimumShouldMatch;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            if (_must != null)
            {
                var mustDocs = _must.Select(clause => clause.Render(documentSerializer, serializerRegistry));
                document.Add("must", new BsonArray(mustDocs));
            }
            if (_mustNot != null)
            {
                var mustNotDocs = _mustNot.Select(clause => clause.Render(documentSerializer, serializerRegistry));
                document.Add("mustNot", new BsonArray(mustNotDocs));
            }
            if (_should != null)
            {
                var shouldDocs = _should.Select(clause => clause.Render(documentSerializer, serializerRegistry));
                document.Add("should", new BsonArray(shouldDocs));
            }
            if (_filter != null)
            {
                var filterDocs = _filter.Select(clause => clause.Render(documentSerializer, serializerRegistry));
                document.Add("filter", new BsonArray(filterDocs));
            }
            if (_minimumShouldMatch > 0)
            {
                document.Add("minimumShouldMatch", _minimumShouldMatch);
            }
            return new BsonDocument("compound", document);
        }
    }

    internal sealed class EqSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _path;
        private readonly BsonValue _value;
        private readonly ScoreDefinition<TDocument> _score;

        public EqSearchDefinition(FieldDefinition<TDocument> path, BsonValue value, ScoreDefinition<TDocument> score)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _value = value;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _path.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument
            {
                ["path"] = renderedField.FieldName,
                ["value"] = _value
            };
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("equals", document);
        }
    }

    internal sealed class ExistsSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _path;

        public ExistsSearchDefinition(FieldDefinition<TDocument> path)
        {
            _path = path;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _path.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument("path", renderedField.FieldName);
            return new BsonDocument("exists", document);
        }
    }

    internal sealed class FacetSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly SearchDefinition<TDocument> _operator;
        private readonly IEnumerable<SearchFacet<TDocument>> _facets;

        public FacetSearchDefinition(SearchDefinition<TDocument> @operator, IEnumerable<SearchFacet<TDocument>> facets)
        {
            _operator = Ensure.IsNotNull(@operator, nameof(@operator));
            _facets = Ensure.IsNotNull(facets, nameof(facets));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("operator", _operator.Render(documentSerializer, serializerRegistry));
            var facetsDocument = new BsonDocument();
            foreach (var facet in _facets)
            {
                facetsDocument.Add(facet.Name, facet.Render(documentSerializer, serializerRegistry));
            }
            document.Add("facets", facetsDocument);
            return new BsonDocument("facet", document);
        }
    }

    internal sealed class GeoShapeSearchDefinition<TDocument, TCoordinates> : SearchDefinition<TDocument>
        where TCoordinates : GeoJsonCoordinates
    {
        private readonly GeoJsonGeometry<TCoordinates> _geometry;
        private readonly PathDefinition<TDocument> _path;
        private readonly GeoShapeRelation _relation;
        private readonly ScoreDefinition<TDocument> _score;

        public GeoShapeSearchDefinition(
            GeoJsonGeometry<TCoordinates> geometry,
            PathDefinition<TDocument> path,
            GeoShapeRelation relation,
            ScoreDefinition<TDocument> score)
        {
            _geometry = Ensure.IsNotNull(geometry, nameof(geometry));
            _path = Ensure.IsNotNull(path, nameof(path));
            _relation = relation;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["geometry"] = _geometry.ToBsonDocument(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry),
                ["relation"] = RelationToString(_relation)
            };
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("geoShape", document);
        }

        private static string RelationToString(GeoShapeRelation relation)
        {
            switch (relation)
            {
                case GeoShapeRelation.Contains:
                    return "contains";
                case GeoShapeRelation.Disjoint:
                    return "disjoint";
                case GeoShapeRelation.Intersects:
                    return "intersects";
                case GeoShapeRelation.Within:
                    return "within";
                default:
                    throw new ArgumentException($"Invalid relation: {relation}", nameof(relation));
            }
        }
    }

    internal sealed class GeoWithinSearchDefinition<TDocument, TCoordinates> : SearchDefinition<TDocument>
        where TCoordinates : GeoJsonCoordinates
    {
        private readonly GeoJsonGeometry<TCoordinates> _geometry;
        private readonly GeoWithinBox<TCoordinates> _box;
        private readonly GeoWithinCircle<TCoordinates> _circle;
        private readonly PathDefinition<TDocument> _path;
        private readonly ScoreDefinition<TDocument> _score;

        public GeoWithinSearchDefinition(
            GeoJsonGeometry<TCoordinates> geometry,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score)
        {
            _geometry = Ensure.IsNotNull(geometry, nameof(geometry));
            _path = Ensure.IsNotNull(path, nameof(path));
            _score = score;
        }

        public GeoWithinSearchDefinition(
            GeoWithinBox<TCoordinates> box,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score)
        {
            _box = Ensure.IsNotNull(box, nameof(box));
            _path = Ensure.IsNotNull(path, nameof(path));
            _score = score;
        }

        public GeoWithinSearchDefinition(
            GeoWithinCircle<TCoordinates> circle,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score)
        {
            _circle = Ensure.IsNotNull(circle, nameof(circle));
            _path = Ensure.IsNotNull(path, nameof(path));
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            if (_geometry != null)
            {
                document.Add("geometry", _geometry.ToBsonDocument());
            }
            if (_box != null)
            {
                document.Add("box", _box.Render());
            }
            if (_circle != null)
            {
                document.Add("circle", _circle.Render());
            }
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("geoWithin", document);
        }
    }

    internal sealed class NearSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly BsonValue _origin;
        private readonly BsonValue _pivot;
        private readonly ScoreDefinition<TDocument> _score;

        public NearSearchDefinition(
            PathDefinition<TDocument> path,
            BsonValue origin,
            BsonValue pivot,
            ScoreDefinition<TDocument> score = null)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _origin = origin;
            _pivot = pivot;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["path"] = _path.Render(documentSerializer, serializerRegistry),
                ["origin"] = _origin,
                ["pivot"] = _pivot
            };
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("near", document);
        }
    }

    internal sealed class PhraseSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly int _slop;
        private readonly ScoreDefinition<TDocument> _score;

        public PhraseSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            int slop,
            ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _slop = slop;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["query"] = _query.Render(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_slop != 0)
            {
                document.Add("slop", _slop);
            }
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("phrase", document);
        }
    }

    internal sealed class QueryStringSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _defaultPath;
        private readonly string _query;
        private readonly ScoreDefinition<TDocument> _score;

        public QueryStringSearchDefinition(FieldDefinition<TDocument> defaultPath, string query, ScoreDefinition<TDocument> score)
        {
            _defaultPath = Ensure.IsNotNull(defaultPath, nameof(defaultPath));
            _query = Ensure.IsNotNull(query, nameof(query));
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _defaultPath.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument
            {
                ["defaultPath"] = renderedField.FieldName,
                ["query"] = _query
            };
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("queryString", document);
        }
    }

    internal sealed class RangeSearchDefinition<TDocument, TValue, TFactory> : SearchDefinition<TDocument>
        where TValue : struct
        where TFactory : IBsonValueFactory<TValue>, new()
    {
        private readonly PathDefinition<TDocument> _path;
        private readonly ScoreDefinition<TDocument> _score;
        private readonly TValue? _min;
        private readonly bool _minInclusive;
        private readonly TValue? _max;
        private readonly bool _maxInclusive;

        public RangeSearchDefinition(
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score,
            TValue? min,
            bool minInclusive,
            TValue? max,
            bool maxInclusive)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _score = score;
            _min = min;
            _minInclusive = minInclusive;
            _max = max;
            _maxInclusive = maxInclusive;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            if (_min.HasValue)
            {
                var factory = new TFactory();
                document.Add(_minInclusive ? "gte" : "gt", factory.Create(_min.Value));
            }
            if (_max.HasValue)
            {
                var factory = new TFactory();
                document.Add(_maxInclusive ? "lte" : "lt", factory.Create(_max.Value));
            }
            return new BsonDocument("range", document);
        }
    }

    internal sealed class RegexSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly bool _allowAnalyzedField;
        private readonly ScoreDefinition<TDocument> _score;

        public RegexSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField,
            ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _allowAnalyzedField = allowAnalyzedField;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["query"] = _query.Render(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_allowAnalyzedField)
            {
                document.Add("allowAnalyzedField", true);
            }
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("regex", document);
        }
    }

    internal sealed class SpanSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly SpanDefinition<TDocument> _clause;

        public SpanSearchDefinition(SpanDefinition<TDocument> clause)
        {
            _clause = Ensure.IsNotNull(clause, nameof(clause));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedClause = _clause.Render(documentSerializer, serializerRegistry);
            return new BsonDocument("span", renderedClause);
        }
    }

    internal sealed class TextSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly FuzzyOptions _fuzzy;
        private readonly ScoreDefinition<TDocument> _score;

        public TextSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            FuzzyOptions fuzzy,
            ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _fuzzy = fuzzy;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["query"] = _query.Render(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_fuzzy != null)
            {
                document.Add("fuzzy", _fuzzy.Render());
            }
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("text", document);
        }
    }

    internal sealed class WildcardSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly bool _allowAnalyzedField;
        private readonly ScoreDefinition<TDocument> _score;

        public WildcardSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField,
            ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _allowAnalyzedField = allowAnalyzedField;
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument
            {
                ["query"] = _query.Render(),
                ["path"] = _path.Render(documentSerializer, serializerRegistry)
            };
            if (_allowAnalyzedField)
            {
                document.Add("allowAnalyzedField", true);
            }
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("wildcard", document);
        }
    }
}
