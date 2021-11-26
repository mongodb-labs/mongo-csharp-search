﻿// Copyright 2021-present MongoDB Inc.
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

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Labs.Search.Tests
{
    public class SpanDefinitionBuilderTests
    {
        [Fact]
        public void First()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.First(subject.Term("foo", "x"), 5),
                "{ first: { operator: { term: { query: 'foo', path: 'x' } }, endPositionLte: 5 } }");
        }

        [Fact]
        public void First_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.First(subject.Term("born", x => x.Biography), 5),
                "{ first: { operator: { term: { query: 'born', path: 'bio' } }, endPositionLte: 5 } }");
            AssertRendered(
                subject.First(subject.Term("born", "Biography"), 5),
                "{ first: { operator: { term: { query: 'born', path: 'bio' } }, endPositionLte: 5 } }");
        }

        private void AssertRendered<TDocument>(SpanDefinition<TDocument> span, string expected)
        {
            AssertRendered(span, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(SpanDefinition<TDocument> span, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedSpan = span.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedSpan);
        }

        private SpanDefinitionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new SpanDefinitionBuilder<TDocument>();
        }

        private class Person
        {
            [BsonElement("bio")]
            public string Biography { get; set; }
        }
    }
}
