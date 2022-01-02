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

using System.Collections.Generic;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Labs.Search
{
    internal sealed class CompoundFluentImpl<TDocument> : CompoundFluent<TDocument>
    {
        private List<SearchDefinition<TDocument>> _must;
        private List<SearchDefinition<TDocument>> _mustNot;
        private List<SearchDefinition<TDocument>> _should;
        private List<SearchDefinition<TDocument>> _filter;
        private int _minimumShouldMatch = 0;

        public override CompoundFluent<TDocument> Must(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            if (_must == null)
            {
                _must = new List<SearchDefinition<TDocument>>();
            }
            _must.AddRange(Ensure.IsNotNull(clauses, nameof(clauses)));
            return this;
        }

        public override CompoundFluent<TDocument> MustNot(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            if (_mustNot == null)
            {
                _mustNot = new List<SearchDefinition<TDocument>>();
            }
            _mustNot.AddRange(Ensure.IsNotNull(clauses, nameof(clauses)));
            return this;
        }

        public override CompoundFluent<TDocument> Should(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            if (_should == null)
            {
                _should = new List<SearchDefinition<TDocument>>();
            }
            _should.AddRange(Ensure.IsNotNull(clauses, nameof(clauses)));
            return this;
        }

        public override CompoundFluent<TDocument> Filter(IEnumerable<SearchDefinition<TDocument>> clauses)
        {
            if (_filter == null)
            {
                _filter = new List<SearchDefinition<TDocument>>();
            }
            _filter.AddRange(Ensure.IsNotNull(clauses, nameof(clauses)));
            return this;
        }

        public override CompoundFluent<TDocument> MinimumShouldMatch(int minimumShouldMatch)
        {
            _minimumShouldMatch = minimumShouldMatch;
            return this;
        }

        public override SearchDefinition<TDocument> ToSearchDefinition()
        {
            return new CompoundSearchDefinition<TDocument>(_must, _mustNot, _should, _filter, _minimumShouldMatch);
        }
    }
}
