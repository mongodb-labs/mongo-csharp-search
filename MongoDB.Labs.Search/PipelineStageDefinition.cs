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
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDB.Labs.Search
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
