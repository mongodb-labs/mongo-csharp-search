using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Driver.Search.Tests
{
    public class SearchDefinitionBuildTests
    {
        [Fact]
        public void Phrase()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Phrase("foo", "x"),
                "{ phrase: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Phrase("foo", new[] { "x", "y" }),
                "{ phrase: { query: \"foo\", path: [\"x\", \"y\"] } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, "x"),
                "{ phrase: { query: [\"foo\", \"bar\"], path: \"x\" } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ phrase: { query: [\"foo\", \"bar\"], path: [\"x\", \"y\"] } }");
        }

        [Fact]
        public void Phrase_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Phrase("foo", x => x.FirstName),
                "{ phrase: { query: \"foo\", path: \"fn\" } }");
            AssertRendered(
                subject.Phrase("foo", "FirstName"),
                "{ phrase: { query: \"foo\", path: \"fn\" } }");

            AssertRendered(
                subject.Phrase(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ phrase: { query: \"foo\", path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Phrase("foo", new[] { "FirstName", "LastName" }),
                "{ phrase: { query: \"foo\", path: [\"fn\", \"ln\"] } }");

            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, x => x.FirstName),
                "{ phrase: { query: [\"foo\", \"bar\"], path: \"fn\" } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, "FirstName"),
                "{ phrase: { query: [\"foo\", \"bar\"], path: \"fn\" } }");

            AssertRendered(
                subject.Phrase(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ phrase: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ phrase: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
        }

        [Fact]
        public void Text()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Text("foo", "x"),
                "{ text: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Text("foo", new[] { "x", "y" }),
                "{ text: { query: \"foo\", path: [\"x\", \"y\"] } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, "x"),
                "{ text: { query: [\"foo\", \"bar\"], path: \"x\" } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ text: { query: [\"foo\", \"bar\"], path: [\"x\", \"y\"] } }");
        }

        [Fact]
        public void Text_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Text("foo", x => x.FirstName),
                "{ text: { query: \"foo\", path: \"fn\" } }");
            AssertRendered(
                subject.Text("foo", "FirstName"),
                "{ text: { query: \"foo\", path: \"fn\" } }");

            AssertRendered(
                subject.Text(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ text: { query: \"foo\", path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Text("foo", new[] { "FirstName", "LastName" }),
                "{ text: { query: \"foo\", path: [\"fn\", \"ln\"] } }");

            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, x => x.FirstName),
                "{ text: { query: [\"foo\", \"bar\"], path: \"fn\" } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, "FirstName"),
                "{ text: { query: [\"foo\", \"bar\"], path: \"fn\" } }");

            AssertRendered(
                subject.Text(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ text: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ text: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
        }

        private void AssertRendered<TDocument>(SearchDefinition<TDocument> query, string expected)
        {
            AssertRendered(query, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(SearchDefinition<TDocument> query, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedQuery = query.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedQuery);
        }

        private SearchDefinitionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new SearchDefinitionBuilder<TDocument>();
        }

        private class Person
        {
            [BsonElement("fn")]
            public string FirstName { get; set; }

            [BsonElement("ln")]
            public string LastName { get; set; }
        }
    }
}
