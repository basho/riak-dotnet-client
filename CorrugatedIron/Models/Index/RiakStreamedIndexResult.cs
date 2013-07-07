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
    public class RiakStreamedIndexResult : IRiakIndexResult
    {
        private readonly IEnumerable<RiakResult<RpbIndexResp>> _responseReader;
        private readonly bool _includeTerms;

        public RiakStreamedIndexResult(bool includeTerms, IEnumerable<RiakResult<RpbIndexResp>> responseReader)
        {
            _responseReader = responseReader;
            _includeTerms = includeTerms;
        }

        public IEnumerable<RiakIndexKeyTerm> IndexKeyTerms 
        { 
            get
            {
                return _responseReader.SelectMany(item => GetIndexKeyTerm(item.Value));
            }
        }

        private IEnumerable<RiakIndexKeyTerm> GetIndexKeyTerm(RpbIndexResp response)
        {
            if (_includeTerms)
            {
                return response.results.Select(pair =>
                                                new RiakIndexKeyTerm(pair.value.FromRiakString(),
                                                                    pair.key.FromRiakString()));
            }

            return response.keys.Select(key => new RiakIndexKeyTerm(key.FromRiakString()));
        }
    }
}
