using MongoDB.Bson;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Search
{
    public static class PipelineStageDefinitionBuilder
    {
        public static PipelineStageDefinition<TInput, TInput> Search<TInput>(
            SearchDefinition<TInput> query)
        {
            Ensure.IsNotNull(query, nameof(query));

            const string operatorName = "$search";
            var stage = new DelegatedPipelineStageDefinition<TInput, TInput>(
                operatorName,
                (s, sr) => new RenderedPipelineStageDefinition<TInput>(operatorName, new BsonDocument(operatorName, query.Render(s, sr)), s));

            return stage;
        }
    }
}
