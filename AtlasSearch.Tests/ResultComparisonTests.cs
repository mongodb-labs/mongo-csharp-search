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

using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Labs.Search;
using MongoDB.Labs.Search.ObjectModel;
using Xunit;

namespace AtlasSearch.Tests
{
    public class ResultComparisonTests
    {
        [Fact]
        public void TestPhrase()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                    SearchBuilders<HistoricalDocument>.Highlight
                        .Options(x => x.Body),
                    "default")
                .Limit(1)
                .Project<HistoricalDocument>(
                    Builders<HistoricalDocument>.Projection
                        .Include(x => x.Title)
                        .Include(x => x.Body)
                        .MetaSearchScore("score")
                        .MetaSearchHighlights("highlights"))
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
            Assert.NotEqual(0, results[0].Score);
            Assert.Single(results[0].Highlights);
            var highlightTexts = results[0].Highlights[0].Texts;
            Assert.Equal(15, highlightTexts.Count);
            Assert.Equal(HighlightTextType.Text, highlightTexts[0].Type);
            var highlightRange = highlightTexts.GetRange(1, 13);
            foreach (var highlight in highlightRange)
            {
                var expected = char.IsLetter(highlight.Value[0]) ?
                    HighlightTextType.Hit : HighlightTextType.Text;
                Assert.Equal(expected, highlight.Type);
            }
            var highlightRangeStr = string.Join(
                string.Empty, highlightRange.Select(x => x.Value));
            Assert.Equal("Life, Liberty and the pursuit of Happiness", highlightRangeStr);
            Assert.Equal(HighlightTextType.Text, highlightTexts[14].Type);
            Assert.Equal(".", highlightTexts[14].Value);
        }

        [Fact]
        public void TestAutocomplete()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Autocomplete("Declaration of Ind", x => x.Title))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestExists()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("life, liberty, and the pursuit of happiness", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Exists(x => x.Title)))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestFilter()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Filter(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true)))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestMust()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true)))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestMustNot()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .MustNot(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body)))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.NotEqual("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestQueryString()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .QueryString(x => x.Body, "life, liberty, and the pursuit of happiness"))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestShould()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Should(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true)))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestText()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Text("life, liberty, and the pursuit of happiness", x => x.Body))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        [Fact]
        public void TestWildcard()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Wildcard("tranquil*", x => x.Body, true))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("US Constitution", results[0].Title);
        }

        [Fact]
        public void TestAnalyzer()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase(
                            "life, liberty, and the pursuit of happiness",
                            SearchBuilders<HistoricalDocument>.Path
                                .Analyzer(x => x.Body, "english")))
                .Limit(1)
                .ToList();
            Assert.Single(results);
            Assert.Equal("Declaration of Independence", results[0].Title);
        }

        private static IMongoCollection<HistoricalDocument> GetTestCollection()
        {
            var uri = Environment.GetEnvironmentVariable("ATLAS_SEARCH_URI");
            var client = new MongoClient(uri);
            var db = client.GetDatabase("sample_training");
            return db.GetCollection<HistoricalDocument>("posts");
        }

        [BsonIgnoreExtraElements]
        private class HistoricalDocument
        {
            [BsonId]
            public ObjectId Id { get; set; }

            [BsonElement("body")]
            public string Body { get; set; }

            [BsonElement("title")]
            public string Title { get; set; }

            [BsonElement("highlights")]
            public List<Highlight> Highlights { get; set; }

            [BsonElement("score")]
            public double Score { get; set; }
        }
    }
}
