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
    public class PathDefinitionBuilderTests
    {
        [Fact]
        public void Analyzer()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Analyzer("x", "english"),
                "{ value: 'x', multi: 'english' }");
        }

        [Fact]
        public void Analyzer_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Analyzer(x => x.FirstName, "english"),
                "{ value: 'fn', multi: 'english' }");
            AssertRendered(
                subject.Analyzer("FirstName", "english"),
                "{ value: 'fn', multi: 'english' }");
        }

        private void AssertRendered<TDocument>(PathDefinition<TDocument> path, string expected)
        {
            AssertRendered(path, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(PathDefinition<TDocument> path, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedPath = path.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            Assert.Equal(expected, renderedPath);
        }

        private PathDefinitionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new PathDefinitionBuilder<TDocument>();
        }
        
        private class Person
        {
            [BsonElement("fn")]
            public string FirstName { get; set; }
        }
    }
}
