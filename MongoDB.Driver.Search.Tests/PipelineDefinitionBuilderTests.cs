using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using Xunit;

namespace MongoDB.Driver.Search.Tests
{
    public class PipelineDefinitionBuilderTests
    {
        [Fact]
        public void Search_should_add_expected_stage()
        {
            var pipeline = new EmptyPipelineDefinition<BsonDocument>();
            var builder = new SearchDefinitionBuilder<BsonDocument>();
            var result = pipeline.Search(builder.Text("foo", "bar"));
            var stages = RenderStages(result, BsonDocumentSerializer.Instance);
            Assert.Equal(
                "{ \"$search\" : { \"text\" : { \"query\" : \"foo\", \"path\" : \"bar\" } } }",
                stages[0].ToString());
        }

        [Fact]
        public void Search_should_throw_when_pipeline_is_null()
        {
            PipelineDefinition<BsonDocument, BsonDocument> pipeline = null;
            var builder = new SearchDefinitionBuilder<BsonDocument>();
            var exception = Record.Exception(() => pipeline.Search(builder.Text("foo", "bar")));
            var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("pipeline", argumentNullException.ParamName);
        }

        [Fact]
        public void Search_should_throw_when_query_is_null()
        {
            var pipeline = new EmptyPipelineDefinition<BsonDocument>();
            var exception = Record.Exception(() => pipeline.Search(null));
            var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("query", argumentNullException.ParamName);
        }

        // private methods
        private IList<BsonDocument> RenderStages<TInput, TOutput>(PipelineDefinition<TInput, TOutput> pipeline, IBsonSerializer<TInput> inputSerializer)
        {
            var renderedPipeline = pipeline.Render(inputSerializer, BsonSerializer.SerializerRegistry);
            return renderedPipeline.Documents;
        }
    }
}
