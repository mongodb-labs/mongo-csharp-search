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
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Labs.Search.ObjectModel.Serializers
{
    public class HighlightTextTypeSerializer : SerializerBase<HighlightTextType>
    {
        private const string hitName = "hit";
        private const string textName = "text";

        public override HighlightTextType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            if (type != BsonType.String)
            {
                throw CreateCannotDeserializeFromBsonTypeException(type);
            }
            string value = context.Reader.ReadString();
            switch (value)
            {
                case hitName:
                    return HighlightTextType.Hit;
                case textName:
                    return HighlightTextType.Text;
                default:
                    throw new NotSupportedException($"Unexpected representation string for HiglightType: {value}");
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, HighlightTextType value)
        {
            switch (value)
            {
                case HighlightTextType.Hit:
                    context.Writer.WriteString(hitName);
                    break;
                case HighlightTextType.Text:
                    context.Writer.WriteString(textName);
                    break;
                default:
                    throw new NotSupportedException($"Unexpected HighlightTextType value: {value}");
            }
        }
    }
}
