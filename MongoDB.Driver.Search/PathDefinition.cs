using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Labs.Search
{
    public abstract class PathDefinition<TDocument>
    {
        public abstract BsonValue Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry);

        public static implicit operator PathDefinition<TDocument>(FieldDefinition<TDocument> field)
        {
            return new SinglePathDefinition<TDocument>(field);
        }

        public static implicit operator PathDefinition<TDocument>(string fieldName)
        {
            return new SinglePathDefinition<TDocument>(new StringFieldDefinition<TDocument>(fieldName));
        }

        public static implicit operator PathDefinition<TDocument>(FieldDefinition<TDocument>[] fields)
        {
            return new MultiPathDefinition<TDocument>(fields);
        }

        public static implicit operator PathDefinition<TDocument>(List<FieldDefinition<TDocument>> fields)
        {
            return new MultiPathDefinition<TDocument>(fields);
        }

        public static implicit operator PathDefinition<TDocument>(string[] fieldNames)
        {
            return new MultiPathDefinition<TDocument>(
                fieldNames.Select(fieldName => new StringFieldDefinition<TDocument>(fieldName)));
        }
    }

    public sealed class SinglePathDefinition<TDocument> : PathDefinition<TDocument>
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

    public sealed class MultiPathDefinition<TDocument> : PathDefinition<TDocument>
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
}
