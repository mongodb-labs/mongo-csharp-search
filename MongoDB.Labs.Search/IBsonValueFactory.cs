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

namespace MongoDB.Labs.Search
{
    internal interface IBsonValueFactory<TValue>
    {
        BsonValue Create(TValue @value);
    }

    internal class Int32BsonValueFactory : IBsonValueFactory<int>
    {
        public BsonValue Create(int @value)
        {
            return new BsonInt32(@value);
        }
    }

    internal class Int64BsonValueFactory : IBsonValueFactory<long>
    {
        public BsonValue Create(long @value)
        {
            return new BsonInt64(value);
        }
    }

    internal class DoubleBsonValueFactory : IBsonValueFactory<double>
    {
        public BsonValue Create(double @value)
        {
            return new BsonDouble(value);
        }
    }

    internal class DateTimeBsonValueFactory : IBsonValueFactory<DateTime>
    {
        public BsonValue Create(DateTime @value)
        {
            return new BsonDateTime(@value);
        }
    }
}
