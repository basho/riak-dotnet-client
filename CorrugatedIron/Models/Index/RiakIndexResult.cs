// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.Index
{
    public class RiakIndexResult : IRiakIndexResult
    {
        private readonly string _continuation;
        private readonly bool _done;
        private readonly IEnumerable<RiakIndexKeyTerm> _indexKeyTerms;

        public string Continuation
        {
            get { return _continuation; }
        }

        public bool Done
        {
            get { return _done; }
        }

        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get { return _indexKeyTerms; }
        }

        internal RiakIndexResult(bool includeTerms, RpbIndexResp response)
        {
            _done = response.done;

            if (response.continuation != null)
            {
                _continuation = response.continuation.FromRiakString();
            }

            if (includeTerms)
            {
                _indexKeyTerms = response.results.Select(pair =>
                                                new RiakIndexKeyTerm(pair.value.FromRiakString(),
                                                                    pair.key.FromRiakString()));
            }
            else
            {
                _indexKeyTerms = response.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }
        }
    }
}
