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

namespace CorrugatedIron.Models.Search
{
    /// <summary>
    /// Specifies the sort order of Riak Search Results
    /// </summary>
    public enum PreSort
    {
        /// <summary>
        /// Sort the results by bucket key.
        /// </summary>
        Key,
        /// <summary>
        /// Sort the results by search score.
        /// </summary>
        Score
    }

    /// <summary>
    /// Specifies the default_op override.
    /// </summary>
    public enum DefaultOperation
    {
        And,
        Or
    }

    public class RiakSearchRequest
    {
        public RiakFluentSearch Query { get; set; }

        public uint Rows { get; set; }

        public uint Start { get; set; }

        public RiakFluentSearch Filter { get; set; }

        public string Sort { get; set; }

        public PreSort? PreSort { get; set; }

        public DefaultOperation? DefaultOperation { get; set; }

        /// <summary>
        /// Specifies the list of fields that should be returned for each
        /// record in the result list.
        /// </summary>
        /// <remarks>
        /// The 'id' field is always returned, even if not specified in this list.
        /// </remarks>
        public List<string> ReturnFields { get; set; }

        internal RpbSearchQueryReq ToMessage()
        {
            var msg = new RpbSearchQueryReq
            {
                index = Query.Bucket.ToRiakString(),
                q = Query.ToString().ToRiakString(),
                rows = Rows,
                start = Start,
                sort = Sort.ToRiakString(),
                filter = Filter == null ? null : Filter.ToString().ToRiakString(),
                presort = PreSort.HasValue ? PreSort.Value.ToString().ToLower().ToRiakString() : null,
                op = DefaultOperation.HasValue ? DefaultOperation.Value.ToString().ToLower().ToRiakString() : null
            };

            if (ReturnFields != null)
            {
                msg.fl.AddRange(ReturnFields.Select(x => x.ToRiakString()));
            }

            return msg;
        }
    }
}
