using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Driver.Search.Tests
{
    public class SearchDefinitionBuildTests
    {
        [Fact]
        public void Autocomplete()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Autocomplete("foo", "x"),
                "{ autocomplete: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Autocomplete("foo", new[] { "x", "y" }),
                "{ autocomplete: { query: \"foo\", path: [\"x\", \"y\"] } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, "x"),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: \"x\" } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: [\"x\", \"y\"] } }");

            AssertRendered(
                subject.Autocomplete("foo", "x", AutocompleteTokenOrder.Any),
                "{ autocomplete: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Autocomplete("foo", "x", AutocompleteTokenOrder.Sequential),
                "{ autocomplete: { query: \"foo\", path: \"x\", tokenOrder: \"sequential\" } }");
        }

        [Fact]
        public void Autocomplete_Typed()
        {
            var subject = CreateSubject<Person>();
            AssertRendered(
                subject.Autocomplete("foo", x => x.FirstName),
                "{ autocomplete: { query: \"foo\", path: \"fn\" } }");
            AssertRendered(
                subject.Autocomplete("foo", "FirstName"),
                "{ autocomplete: { query: \"foo\", path: \"fn\" } }");

            AssertRendered(
                subject.Autocomplete(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ autocomplete: { query: \"foo\", path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Autocomplete("foo", new[] { "FirstName", "LastName" }),
                "{ autocomplete: { query: \"foo\", path: [\"fn\", \"ln\"] } }");

            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, x => x.FirstName),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: \"fn\" } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, "FirstName"),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: \"fn\" } }");

            AssertRendered(
                subject.Autocomplete(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ autocomplete: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
        }

        [Fact]
        public void Eq()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Eq("x", true),
                "{ equals: { path: \"x\", value: true } }");
            AssertRendered(
                subject.Eq("x", ObjectId.Empty),
                "{ equals: { path: \"x\", value: { $oid: \"000000000000000000000000\" } } }");
        }

        [Fact]
        public void Eq_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Eq(x => x.Retired, true),
                "{ equals: { path: \"ret\", value: true } }");
            AssertRendered(
                subject.Eq("Retired", true),
                "{ equals: { path: \"ret\", value: true } }");

            AssertRendered(
                subject.Eq(x => x.Id, ObjectId.Empty),
                "{ equals: { path: \"_id\", value: { $oid: \"000000000000000000000000\" } } }");
            AssertRendered(
                subject.Eq(x => x.Id, ObjectId.Empty),
                "{ equals: { path: \"_id\", value: { $oid: \"000000000000000000000000\" } } }");
        }

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
        public void QueryString()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.QueryString("x", "foo"),
                "{ queryString: { defaultPath: \"x\", query: \"foo\" } }");
        }

        [Fact]
        public void QueryString_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.QueryString(x => x.FirstName, "foo"),
                "{ queryString: { defaultPath: \"fn\", query: \"foo\" } }");
            AssertRendered(
                subject.QueryString("FirstName", "foo"),
                "{ queryString: { defaultPath: \"fn\", query: \"foo\" } }");
        }

        [Fact]
        public void Regex()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Regex("foo", "x"),
                "{ regex: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Regex("foo", new[] { "x", "y" }),
                "{ regex: { query: \"foo\", path: [\"x\", \"y\"] } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, "x"),
                "{ regex: { query: [\"foo\", \"bar\"], path: \"x\" } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ regex: { query: [\"foo\", \"bar\"], path: [\"x\", \"y\"] } }");

            AssertRendered(
                subject.Regex("foo", "x", false),
                "{ regex: { query: \"foo\", path: \"x\" } }");
            AssertRendered(
                subject.Regex("foo", "x", true),
                "{ regex: { query: \"foo\", path: \"x\", allowAnalyzedField: true } }");
        }

        [Fact]
        public void Regex_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Regex("foo", x => x.FirstName),
                "{ regex: { query: \"foo\", path: \"fn\" } }");
            AssertRendered(
                subject.Regex("foo", "FirstName"),
                "{ regex: { query: \"foo\", path: \"fn\" } }");

            AssertRendered(
                subject.Regex(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ regex: { query: \"foo\", path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Regex("foo", new[] { "FirstName", "LastName" }),
                "{ regex: { query: \"foo\", path: [\"fn\", \"ln\"] } }");

            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, x => x.FirstName),
                "{ regex: { query: [\"foo\", \"bar\"], path: \"fn\" } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, "FirstName"),
                "{ regex: { query: [\"foo\", \"bar\"], path: \"fn\" } }");

            AssertRendered(
                subject.Regex(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ regex: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ regex: { query: [\"foo\", \"bar\"], path: [\"fn\", \"ln\"] } }");
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
            [BsonId]
            public ObjectId Id { get; set; }

            [BsonElement("fn")]
            public string FirstName { get; set; }

            [BsonElement("ln")]
            public string LastName { get; set; }

            [BsonElement("ret")]
            public bool Retired { get; set; }
        }
    }
}
