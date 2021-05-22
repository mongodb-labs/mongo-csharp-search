using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDB.Labs.Search
{
    public abstract class ScoreDefinition<TDocument>
    {
        public abstract BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry);
    }
}
