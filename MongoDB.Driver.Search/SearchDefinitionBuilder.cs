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
        public SearchDefinition<TDocument> Text(IEnumerable<string> query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return new TextSearchDefinition<TDocument>(query, path);
        }

        public SearchDefinition<TDocument> Text(IEnumerable<string> query, FieldDefinition<TDocument> path)
        {
            return new TextSearchDefinition<TDocument>(query, new[] { path });
        }

        public SearchDefinition<TDocument> Text(string query, IEnumerable<FieldDefinition<TDocument>> path)
        {
            return new TextSearchDefinition<TDocument>(new[] { query }, path);
        }

        public SearchDefinition<TDocument> Text(string query, FieldDefinition<TDocument> path)
        {
            return new TextSearchDefinition<TDocument>(new[] { query }, new[] { path });
        }

        public SearchDefinition<TDocument> Text<TField>(string query, Expression<Func<TDocument, TField>> field)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument, TField>(field));
        }

        public SearchDefinition<TDocument> Text(string query, IEnumerable<string> path)
        {
            return Text(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
        }

        public SearchDefinition<TDocument> Text<TField>(IEnumerable<string> query, Expression<Func<TDocument, TField>> field)
        {
            return Text(query, new ExpressionFieldDefinition<TDocument, TField>(field));
        }

        public SearchDefinition<TDocument> Text(IEnumerable<string> query, IEnumerable<string> path)
        {
            return Text(query, path.Select(field => new StringFieldDefinition<TDocument>(field)));
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

            BsonDocument doc = new BsonDocument();
            doc.Add("query", queryVal);
            doc.Add("path", pathVal);
            return new BsonDocument("text", doc);
        }
    }
}
