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
            QueryDefinition query,
            PathDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return new AutocompleteSearchDefinition<TDocument>(query, path, tokenOrder);
        }

        public SearchDefinition<TDocument> Autocomplete<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            AutocompleteTokenOrder tokenOrder = AutocompleteTokenOrder.Any)
        {
            return Autocomplete(query, new ExpressionFieldDefinition<TDocument>(path), tokenOrder);
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

        public SearchDefinition<TDocument> Near(PathDefinition<TDocument> path, double origin, double pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDouble(origin), new BsonDouble(pivot));
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, double origin, double pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(PathDefinition<TDocument> path, int origin, int pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt32(origin), new BsonInt32(pivot));
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, int origin, int pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(PathDefinition<TDocument> path, long origin, long pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonInt64(origin), new BsonInt64(pivot));
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, long origin, long pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Near(PathDefinition<TDocument> path, DateTime origin, long pivot)
        {
            return new NearSearchDefinition<TDocument>(path, new BsonDateTime(origin), new BsonInt64(pivot));
        }

        public SearchDefinition<TDocument> Near<TField>(Expression<Func<TDocument, TField>> path, DateTime origin, long pivot)
        {
            return Near(new ExpressionFieldDefinition<TDocument>(path), origin, pivot);
        }

        public SearchDefinition<TDocument> Phrase(QueryDefinition query, PathDefinition<TDocument> path)
        {
            return new PhraseSearchDefinition<TDocument>(query, path);
        }

        public SearchDefinition<TDocument> Phrase<TField>(QueryDefinition query, Expression<Func<TDocument, TField>> path)
        {
            return Phrase(query, new ExpressionFieldDefinition<TDocument>(path));
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
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return new RegexSearchDefinition<TDocument>(query, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Regex<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false)
        {
            return Regex(query, new ExpressionFieldDefinition<TDocument>(path), allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Should(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            return new CompoundSearchDefinition<TDocument>("should", clauses);
        }

        public SearchDefinition<TDocument> Should(params SearchDefinition<TDocument>[] clauses)
        {
            return Should((IEnumerable<SearchDefinition<TDocument>>)clauses);
        }

        public SearchDefinition<TDocument> Text(QueryDefinition query, PathDefinition<TDocument> path)
        {
            return new TextSearchDefinition<TDocument>(query, path);
        }

        public SearchDefinition<TDocument> Text<TField>(QueryDefinition query, Expression<Func<TDocument, TField>> path)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument>(path));
        }

        public SearchDefinition<TDocument> Wildcard(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField = false)
        {
            return new WildcardSearchDefinition<TDocument>(query, path, allowAnalyzedField);
        }

        public SearchDefinition<TDocument> Wildcard<TField>(
            QueryDefinition query,
            Expression<Func<TDocument, TField>> path,
            bool allowAnalyzedField = false)
        {
            return Wildcard(query, new ExpressionFieldDefinition<TDocument>(path), allowAnalyzedField);
        }
    }

    internal sealed class AutocompleteSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private readonly AutocompleteTokenOrder _tokenOrder;

        public AutocompleteSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            AutocompleteTokenOrder tokenOrder)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _tokenOrder = tokenOrder;
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

        public EqSearchDefinition(FieldDefinition<TDocument> path, BsonValue value)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _value = value;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var renderedField = _path.Render(documentSerializer, serializerRegistry);
            var document = new BsonDocument();
            document.Add("path", renderedField.FieldName);
            document.Add("value", _value);
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

        public NearSearchDefinition(PathDefinition<TDocument> path, BsonValue origin, BsonValue pivot)
        {
            _path = Ensure.IsNotNull(path, nameof(path));
            _origin = origin;
            _pivot = pivot;
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            document.Add("origin", _origin);
            document.Add("pivot", _pivot);
            return new BsonDocument("near", document);
        }
    }

    internal sealed class PhraseSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;

        public PhraseSearchDefinition(QueryDefinition query, PathDefinition<TDocument> path)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            return new BsonDocument("phrase", document);
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
            var document = new BsonDocument();
            document.Add("defaultPath", renderedField.FieldName);
            document.Add("query", _query);
            return new BsonDocument("queryString", document);
        }
    }

    internal sealed class RegexSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private bool _allowAnalyzedField;

        public RegexSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _allowAnalyzedField = allowAnalyzedField;
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
            return new BsonDocument("regex", document);
        }
    }

    internal sealed class TextSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;

        public TextSearchDefinition(QueryDefinition query, PathDefinition<TDocument> path)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument();
            document.Add("query", _query.Render());
            document.Add("path", _path.Render(documentSerializer, serializerRegistry));
            return new BsonDocument("text", document);
        }
    }

    internal sealed class WildcardSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly QueryDefinition _query;
        private readonly PathDefinition<TDocument> _path;
        private bool _allowAnalyzedField;

        public WildcardSearchDefinition(
            QueryDefinition query,
            PathDefinition<TDocument> path,
            bool allowAnalyzedField)
        {
            _query = Ensure.IsNotNull(query, nameof(query));
            _path = Ensure.IsNotNull(path, nameof(path));
            _allowAnalyzedField = allowAnalyzedField;
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
            return new BsonDocument("wildcard", document);
        }
    }
}
