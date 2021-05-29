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

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Xunit;

namespace MongoDB.Labs.Search.Tests
{
    public class HighlightOptionsBuilderTests
    {
        [Fact]
        public void Options()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Options("bio"),
                "{ path: 'bio' }");
            AssertRendered(
                subject.Options("bio", maxCharsToExamine: 1000),
                "{ path: 'bio', maxCharsToExamine: 1000 }");
            AssertRendered(
                subject.Options("bio", maxNumPassages: 5),
                "{ path: 'bio', maxNumPassages: 5 }");
        }

        [Fact]
        public void Options_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Options(x => x.Biography),
                "{ path: 'bio' }");
            AssertRendered(
                subject.Options("Biography"),
                "{ path: 'bio' }");
        }

        private void AssertRendered<TDocument>(HighlightOptions<TDocument> highlight, string expected)
        {
            AssertRendered(highlight, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(HighlightOptions<TDocument> highlight, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedHighlight = highlight.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedHighlight);
        }

        private HighlightOptionsBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new HighlightOptionsBuilder<TDocument>();
        }

        private class Person
        {
            [BsonElement("bio")]
            public string Biography { get; set; }
        }
    }
}
