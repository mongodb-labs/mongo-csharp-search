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

using MongoDB.Driver.Core.Misc;
using System;

namespace MongoDB.Labs.Search
{
    internal static class EnsureExtensions
    {
        public static double IsGreaterThanZero(double value, string paramName)
        {
            if (value <= 0)
            {
                var message = string.Format("Value is not greater than zero: {0}", value);
                throw new ArgumentOutOfRangeException(paramName, message);
            }
            return value;
        }

        public static int? IsNullOrBetween(int? value, int min, int max, string paramName)
        {
            if (value != null)
            {
                Ensure.IsBetween(value.Value, min, max, paramName);
            }
            return value;
        }
    }
}
