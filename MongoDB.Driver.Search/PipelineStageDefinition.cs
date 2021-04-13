using System;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver.Search
{
    // This class is identical to the class with the same name in MongoDB.Driver.
    // It is duplicated here because it is internal and hence inaccessible to this
    // library.
    internal sealed class DelegatedPipelineStageDefinition<TInput, TOutput> : PipelineStageDefinition<TInput, TOutput>
    {
        private readonly string _operatorName;
        private readonly Func<IBsonSerializer<TInput>, IBsonSerializerRegistry, RenderedPipelineStageDefinition<TOutput>> _renderer;

        public DelegatedPipelineStageDefinition(string operatorName, Func<IBsonSerializer<TInput>, IBsonSerializerRegistry, RenderedPipelineStageDefinition<TOutput>> renderer)
        {
            _operatorName = operatorName;
            _renderer = renderer;
        }

        public override string OperatorName
        {
            get { return _operatorName; }
        }

        public override RenderedPipelineStageDefinition<TOutput> Render(IBsonSerializer<TInput> inputSerializer, IBsonSerializerRegistry serializerRegistry)
        {
            return _renderer(inputSerializer, serializerRegistry);
        }
    }
}
