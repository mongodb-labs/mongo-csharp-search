using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Search
{
    public sealed class SearchDefinitionBuilder<TDocument>
    {
        public SearchDefinition<TDocument> Autocomplete(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return new AutocompleteSearchDefinition<TDocument>(query, path, tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete(
            IEnumerable<string> query,
            FieldDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, new[] { path }, tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete(
            string query,
            IEnumerable<FieldDefinition<TDocument>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(new[] { query }, path, tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete(
            string query,
            FieldDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(new[] { query }, new[] { path }, tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete<TField>(
            string query,
            Expression<Func<TDocument, TField>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, new ExpressionFieldDefinition<TDocument, TField>(path), tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete(
            string query,
            IEnumerable<string> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete<TField>(
            IEnumerable<string> query,
            Expression<Func<TDocument, TField>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, new ExpressionFieldDefinition<TDocument, TField>(path), tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete(
            IEnumerable<string> query,
            IEnumerable<string> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), tokenOrder);
        }

        public SearchDefinition<TDocument> Eq(FieldDefinition<TDocument, bool> path, bool value)
        {
            return new EqSearchDefinition<TDocument>(path, new BsonBoolean(value));
        }

        public SearchDefinition<TDocument> Eq(FieldDefinition<TDocument, ObjectId> path, ObjectId value)
        {
            return new EqSearchDefinition<TDocument>(path, value);
        }

        public SearchDefinition<TDocument> Eq(Expression<Func<TDocument, bool>> path, bool value)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, bool>(path), value);
        }

        public SearchDefinition<TDocument> Eq(Expression<Func<TDocument, ObjectId>> path, ObjectId value)
        {
            return Eq(new ExpressionFieldDefinition<TDocument, ObjectId>(path), value);
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

        public SearchDefinition<TDocument> Near(IEnumerable<FieldDefinition<TDocument>> path, double origin, double pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDouble(origin), new BsonDouble(pivot));
        }

        public SearchDefinition<TDocument> Near(FieldDefinition<TDocument> path, double origin, double pivot)
        {
            return Near(new[] { path }, origin, pivot);
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, double origin, double pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(IEnumerable<FieldDefinition<TDocument>> path, int origin, int pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt32(origin), new BsonInt32(pivot));
        }

        public SearchDefinition<TDocument> Near(FieldDefinition<TDocument> path, int origin, int pivot)
        {
            return Near(new[] { path }, origin, pivot);
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, int origin, int pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(IEnumerable<FieldDefinition<TDocument>> path, long origin, long pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt64(origin), new BsonInt64(pivot));
        }

        public SearchDefinition<TDocument> Near(FieldDefinition<TDocument> path, long origin, long pivot)
        {
            return Near(new[] { path }, origin, pivot);
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, long origin, long pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(IEnumerable<FieldDefinition<TDocument>> path, DateTime origin, long pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDateTime(origin), new BsonInt64(pivot));
        }

        public SearchDefinition<TDocument> Near(FieldDefinition<TDocument> path, DateTime origin, long pivot)
        {
            return Near(new[] { path }, origin, pivot);
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, DateTime origin, long pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Phrase(IEnumerable<string> query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return new PhraseSearchDefinition<TDocument>(query, path);
        }

        public SearchDefinition<TDocument> Phrase(IEnumerable<string> query, FieldDefinition<TDocument> path)
        {
            return Phrase(query, new[] { path });
        }

        public SearchDefinition<TDocument> Phrase(string query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return Phrase(new[] { query }, path);
        }

        public SearchDefinition<TDocument> Phrase(string query, FieldDefinition<TDocument> path)
        {
            return Phrase(new[] { query }, new[] { path });
        }

        public SearchDefinition<TDocument> Phrase<TField>(string query, Expression<Func<TDocument, TField>> path)
        {
            return Phrase(query, new ExpressionFieldDefinition<TDocument, TField>(path));
        }

        public SearchDefinition<TDocument> Phrase(string query, IEnumerable<string> path)
        {
            return Phrase(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
        }

        public SearchDefinition<TDocument> Phrase<TField>(IEnumerable<string> query, Expression<Func<TDocument, TField>> path)
        {
            return Phrase(query, new ExpressionFieldDefinition<TDocument, TField>(path));
        }

        public SearchDefinition<TDocument> Phrase(IEnumerable<string> query, IEnumerable<string> path)
        {
            return Phrase(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
        }

        public SearchDefinition<TDocument> QueryString(FieldDefinition<TDocument> defaultPath, string query)
        {
            return new QueryStringSearchDefinition<TDocument>(defaultPath, query);
        }

        public SearchDefinition<TDocument> QueryString<TField>(Expression<Func<TDocument, TField>> defaultPath, string query)
        {
            return QueryString(new ExpressionFieldDefinition<TDocument>(defaultPath), query);
        }

        public SearchDefinition<TDocument> Regex(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField = false)
        {
            return new RegexSearchDefinition<TDocument>(query, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex(
            IEnumerable<string> query,
            FieldDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return Regex(query, new[] { path }, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex(
            string query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField = false)
        {
            return Regex(new[] { query }, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex(
            string query,
            FieldDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return Regex(new[] { query }, new[] { path }, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex<TField>(
            string query,
            Expression<Func<TDocument, TField>> field,
            bool allowAnalyzedField = false)
        {
            return Regex(query, new ExpressionFieldDefinition<TDocument, TField>(field), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex(
            string query,
            IEnumerable<string> path,
            bool allowAnalyzedField = false)
        {
            return Regex(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex<TField>(
            IEnumerable<string> query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false)
        {
            return Regex(query, new ExpressionFieldDefinition<TDocument, TField>(path), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex(
            IEnumerable<string> query,
            IEnumerable<string> path,
            bool allowAnalyzedField = false)
        {
            return Regex(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Should(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("should", clauses);
        }

        public SearchDefinition<TDocument> Should(params SearchDefinition<TDocument>[] clauses)
        {
            return Should((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> Text(IEnumerable<string> query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return new TextSearchDefinition<TDocument>(query, path);
        }

        public SearchDefinition<TDocument> Text(IEnumerable<string> query, FieldDefinition<TDocument> path)
        {
            return Text(query, new[] { path });
        }

        public SearchDefinition<TDocument> Text(string query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return Text(new[] { query }, path);
        }

        public SearchDefinition<TDocument> Text(string query, FieldDefinition<TDocument> path)
        {
            return Text(new[] { query }, new[] { path });
        }

        public SearchDefinition<TDocument> Text<TField>(string query, Expression<Func<TDocument, TField>> field)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument, TField>(field));
        }

        public SearchDefinition<TDocument> Text(string query, IEnumerable<string> path)
        {
            return Text(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
        }

        public SearchDefinition<TDocument> Text<TField>(IEnumerable<string> query, Expression<Func<TDocument, TField>> path)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument, TField>(path));
        }

        public SearchDefinition<TDocument> Text(IEnumerable<string> query, IEnumerable<string> path)
        {
            return Text(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
        }

        public SearchDefinition<TDocument> Wildcard(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField = false)
        {
            return new WildcardSearchDefinition<TDocument>(query, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard(
            IEnumerable<string> query,
            FieldDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, new[] { path }, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard(
            string query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(new[] { query }, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard(
            string query,
            FieldDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(new[] { query }, new[] { path }, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard<TField>(
            string query,
            Expression<Func<TDocument, TField>> field,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, new ExpressionFieldDefinition<TDocument, TField>(field), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard(
            string query,
            IEnumerable<string> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard<TField>(
            IEnumerable<string> query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, new ExpressionFieldDefinition<TDocument, TField>(path), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard(
            IEnumerable<string> query,
            IEnumerable<string> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, path.Select(field => new StringFieldDefinition<TDocument>(field)), allowAnalyzedField);
        }
    }

    internal sealed class AutocompleteSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<string> _query;
        private readonly List<FieldDefinition<TDocument>> _path;
        private readonly AutocompleteTokenOrder _tokenOrder;

        public AutocompleteSearchDefinition(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            AutocompleteTokenOrder tokenOrder)
        {
            _query = Ensure.IsNotNull(query, nameof(query)).ToList();
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
            _tokenOrder = tokenOrder;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue queryVal;
            if (_query.Count == 1)
            {
                queryVal = new BsonString(_query[0]);
            }
            else
            {
                queryVal = new BsonArray(_query);
            }

            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            if (_tokenOrder == AutocompleteTokenOrder.Sequential)
            {
                doc.Add("tokenOrder", "sequential");
            }
            return new BsonDocument("autocomplete", doc);
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
            var doc = new BsonDocument(_term, new BsonArray(clauseDocs));
            return new BsonDocument("compound", doc);
        }
    }

    internal sealed class EqSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _path;
        private readonly BsonValue _value;

        public EqSearchDefinition(FieldDefinition<TDocument> path, BsonValue value)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _value = value;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _path.Render(documentSerializer, serializerRegistry);
            var doc = new BsonDocument();
            doc.Add("path", renderedField.FieldName);
            doc.Add("value", _value);
            return new BsonDocument("equals", doc);
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
            var doc = new BsonDocument("path", renderedField.FieldName);
            return new BsonDocument("exists", doc);
        }
    }

    internal sealed class NearSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<FieldDefinition<TDocument>> _path;
        private readonly BsonValue _origin;
        private readonly BsonValue _pivot;

        public NearSearchDefinition(IEnumerable<FieldDefinition<TDocument>> path, BsonValue origin, BsonValue pivot)
        {
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
            _origin = origin;
            _pivot = pivot;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("path", pathVal);
            doc.Add("origin", _origin);
            doc.Add("pivot", _pivot);
            return new BsonDocument("near", doc);
        }
    }

    internal sealed class PhraseSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<string> _query;
        private readonly List<FieldDefinition<TDocument>> _path;

        public PhraseSearchDefinition(IEnumerable<string> query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            _query = Ensure.IsNotNull(query, nameof(query)).ToList();
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue queryVal;
            if (_query.Count == 1)
            {
                queryVal = new BsonString(_query[0]);
            }
            else
            {
                queryVal = new BsonArray(_query);
            }

            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            return new BsonDocument("phrase", doc);
        }
    }

    internal sealed class QueryStringSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly FieldDefinition<TDocument> _defaultPath;
        private readonly string _query;

        public QueryStringSearchDefinition(FieldDefinition<TDocument> defaultPath, string query)
        {
            _defaultPath = Ensure.IsNotNull(defaultPath, nameof(defaultPath));
            _query = Ensure.IsNotNull(query, nameof(query));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _defaultPath.Render(documentSerializer, serializerRegistry);
            var doc = new BsonDocument();
            doc.Add("defaultPath", renderedField.FieldName);
            doc.Add("query", _query);
            return new BsonDocument("queryString", doc);
        }
    }

    internal sealed class RegexSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<string> _query;
        private readonly List<FieldDefinition<TDocument>> _path;
        private bool _allowAnalyzedField;

        public RegexSearchDefinition(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField)
        {
            _query = Ensure.IsNotNull(query, nameof(query)).ToList();
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
            _allowAnalyzedField = allowAnalyzedField;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue queryVal;
            if (_query.Count == 1)
            {
                queryVal = new BsonString(_query[0]);
            }
            else
            {
                queryVal = new BsonArray(_query);
            }

            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            if (_allowAnalyzedField)
            {
                doc.Add("allowAnalyzedField", true);
            }
            return new BsonDocument("regex", doc);
        }
    }

    internal sealed class TextSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<string> _query;
        private readonly List<FieldDefinition<TDocument>> _path;

        public TextSearchDefinition(IEnumerable<string> query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            _query = Ensure.IsNotNull(query, nameof(query)).ToList();
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue queryVal;
            if (_query.Count == 1)
            {
                queryVal = new BsonString(_query[0]);
            }
            else
            {
                queryVal = new BsonArray(_query);
            }

            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            return new BsonDocument("text", doc);
        }
    }

    internal sealed class WildcardSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly List<string> _query;
        private readonly List<FieldDefinition<TDocument>> _path;
        private bool _allowAnalyzedField;

        public WildcardSearchDefinition(
            IEnumerable<string> query,
            IEnumerable<FieldDefinition<TDocument>> path,
            bool allowAnalyzedField)
        {
            _query = Ensure.IsNotNull(query, nameof(query)).ToList();
            _path = Ensure.IsNotNull(path, nameof(path)).ToList();
            _allowAnalyzedField = allowAnalyzedField;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            BsonValue queryVal;
            if (_query.Count == 1)
            {
                queryVal = new BsonString(_query[0]);
            }
            else
            {
                queryVal = new BsonArray(_query);
            }

            BsonValue pathVal;
            if (_path.Count == 1)
            {
                var renderedField = _path[0].Render(documentSerializer, serializerRegistry);
                pathVal = new BsonString(renderedField.FieldName);
            }
            else
            {
                pathVal = new BsonArray(_path.Select(field =>
                {
                    var renderedField = field.Render(documentSerializer, serializerRegistry);
                    return new BsonString(renderedField.FieldName);
                }));
            }

            var doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            if (_allowAnalyzedField)
            {
                doc.Add("allowAnalyzedField", true);
            }
            return new BsonDocument("wildcard", doc);
        }
    }
}
