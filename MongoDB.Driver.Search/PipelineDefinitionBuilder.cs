using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver.Search
{
    public static class PipelineDefinitionBuilder
    {
        public static PipelineDefinition<TInput, TOutput> Search<TInput, TOutput>(
            this PipelineDefinition<TInput, TOutput> pipeline,
            SearchDefinition<TOutput> query)
        {
            Ensure.IsNotNull(pipeline, nameof(pipeline));
            return pipeline.AppendStage(PipelineStageDefinitionBuilder.Search(query));
        }
    }
}
