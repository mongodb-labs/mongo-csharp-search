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
    public class ScoreFunctionBuilderTests
    {
        [Fact]
        public void Path()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Path("x"),
                "{ path: 'x' }");
            AssertRendered(
                subject.Path("x", 1),
                "{ path: { value: 'x', undefined: 1 } }");
        }

        [Fact]
        public void Path_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Path(x => x.Age),
                "{ path: 'age' }");
            AssertRendered(
                subject.Path("Age"),
                "{ path: 'age' }");
        }

        [Fact]
        public void Relevance()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Relevance(),
                "{ score: 'relevance' }");
        }

        [Fact]
        public void Constant()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Constant(1),
                "{ constant: 1 }");
        }

        [Fact]
        public void Add()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Add(
                    subject.Constant(1),
                    subject.Constant(2)),
                "{ add: [{ constant: 1 }, { constant: 2 }] }");
        }

        [Fact]
        public void Multiply()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Multiply(
                    subject.Constant(1),
                    subject.Constant(2)),
                "{ multiply: [{ constant: 1 }, { constant: 2 }] }");
        }

        [Fact]
        public void Gauss()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Gauss("x", 100, 1),
                "{ gauss: { path: 'x', origin: 100, scale: 1 } }");
            AssertRendered(
                subject.Gauss("x", 100, 1, 0.1, 1),
                "{ gauss: { path: 'x', origin: 100, scale: 1, decay: 0.1, offset: 1 } }");
        }

        [Fact]
        public void Gauss_Typed()
        {
            var subject = CreateSubject<Person>();

            AssertRendered(
                subject.Gauss(x => x.Age, 100, 1),
                "{ gauss: { path: 'age', origin: 100, scale: 1 } }");
            AssertRendered(
                subject.Gauss("Age", 100, 1),
                "{ gauss: { path: 'age', origin: 100, scale: 1 } }");
        }

        [Fact]
        public void Log()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Log(
                    subject.Constant(1)),
                "{ log: { constant: 1 } }");
        }

        [Fact]
        public void Log1p()
        {
            var subject = CreateSubject<BsonDocument>();

            AssertRendered(
                subject.Log1p(
                    subject.Constant(1)),
                "{ log1p: { constant: 1 } }");
        }

        private void AssertRendered<TDocument>(ScoreFunction<TDocument> function, string expected)
        {
            AssertRendered(function, BsonDocument.Parse(expected));
        }

        private void AssertRendered<TDocument>(ScoreFunction<TDocument> function, BsonDocument expected)
        {
            var documentSerializer = BsonSerializer.SerializerRegistry.GetSerializer<TDocument>();
            var renderedFunction = function.Render(documentSerializer, BsonSerializer.SerializerRegistry);

            renderedFunction.Should().Equal(expected);
        }

        private ScoreFunctionBuilder<TDocument> CreateSubject<TDocument>()
        {
            return new ScoreFunctionBuilder<TDocument>();
        }

        private class Person
        {
            [BsonElement("age")]
            public int Age { get; set; }
        }
    }
}
