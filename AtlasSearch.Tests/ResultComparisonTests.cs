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
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Labs.Search;
using MongoDB.Labs.Search.ObjectModel;
using Xunit;

namespace AtlasSearch.Tests
{
    public class ResultComparisonTests
    {
        private static readonly GeoJsonPolygon<GeoJson2DGeographicCoordinates> __testPolygon =
            new GeoJsonPolygon<GeoJson2DGeographicCoordinates>(
                new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(
                    new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                        new List<GeoJson2DGeographicCoordinates>()
                        {
                            new GeoJson2DGeographicCoordinates(-161.323242, 22.512557),
                            new GeoJson2DGeographicCoordinates(-152.446289, 22.065278),
                            new GeoJson2DGeographicCoordinates(-156.09375, 17.811456),
                            new GeoJson2DGeographicCoordinates(-161.323242, 22.512557)
                        })));
        private static readonly GeoWithinBox<GeoJson2DGeographicCoordinates> __testBox =
            new GeoWithinBox<GeoJson2DGeographicCoordinates>(
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(-161.323242, 22.065278)),
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(-152.446289, 22.512557)));
        private static readonly GeoWithinCircle<GeoJson2DGeographicCoordinates> __testCircle =
            new GeoWithinCircle<GeoJson2DGeographicCoordinates>(
                new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(-161.323242, 22.512557)),
                7.5);

        [Fact]
        public void TestPhrase()
        {
            // This test case exercises the indexName and returnStoredSource arguments. The
            // remaining test cases omit them.
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                    SearchBuilders<HistoricalDocument>.Highlight
                        .Options(x => x.Body),
                    indexName: "default",
                    returnStoredSource: true)
                .Limit(1)
                .Project<HistoricalDocument>(
                    Builders<HistoricalDocument>.Projection
                        .Include(x => x.Title)
                        .Include(x => x.Body)
                        .MetaSearchScore("score")
                        .MetaSearchHighlights("highlights"))
                .ToList();
            results.Should().ContainSingle();
            results[0].Title.Should().Be("Declaration of Independence");
            results[0].Score.Should().NotBe(0);
            results[0].Highlights.Should().ContainSingle();
            var highlightTexts = results[0].Highlights[0].Texts;
            highlightTexts.Should().HaveCount(15);
            highlightTexts[0].Type.Should().Be(HighlightTextType.Text);
            var highlightRange = highlightTexts.GetRange(1, 13);
            foreach (var highlight in highlightRange)
            {
                var expected = char.IsLetter(highlight.Value[0]) ?
                    HighlightTextType.Hit : HighlightTextType.Text;
                highlight.Type.Should().Be(expected);
            }
            var highlightRangeStr = string.Join(
                string.Empty, highlightRange.Select(x => x.Value));
            highlightRangeStr.Should().Be("Life, Liberty and the pursuit of Happiness");
            highlightTexts[14].Type.Should().Be(HighlightTextType.Text);
            highlightTexts[14].Value.Should().Be(".");
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
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestCompound()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("life", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("liberty", x => x.Body))
                        .MustNot(
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("property", x => x.Body))
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("pursuit of happiness", x => x.Body)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestExists()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Text("life, liberty, and the pursuit of happiness", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Exists(x => x.Title)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestFilter()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .Filter(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestGeoShape()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .GeoShape(__testPolygon, x => x.Coordinates, GeoShapeRelation.Contains))
                .Limit(1)
                .ToList();
            results.Should().BeEmpty();
        }

        [Fact]
        public void TestGeoWithin()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .GeoWithin(__testPolygon, x => x.Coordinates))
                .Limit(1)
                .ToList();
            results.Should().BeEmpty();
        }

        [Fact]
        public void TestGeoWithinBox()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .GeoWithin(__testBox, x => x.Coordinates))
                .Limit(1)
                .ToList();
            results.Should().BeEmpty();
        }

        [Fact]
        public void TestGeoWithinCircle()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .GeoWithin(__testCircle, x => x.Coordinates))
                .Limit(1)
                .ToList();
            results.Should().BeEmpty();
        }

        [Fact]
        public void TestMoreLikeThis()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .MoreLikeThis(
                            new HistoricalDocument
                            {
                                Title = "Declaration of Independence",
                                Body = "We hold these truths to be self-evident that all men are created equal..."
                            }))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestMust()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .Must(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestMustNot()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .MustNot(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().NotBe("Declaration of Independence");
        }

        [Fact]
        public void TestNear()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .Near(x => x.Coordinates, __testCircle.Center, 1.0))
                .Limit(1)
                .ToList();
            results.Should().BeEmpty();
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
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestRange()
        {
            var coll = GetGeoTestCollection();
            List<Shipwreck> results = coll.Aggregate()
                .Search(
                    SearchBuilders<Shipwreck>.Search
                        .Compound()
                        .Must(
                            SearchBuilders<Shipwreck>.Search
                                .RangeDouble(x => x.Latitude).Gt(60.1).Lt(60.2),
                            SearchBuilders<Shipwreck>.Search
                                .RangeDouble(x => x.Longitude).Gt(-149.5).Lt(-149.4)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Chart.Should().Be("US,US,graph,Chart 16682");
        }

        [Fact]
        public void TestShould()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Compound()
                        .Should(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Search
                                .Wildcard("happ*", x => x.Body, true))
                        .MinimumShouldMatch(2))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestSpanFirst()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Span(
                            SearchBuilders<HistoricalDocument>.Span
                                .First(
                                    SearchBuilders<HistoricalDocument>.Span
                                        .Term("happiness", x => x.Body),
                                    250)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestSpanNear()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Span(
                            SearchBuilders<HistoricalDocument>.Span
                                .Near(
                                    new List<SpanDefinition<HistoricalDocument>>()
                                    {
                                        SearchBuilders<HistoricalDocument>.Span
                                            .Term("life", x => x.Body),
                                        SearchBuilders<HistoricalDocument>.Span
                                            .Term("liberty", x => x.Body),
                                        SearchBuilders<HistoricalDocument>.Span
                                            .Term("pursuit", x => x.Body),
                                        SearchBuilders<HistoricalDocument>.Span
                                            .Term("happiness", x => x.Body),
                                    },
                                    3,
                                    inOrder: true)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestSpanOr()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Span(
                            SearchBuilders<HistoricalDocument>.Span
                                .Or(
                                    SearchBuilders<HistoricalDocument>.Span
                                        .Term("unalienable", x => x.Body),
                                    SearchBuilders<HistoricalDocument>.Span
                                        .Term("inalienable", x => x.Body))))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestSpanSubtract()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Span(
                            SearchBuilders<HistoricalDocument>.Span
                                .Subtract(
                                    SearchBuilders<HistoricalDocument>.Span
                                        .Term("unalienable", x => x.Body),
                                    SearchBuilders<HistoricalDocument>.Span
                                        .Term("inalienable", x => x.Body))))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
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
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
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
            results.Should().ContainSingle().Which.Title.Should().Be("US Constitution");
        }

        [Fact]
        public void TestMultiPath()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase(
                            "life, liberty, and the pursuit of happiness",
                            SearchBuilders<HistoricalDocument>.Path
                                .Multi(x => x.Title, x => x.Body)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestAnalyzerPath()
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
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestWildcardPath()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase(
                            "life, liberty, and the pursuit of happiness",
                            SearchBuilders<HistoricalDocument>.Path
                                .Wildcard("b*")))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestCount_LowerBound()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                    count: new SearchCountOptions()
                    {
                        Type = SearchCountType.LowerBound,
                        Threshold = 128
                    })
                .Project<HistoricalDocument>(
                    Builders<HistoricalDocument>.Projection
                        .SearchMeta(x => x.MetaResult))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.MetaResult.Count.LowerBound.Should().Be(108);
        }

        [Fact]
        public void TestCount_Total()
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                    count: new SearchCountOptions()
                    {
                        Type = SearchCountType.Total
                    })
                .Project<HistoricalDocument>(
                    Builders<HistoricalDocument>.Projection
                        .SearchMeta(x => x.MetaResult))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.MetaResult.Count.Total.Should().Be(108);
        }

        [Fact]
        public void TestFunctionScore_Path()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Path(x => x.Score));
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Path(x => x.Score, 1));
        }

        [Fact]
        public void TestFunctionScore_Relevance()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Relevance());
        }

        [Fact]
        public void TestFunctionScore_Constant()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Constant(1));
        }

        [Fact]
        public void TestFunctionScore_Add()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Add(
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(1),
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(2)));
        }

        [Fact]
        public void TestFunctionScore_Multiply()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Multiply(
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(1),
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(2)));
        }

        [Fact]
        public void TestFunctionScore_Gauss()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Gauss(x => x.Score, 100, 1));
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Gauss(x => x.Score, 100, 1, 0.1, 1));
        }

        [Fact]
        public void TestFunctionScore_Log()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Log(
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(1)));
        }

        [Fact]
        public void TestFunctionScore_Log1p()
        {
            TestFunctionScore(
                SearchBuilders<HistoricalDocument>.ScoreFunction
                    .Log1p(
                        SearchBuilders<HistoricalDocument>.ScoreFunction
                            .Constant(1)));
        }

        private void TestFunctionScore(ScoreFunction<HistoricalDocument> function)
        {
            var coll = GetTestCollection();
            List<HistoricalDocument> results = coll.Aggregate()
                .Search(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase(
                            "life, liberty, and the pursuit of happiness",
                            x => x.Body,
                            score: SearchBuilders<HistoricalDocument>.Score
                                .Function(function)))
                .Limit(1)
                .ToList();
            results.Should().ContainSingle().Which.Title.Should().Be("Declaration of Independence");
        }

        [Fact]
        public void TestSearchMeta_Count()
        {
            var coll = GetTestCollection();
            SearchMetaResult result = coll.Aggregate()
                .SearchMeta(
                    SearchBuilders<HistoricalDocument>.Search
                        .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                    "default",
                    new SearchCountOptions()
                    {
                        Type = SearchCountType.Total
                    })
                .Single();
            result.Should().NotBeNull();
            result.Count.Should().NotBeNull();
            result.Count.Total.Should().Be(108);
        }

        [Fact]
        public void TestSearchMeta_Facet()
        {
            var coll = GetTestCollection();
            SearchMetaResult result = coll.Aggregate()
                .SearchMeta(
                    SearchBuilders<HistoricalDocument>.Search
                        .Facet(
                            SearchBuilders<HistoricalDocument>.Search
                                .Phrase("life, liberty, and the pursuit of happiness", x => x.Body),
                            SearchBuilders<HistoricalDocument>.Facet
                                .String("string", x => x.Author, 100),
                            SearchBuilders<HistoricalDocument>.Facet
                                .Number("number", x => x.Index, 0, 100),
                            SearchBuilders<HistoricalDocument>.Facet
                                .Date("date", x => x.Date, DateTime.MinValue, DateTime.MaxValue)))
                .Single();
            result.Should().NotBeNull();
            result.Facet.Should().NotBeNull()
                .And.ContainKey("string")
                .And.ContainKey("date");
            result.Facet["string"].Buckets.Should().NotBeNull().And.ContainSingle();
            result.Facet["string"].Buckets[0].Id.Should().Be("machine");
            result.Facet["string"].Buckets[0].Count.Should().Be(108);
            result.Facet["number"].Buckets.Should().NotBeNull().And.ContainSingle();
            result.Facet["number"].Buckets[0].Id.Should().Be(0);
            result.Facet["number"].Buckets[0].Count.Should().Be(0);
            result.Facet["date"].Buckets.Should().NotBeNull().And.ContainSingle();
            result.Facet["date"].Buckets[0].Id.Should().Be(DateTime.MinValue);
            result.Facet["date"].Buckets[0].Count.Should().Be(108);
        }

        private static MongoClient GetTestClient()
        {
            var uri = Environment.GetEnvironmentVariable("ATLAS_SEARCH_URI");
            return new MongoClient(uri);
        }

        private static IMongoCollection<HistoricalDocument> GetTestCollection()
        {
            var client = GetTestClient();
            var db = client.GetDatabase("sample_training");
            return db.GetCollection<HistoricalDocument>("posts");
        }

        private static IMongoCollection<Shipwreck> GetGeoTestCollection()
        {
            var client = GetTestClient();
            var db = client.GetDatabase("sample_geospatial");
            return db.GetCollection<Shipwreck>("shipwrecks");
        }

        [BsonIgnoreExtraElements]
        private class HistoricalDocument
        {
            [BsonId]
            public ObjectId Id { get; set; }

            [BsonElement("body")]
            public string Body { get; set; }

            [BsonElement("author")]
            public string Author { get; set; }

            [BsonElement("title")]
            public string Title { get; set; }

            [BsonElement("highlights")]
            public List<Highlight> Highlights { get; set; }

            [BsonElement("score")]
            public double Score { get; set; }

            [BsonElement("date")]
            public DateTime Date { get; set; }

            [BsonElement("index")]
            public int Index { get; set; }

            [BsonElement("metaResult")]
            public SearchMetaResult MetaResult { get; set; }
        }

        [BsonIgnoreExtraElements]
        private class Shipwreck
        {
            [BsonElement("latdec")]
            public double Latitude { get; set; }

            [BsonElement("londec")]
            public double Longitude { get; set; }

            [BsonElement("chart")]
            public string Chart { get; set; }

            [BsonElement("coordinates")]
            public GeoJson2DGeographicCoordinates Coordinates { get; set; }
        }
    }
}
