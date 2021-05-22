using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    public abstract class SearchDefinition<TDocument>
    {
        public abstract BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry);

        public static implicit operator SearchDefinition<TDocument>(BsonDocument document)
        {
            if (document == null)
            {
                return null;
            }

            return new BsonDocumentSearchDefinition<TDocument>(document);
        }

        public static implicit operator SearchDefinition<TDocument>(string json)
        {
            if (json == null)
            {
                return null;
            }

            return new JsonSearchDefinition<TDocument>(json);
        }
    }

    public sealed class BsonDocumentSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly BsonDocument _document;

        public BsonDocumentSearchDefinition(BsonDocument document)
        {
            _document = Ensure.IsNotNull(document, nameof(document));
        }

        public BsonDocument Document
        {
            get { return _document; }
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return _document;
        }
    }

    public sealed class JsonSearchDefinition<TDocument> : SearchDefinition<TDocument>
    {
        private readonly string _json;

        public JsonSearchDefinition(string json)
        {
            _json = Ensure.IsNotNullOrEmpty(json, nameof(json));
        }

        public string Json
        {
            get { return _json; }
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return BsonDocument.Parse(_json);
        }
    }
}
