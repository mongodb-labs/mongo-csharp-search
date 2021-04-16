using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver.Search
{
    public sealed class ScoreDefinitionBuilder<TDocument>
    {
        public ScoreDefinition<TDocument> Constant(double value)
        {
            return new ConstantScoreDefinition<TDocument>(value);
        }
    }

    internal sealed class ConstantScoreDefinition<TDocument> : ScoreDefinition<TDocument>
    {
        private readonly double _value;

        public ConstantScoreDefinition(double value)
        {
            _value = EnsureExtensions.IsGreaterThanZero(value, nameof(value));
        }

        public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            var document = new BsonDocument("value", _value);
            return new BsonDocument("constant", document);
        }
    }
}
