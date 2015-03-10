// <copyright file="RiakSearchRequest.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient.Models.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

    /// <summary>
    /// An enumeration of Riak Search result sort orders
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
    /// An enumeration of different default search query operators.
    /// </summary>
    public enum DefaultOperation
    {
        /// <summary>
        /// The and operator.
        /// </summary>
        And,

        /// <summary>
        /// The or operator.
        /// </summary>
        Or
    }

    /// <summary>
    /// Represents a Riak Search request.
    /// </summary>
    public class RiakSearchRequest
    {
        /// <summary>
        /// The query to run for the search.
        /// </summary>
        public RiakFluentSearch Query { get; set; }

        /// <summary>
        /// The maximum number of rows to return.
        /// </summary>
        /// <remarks>
        /// Combine with <see cref="Start"/> to implement paging.
        /// Distributed pagination in Riak Search cannot be used reliably when sorting on fields 
        /// that can have different values per replica of the same object, namely score and _yz_id. 
        /// In the case of sorting by these fields, you may receive redundant objects. 
        /// In the case of score, the top-N can return different results over multiple runs.
        /// </remarks>
        public long Rows { get; set; }

        /// <summary>
        /// The starting row to return.
        /// </summary>
        /// <remarks>
        /// Combine with <see cref="Rows"/> to implement paging.
        /// Distributed pagination in Riak Search cannot be used reliably when sorting on fields 
        /// that can have different values per replica of the same object, namely score and _yz_id. 
        /// In the case of sorting by these fields, you may receive redundant objects. 
        /// In the case of score, the top-N can return different results over multiple runs.
        /// </remarks>
        public long Start { get; set; }

        /// <summary>
        /// A <see cref="RiakFluentSearch"/> "filter" to run on the query.
        /// </summary>
        public RiakFluentSearch Filter { get; set; }

        /// <summary>
        /// The field to sort on.
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Presort the results by Key or Score.
        /// </summary>
        public PreSort? PreSort { get; set; }

        /// <summary>
        /// The default operator for parsing queries.
        /// </summary>
        /// <remarks>Defaults to <see cref="E:DefaultOperation.And"/> if not specified.</remarks>
        public DefaultOperation? DefaultOperation { get; set; }

        /// <summary>
        /// The list of fields that should be returned for each record in the result list.
        /// </summary>
        /// <remarks>
        /// The 'id' field is always returned, even if not specified in this list.
        /// </remarks>
        public List<string> ReturnFields { get; set; }

        internal RpbSearchQueryReq ToMessage()
        {
            var msg = new RpbSearchQueryReq
            {
                index = Query.Index.ToRiakString(),
                q = Query.ToString().ToRiakString(),
                rows = (uint)Rows,
                start = (uint)Start,
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
