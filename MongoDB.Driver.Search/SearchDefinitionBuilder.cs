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

namespace MongoDB.Labs.Search
{
    public sealed class SearchDefinitionBuilder<TDocument>
    {
        public SearchDefinition<TDocument> Autocomplete(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return new AutocompleteSearchDefinition<TDocument>(query, path, tokenOrder, fuzzy, score);
        }

        public SearchDefinition<TDocument> Autocomplete<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return Autocomplete(query, new ExpressionFieldDefinition<TDocument>(path), tokenOrder, fuzzy, score);
        }

        public SearchDefinition<TDocument> Eq(
            FieldDefinition<TDocument, bool> path,
            bool value,
            ScoreDefinition<TDocument> score = null)
        {
            return new EqSearchDefinition<TDocument>(path, new BsonBoolean(value), score);
        }

        public SearchDefinition<TDocument> Eq(
            FieldDefinition<TDocument, ObjectId> path,
            ObjectId value,
            ScoreDefinition<TDocument> score = null)
        {
            return new EqSearchDefinition<TDocument>(path, value, score);
        }

        public SearchDefinition<TDocument> Eq(
            Expression<Func<TDocument, bool>> path,
            bool value,
            ScoreDefinition<TDocument> score = null)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, bool>(path), value, score);
        }

        public SearchDefinition<TDocument> Eq(
            Expression<Func<TDocument, ObjectId>> path,
            ObjectId value,
            ScoreDefinition<TDocument> score = null)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, ObjectId>(path), value, score);
        }

        public SearchDefinition<TDocument> Exists(FieldDefinition<TDocument> path)
        {
            return new ExistsSearchDefinition<TDocument>(path);
        }

        public SearchDefinition<TDocument> Exists<TField>(Expression<Func<TDocument, TField>> path)
        {
            return Exists(new ExpressionFieldDefinition<TDocument>(path));
        }

        public SearchDefinition<TDocument> Filter(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("filter", clauses);
        }

        public SearchDefinition<TDocument> Filter(params SearchDefinition<TDocument>[] clauses)
        {
            return Filter((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> Must(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("must", clauses);
        }

        public SearchDefinition<TDocument> Must(params SearchDefinition<TDocument>[] clauses)
        {
            return Must((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> MustNot(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("mustNot", clauses);
        }

        public SearchDefinition<TDocument> MustNot(params SearchDefinition<TDocument>[] clauses)
        {
            return MustNot((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            double origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDouble(origin), new BsonDouble(pivot), score);
        }

        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            double origin,
            double pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            int origin,
            int pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt32(origin), new BsonInt32(pivot), score);
        }

        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            int origin,
            int pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            long origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt64(origin), new BsonInt64(pivot), score);
        }

        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            long origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        public SearchDefinition<TDocument> Near(
            PathDefinition<TDocument> path,
            DateTime origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDateTime(origin), new BsonInt64(pivot), score);
        }

        public SearchDefinition<TDocument> Near<TField>(
            Expression<Func<TDocument, TField>> path,
            DateTime origin,
            long pivot,
            ScoreDefinition<TDocument> score = null)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot, score);
        }

        public SearchDefinition<TDocument> Phrase(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            ScoreDefinition<TDocument> score = null)
        {
            return new PhraseSearchDefinition<TDocument>(query, path, score);
        }

        public SearchDefinition<TDocument> Phrase<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            ScoreDefinition<TDocument> score = null)
        {
            return Phrase(query, new ExpressionFieldDefinition<TDocument>(path), score);
        }

        public SearchDefinition<TDocument> QueryString(
            FieldDefinition<TDocument> defaultPath,
            string query,
            ScoreDefinition<TDocument> score = null)
        {
            return new QueryStringSearchDefinition<TDocument>(defaultPath, query, score);
        }

        public SearchDefinition<TDocument> QueryString<TField>(
            Expression<Func<TDocument, TField>> defaultPath,
            string query,
            ScoreDefinition<TDocument> score = null)
        {
            return QueryString(new ExpressionFieldDefinition<TDocument>(defaultPath), query, score);
        }

        public SearchDefinition<TDocument> Regex(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return new RegexSearchDefinition<TDocument>(query, path, allowAnalyzedField, score);
        }

        public SearchDefinition<TDocument> Regex<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return Regex(query, new ExpressionFieldDefinition<TDocument>(path), allowAnalyzedField, score);
        }

        public SearchDefinition<TDocument> Should(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("should", clauses);
        }

        public SearchDefinition<TDocument> Should(params SearchDefinition<TDocument>[] clauses)
        {
            return Should((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> Text(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return new TextSearchDefinition<TDocument>(query, path, fuzzy, score);
        }

        public SearchDefinition<TDocument> Text<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            FuzzyOptions fuzzy = null,
            ScoreDefinition<TDocument> score = null)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument>(path), fuzzy, score);
        }

        public SearchDefinition<TDocument> Wildcard(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false,
            ScoreDefinition<TDocument> score = null)
        {
            return new WildcardSearchDefinition<TDocument>(query, path, allowAnalyzedField, score);
        }

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
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
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
        private readonly string _term;
        private readonly List<SearchDefinition<TDocument>> _clauses;

        public CompoundSearchDefinition(string term, IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            _term = Ensure.IsNotNullOrEmpty(term, nameof(term));
            _clauses = Ensure.IsNotNull(clauses, nameof(clauses)).ToList();
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var clauseDocs = _clauses.Select(clause => clause.Render(documentSerializer, serializerRegistry));
            var document = new BsonDocument(_term, new BsonArray(clauseDocs));
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
            var document = new BsonDocument();
            document.Add("path", renderedField.FieldName);
            document.Add("value", _value);
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
            var document = new BsonDocument();
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            document.Add("origin", _origin);
            document.Add("pivot", _pivot);
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
        private readonly ScoreDefinition<TDocument> _score;

        public PhraseSearchDefinition(QueryDefinition query, PathDefinition<TDocument> path, ScoreDefinition<TDocument> score)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _score = score;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
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
            var document = new BsonDocument();
            document.Add("defaultPath", renderedField.FieldName);
            document.Add("query", _query);
            if (_score != null)
            {
                document.Add("score", _score.Render(documentSerializer, serializerRegistry));
            }
            return new BsonDocument("queryString", document);
        }
    }

    internal sealed class RegexSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private bool _allowAnalyzedField;
        private ScoreDefinition<TDocument> _score;

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
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
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
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
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
        private bool _allowAnalyzedField;
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
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
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
