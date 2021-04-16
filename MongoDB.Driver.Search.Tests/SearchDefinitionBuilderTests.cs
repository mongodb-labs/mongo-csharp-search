using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
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
                "{ autocomplete: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Autocomplete("foo", new[] { "x", "y" }),
                "{ autocomplete: { query: 'foo', path: ['x', 'y'] } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, "x"),
                "{ autocomplete: { query: ['foo', 'bar'], path: 'x' } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ autocomplete: { query: ['foo', 'bar'], path: ['x', 'y'] } }");

            AssertRendered(
                subject.Autocomplete("foo", "x", AutocompleteTokenOrder.Any),
                "{ autocomplete: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Autocomplete("foo", "x", AutocompleteTokenOrder.Sequential),
                "{ autocomplete: { query: 'foo', path: 'x', tokenOrder: 'sequential' } }");

            AssertRendered(
                subject.Autocomplete("foo", "x", fuzzy: new FuzzyOptions()),
                "{ autocomplete: { query: 'foo', path: 'x', fuzzy: {} } }");
            AssertRendered(
                subject.Autocomplete("foo", "x", fuzzy: new FuzzyOptions()
                {
                    MaxEdits = 1,
                    PrefixLength = 5,
                    MaxExpansions = 25
                }),
                "{ autocomplete: { query: 'foo', path: 'x', fuzzy: { maxEdits: 1, prefixLength: 5, maxExpansions: 25 } } }");
        }

        [Fact]
        public void Autocomplete_Typed()
        {
            var subject = CreateSubject<Person>();
            AssertRendered(
                subject.Autocomplete("foo", x => x.FirstName),
                "{ autocomplete: { query: 'foo', path: 'fn' } }");
            AssertRendered(
                subject.Autocomplete("foo", "FirstName"),
                "{ autocomplete: { query: 'foo', path: 'fn' } }");

            AssertRendered(
                subject.Autocomplete(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ autocomplete: { query: 'foo', path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Autocomplete("foo", new[] { "FirstName", "LastName" }),
                "{ autocomplete: { query: 'foo', path: ['fn', 'ln'] } }");

            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, x => x.FirstName),
                "{ autocomplete: { query: ['foo', 'bar'], path: 'fn' } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, "FirstName"),
                "{ autocomplete: { query: ['foo', 'bar'], path: 'fn' } }");

            AssertRendered(
                subject.Autocomplete(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ autocomplete: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Autocomplete(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ autocomplete: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
        }

        [Fact]
        public void Eq()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Eq("x", true),
                "{ equals: { path: 'x', value: true } }");
            AssertRendered(
                subject.Eq("x", ObjectId.Empty),
                "{ equals: { path: 'x', value: { $oid: '000000000000000000000000' } } }");
        }

        [Fact]
        public void Eq_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Eq(x => x.Retired, true),
                "{ equals: { path: 'ret', value: true } }");
            AssertRendered(
                subject.Eq("Retired", true),
                "{ equals: { path: 'ret', value: true } }");

            AssertRendered(
                subject.Eq(x => x.Id, ObjectId.Empty),
                "{ equals: { path: '_id', value: { $oid: '000000000000000000000000' } } }");
            AssertRendered(
                subject.Eq(x => x.Id, ObjectId.Empty),
                "{ equals: { path: '_id', value: { $oid: '000000000000000000000000' } } }");
        }

        [Fact]
        public void Exists()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Exists("x"),
                "{ exists: { path: 'x' } }");
        }

        [Fact]
        public void Exists_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Exists(x => x.FirstName),
                "{ exists: { path: 'fn' } }");
            AssertRendered(
                subject.Exists("FirstName"),
                "{ exists: { path: 'fn' } }");
        }

        [Fact]
        public void Filter()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Filter(
                    subject.Exists("x"),
                    subject.Exists("y")),
                "{ compound: { filter: [{ exists: { path: 'x' } }, { exists: { path: 'y' } }] } }");
        }

        [Fact]
        public void Must()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Must(
                    subject.Exists("x"),
                    subject.Exists("y")),
                "{ compound: { must: [{ exists: { path: 'x' } }, { exists: { path: 'y' } }] } }");
        }

        [Fact]
        public void MustNot()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.MustNot(
                    subject.Exists("x"),
                    subject.Exists("y")),
                "{ compound: { mustNot: [{ exists: { path: 'x' } }, { exists: { path: 'y' } }] } }");
        }

        [Fact]
        public void Near()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Near("x", 5.0, 1.0),
                "{ near: { path: 'x', origin: 5.0, pivot: 1.0 } }");
            AssertRendered(
                subject.Near("x", 5, 1),
                "{ near: { path: 'x', origin: 5, pivot: 1 } }");
            AssertRendered(
                subject.Near("x", 5L, 1L),
                "{ near: { path: 'x', origin: { $numberLong: '5' }, pivot: { $numberLong: '1' } } }");
            AssertRendered(
                subject.Near("x", new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1000L),
                "{ near: { path: 'x', origin: { $date: '2000-01-01T00:00:00Z' }, pivot: { $numberLong: '1000' } } }");
        }

        [Fact]
        public void Near_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Near(x => x.Age, 35.0, 5.0),
                "{ near: { path: 'age', origin: 35.0, pivot: 5.0 } }");
            AssertRendered(
                subject.Near("Age", 35.0, 5.0),
                "{ near: { path: 'age', origin: 35.0, pivot: 5.0 } }");

            AssertRendered(
                subject.Near(x => x.Age, 35, 5),
                "{ near: { path: 'age', origin: 35, pivot: 5 } }");
            AssertRendered(
                subject.Near("Age", 35, 5),
                "{ near: { path: 'age', origin: 35, pivot: 5 } }");

            AssertRendered(
                subject.Near(x => x.Age, 35L, 5L),
                "{ near: { path: 'age', origin: { $numberLong: '35' }, pivot: { $numberLong: '5' } } }");
            AssertRendered(
                subject.Near("Age", 35L, 5L),
                "{ near: { path: 'age', origin: { $numberLong: '35' }, pivot: { $numberLong: '5' } } }");

            AssertRendered(
                subject.Near(x => x.Birthday, new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1000L),
                "{ near: { path: 'dob', origin: { $date: '2000-01-01T00:00:00Z' }, pivot: { $numberLong: '1000' } } }");
            AssertRendered(
                subject.Near("Birthday", new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1000L),
                "{ near: { path: 'dob', origin: { $date: '2000-01-01T00:00:00Z' }, pivot: { $numberLong: '1000' } } }");
        }

        [Fact]
        public void Phrase()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Phrase("foo", "x"),
                "{ phrase: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Phrase("foo", new[] { "x", "y" }),
                "{ phrase: { query: 'foo', path: ['x', 'y'] } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, "x"),
                "{ phrase: { query: ['foo', 'bar'], path: 'x' } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ phrase: { query: ['foo', 'bar'], path: ['x', 'y'] } }");
        }

        [Fact]
        public void Phrase_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Phrase("foo", x => x.FirstName),
                "{ phrase: { query: 'foo', path: 'fn' } }");
            AssertRendered(
                subject.Phrase("foo", "FirstName"),
                "{ phrase: { query: 'foo', path: 'fn' } }");

            AssertRendered(
                subject.Phrase(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ phrase: { query: 'foo', path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Phrase("foo", new[] { "FirstName", "LastName" }),
                "{ phrase: { query: 'foo', path: ['fn', 'ln'] } }");

            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, x => x.FirstName),
                "{ phrase: { query: ['foo', 'bar'], path: 'fn' } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, "FirstName"),
                "{ phrase: { query: ['foo', 'bar'], path: 'fn' } }");

            AssertRendered(
                subject.Phrase(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ phrase: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Phrase(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ phrase: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
        }

        [Fact]
        public void QueryString()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.QueryString("x", "foo"),
                "{ queryString: { defaultPath: 'x', query: 'foo' } }");
        }

        [Fact]
        public void QueryString_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.QueryString(x => x.FirstName, "foo"),
                "{ queryString: { defaultPath: 'fn', query: 'foo' } }");
            AssertRendered(
                subject.QueryString("FirstName", "foo"),
                "{ queryString: { defaultPath: 'fn', query: 'foo' } }");
        }

        [Fact]
        public void Regex()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Regex("foo", "x"),
                "{ regex: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Regex("foo", new[] { "x", "y" }),
                "{ regex: { query: 'foo', path: ['x', 'y'] } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, "x"),
                "{ regex: { query: ['foo', 'bar'], path: 'x' } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ regex: { query: ['foo', 'bar'], path: ['x', 'y'] } }");

            AssertRendered(
                subject.Regex("foo", "x", false),
                "{ regex: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Regex("foo", "x", true),
                "{ regex: { query: 'foo', path: 'x', allowAnalyzedField: true } }");
        }

        [Fact]
        public void Regex_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Regex("foo", x => x.FirstName),
                "{ regex: { query: 'foo', path: 'fn' } }");
            AssertRendered(
                subject.Regex("foo", "FirstName"),
                "{ regex: { query: 'foo', path: 'fn' } }");

            AssertRendered(
                subject.Regex(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ regex: { query: 'foo', path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Regex("foo", new[] { "FirstName", "LastName" }),
                "{ regex: { query: 'foo', path: ['fn', 'ln'] } }");

            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, x => x.FirstName),
                "{ regex: { query: ['foo', 'bar'], path: 'fn' } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, "FirstName"),
                "{ regex: { query: ['foo', 'bar'], path: 'fn' } }");

            AssertRendered(
                subject.Regex(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ regex: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Regex(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ regex: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
        }

        [Fact]
        public void Should()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Should(
                    subject.Exists("x"),
                    subject.Exists("y")),
                "{ compound: { should: [{ exists: { path: 'x' } }, { exists: { path: 'y' } }] } }");
        }

        [Fact]
        public void Text()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Text("foo", "x"),
                "{ text: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Text("foo", new[] { "x", "y" }),
                "{ text: { query: 'foo', path: ['x', 'y'] } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, "x"),
                "{ text: { query: ['foo', 'bar'], path: 'x' } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ text: { query: ['foo', 'bar'], path: ['x', 'y'] } }");

            AssertRendered(
                subject.Text("foo", "x", new FuzzyOptions()),
                "{ text: { query: 'foo', path: 'x', fuzzy: {} } }");
            AssertRendered(
                subject.Text("foo", "x", new FuzzyOptions()
                {
                    MaxEdits = 1,
                    PrefixLength = 5,
                    MaxExpansions = 25
                }),
                "{ text: { query: 'foo', path: 'x', fuzzy: { maxEdits: 1, prefixLength: 5, maxExpansions: 25 } } }");
        }

        [Fact]
        public void Text_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Text("foo", x => x.FirstName),
                "{ text: { query: 'foo', path: 'fn' } }");
            AssertRendered(
                subject.Text("foo", "FirstName"),
                "{ text: { query: 'foo', path: 'fn' } }");

            AssertRendered(
                subject.Text(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ text: { query: 'foo', path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Text("foo", new[] { "FirstName", "LastName" }),
                "{ text: { query: 'foo', path: ['fn', 'ln'] } }");

            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, x => x.FirstName),
                "{ text: { query: ['foo', 'bar'], path: 'fn' } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, "FirstName"),
                "{ text: { query: ['foo', 'bar'], path: 'fn' } }");

            AssertRendered(
                subject.Text(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ text: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Text(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ text: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
        }

        [Fact]
        public void Wildcard()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Wildcard("foo", "x"),
                "{ wildcard: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Wildcard("foo", new[] { "x", "y" }),
                "{ wildcard: { query: 'foo', path: ['x', 'y'] } }");
            AssertRendered(
                subject.Wildcard(new[] { "foo", "bar" }, "x"),
                "{ wildcard: { query: ['foo', 'bar'], path: 'x' } }");
            AssertRendered(
                subject.Wildcard(new[] { "foo", "bar" }, new[] { "x", "y" }),
                "{ wildcard: { query: ['foo', 'bar'], path: ['x', 'y'] } }");

            AssertRendered(
                subject.Wildcard("foo", "x", false),
                "{ wildcard: { query: 'foo', path: 'x' } }");
            AssertRendered(
                subject.Wildcard("foo", "x", true),
                "{ wildcard: { query: 'foo', path: 'x', allowAnalyzedField: true } }");
        }

        [Fact]
        public void Wildcard_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Wildcard("foo", x => x.FirstName),
                "{ wildcard: { query: 'foo', path: 'fn' } }");
            AssertRendered(
                subject.Wildcard("foo", "FirstName"),
                "{ wildcard: { query: 'foo', path: 'fn' } }");

            AssertRendered(
                subject.Wildcard(
                    "foo",
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ wildcard: { query: 'foo', path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Wildcard("foo", new[] { "FirstName", "LastName" }),
                "{ wildcard: { query: 'foo', path: ['fn', 'ln'] } }");

            AssertRendered(
                subject.Wildcard(new[] { "foo", "bar" }, x => x.FirstName),
                "{ wildcard: { query: ['foo', 'bar'], path: 'fn' } }");
            AssertRendered(
                subject.Wildcard(new[] { "foo", "bar" }, "FirstName"),
                "{ wildcard: { query: ['foo', 'bar'], path: 'fn' } }");

            AssertRendered(
                subject.Wildcard(
                    new[] { "foo", "bar" },
                    new FieldDefinition<Person>[]
                    {
                        new ExpressionFieldDefinition<Person, string>(x => x.FirstName),
                        new ExpressionFieldDefinition<Person, string>(x => x.LastName)
                    }),
                "{ wildcard: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
            AssertRendered(
                subject.Wildcard(new[] { "foo", "bar" }, new[] { "FirstName", "LastName" }),
                "{ wildcard: { query: ['foo', 'bar'], path: ['fn', 'ln'] } }");
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

            [BsonElement("age")]
            public int Age { get; set; }

            [BsonElement("ret")]
            public bool Retired { get; set; }

            [BsonElement("dob")]
            public DateTime Birthday { get; set; }
        }
    }
}
