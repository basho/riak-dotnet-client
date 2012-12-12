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

using CorrugatedIron.Messages;
using System.Collections.ObjectModel;
using System.Linq;

namespace CorrugatedIron.Models.Search
{
    public class RiakSearchResult
    {
        public float MaxScore { get; private set; }
        public uint NumFound { get; private set; }
        public ReadOnlyCollection<RiakSearchResultDocument> Documents { get; private set; }

        internal RiakSearchResult(RpbSearchQueryResp response)
        {
            MaxScore = response.max_score;
            NumFound = response.num_found;

            var docs = response.docs.Select(d => new RiakSearchResultDocument(d));
            Documents = new ReadOnlyCollection<RiakSearchResultDocument>(docs.ToList());
        }
    }
}
