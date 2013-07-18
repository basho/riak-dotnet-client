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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.Index
{
    public class RiakIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakIndexKeyTerm> _indexKeyTerms;

        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms
        {
            get { return _indexKeyTerms; }
        }

        internal RiakIndexResult(bool includeTerms, RiakResult<RpbIndexResp> response)
        {
            if (includeTerms)
            {
                _indexKeyTerms = response.Value.results.Select(pair =>
                                                new RiakIndexKeyTerm(pair.value.FromRiakString(),
                                                                    pair.key.FromRiakString()));
            }
            else
            {
                _indexKeyTerms = response.Value.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
            }
        }
    }
}
