// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Labs.Search.Tests
{
    public class SearchFacetBuilderTests
    {
        [Fact]
        public void String()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.String("string", "x", 100),
                "{ type: 'string', path: 'x', numBuckets: 100 }");
        }

        [Fact]
        public void String_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.String("string", x => x.FirstName, 100),
                "{ type: 'string', path: 'fn', numBuckets: 100 }");
            AssertRendered(
                subject.String("string", "FirstName", 100),
                "{ type: 'string', path: 'fn', numBuckets: 100 }");
        }

        private void AssertRendered<TDocument>(SearchFacet<TDocument> facet, string expected)
        {
            AssertRendered(facet, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(SearchFacet<TDocument> facet, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedFacet = facet.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            renderedFacet.Should().Equal(expected);
        }

        private SearchFacetBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new SearchFacetBuilder<TDocument>();
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
