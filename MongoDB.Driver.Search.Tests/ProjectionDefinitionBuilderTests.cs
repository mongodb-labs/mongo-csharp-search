using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Xunit;

namespace MongoDB.Labs.Search.Tests
{
    public class ProjectionDefinitionBuilderTests
    {
        [Fact]
        public void MetaSearchScore()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(subject.MetaSearchScore("a"), "{ a : { $meta: 'searchScore' } }");
        }

        private void AssertRendered<TDocument>(ProjectionDefinition<TDocument> projection, string expected)
        {
            AssertRendered(projection, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(ProjectionDefinition<TDocument> projection, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedProjection = projection.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedProjection);
        }

        private ProjectionDefinitionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new ProjectionDefinitionBuilder<TDocument>();
        }
    }
}
