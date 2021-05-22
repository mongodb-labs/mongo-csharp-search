using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Labs.Search.Tests
{
    public class ScoreDefinitionBuilderTests
    {
        [Fact]
        public void Boost()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Boost(1),
                "{ boost: { value: 1 } }");
            AssertRendered(
                subject.Boost("x"),
                "{ boost: { path: 'x' } }");
            AssertRendered(
                subject.Boost("x", 1),
                "{ boost: { path: 'x', undefined: 1 } }");
        }

        [Fact]
        public void Boost_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Boost(x => x.Age),
                "{ boost: { path: 'age' } }");
            AssertRendered(
                subject.Boost("Age"),
                "{ boost: { path: 'age' } }");
        }

        [Fact]
        public void Constant()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Constant(1),
                "{ constant: { value: 1 } }");
        }

        private void AssertRendered<TDocument>(ScoreDefinition<TDocument> score, string expected)
        {
            AssertRendered(score, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(ScoreDefinition<TDocument> score, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedQuery = score.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedQuery);
        }

        private ScoreDefinitionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new ScoreDefinitionBuilder<TDocument>();
        }

        private class Person
        {
            [BsonElement("age")]
            public int Age { get; set; }
        }
    }
}
