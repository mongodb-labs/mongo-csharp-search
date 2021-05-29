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

using MongoDB.Bson.Serialization;
using MongoDB.Labs.Search.ObjectModel;
using Xunit;

namespace MongoDB.Labs.Search.Tests.ObjectModel
{
    public class HighlightTests
    {
        [Fact]
        public void TestDeserialize()
        {
            var json = "{ path: 'abc', texts: [{ value: 'foo', type: 'hit' }, { value: 'bar', type: 'text' }], score: 1.23 }";
            var highlight = BsonSerializer.Deserialize<Highlight>(json);
            Assert.Equal("abc", highlight.Path);
            Assert.Equal(2, highlight.Texts.Count);
            Assert.Equal("foo", highlight.Texts[0].Value);
            Assert.Equal(HighlightTextType.Hit, highlight.Texts[0].Type);
            Assert.Equal("bar", highlight.Texts[1].Value);
            Assert.Equal(HighlightTextType.Text, highlight.Texts[1].Type);
            Assert.Equal(1.23, highlight.Score);
        }
    }
}
