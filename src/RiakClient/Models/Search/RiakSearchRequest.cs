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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Messages;

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
        private readonly string solrIndex;
        private readonly string solrQuery;
        private readonly string solrFilter;

        public RiakSearchRequest()
        {
        }

        public RiakSearchRequest(string solrIndex, string solrQuery, string solrFilter = null)
        {
            if (string.IsNullOrWhiteSpace(solrIndex))
            {
                throw new ArgumentNullException("solrIndex");
            }

            this.solrIndex = solrIndex;

            if (string.IsNullOrWhiteSpace(solrQuery))
            {
                throw new ArgumentNullException("solrQuery");
            }

            this.solrQuery = solrQuery;
            this.solrFilter = solrFilter;
        }

        public RiakSearchRequest(
            RiakFluentSearch solrQuery,
            RiakFluentSearch solrFilter = null)
        {
            if (solrQuery == null)
            {
                throw new ArgumentNullException("solrQuery");
            }

            this.Query = solrQuery;
            this.Filter = solrFilter;
        }

        public RiakFluentSearch Query { get; set; }

        public RiakFluentSearch Filter { get; set; }

        public long Rows { get; set; }

        public long Start { get; set; }

        public string Sort { get; set; }

        public PreSort? PreSort { get; set; }

        public DefaultOperation? DefaultOperation { get; set; }

        /// <summary>
        /// Gets or sets the list of fields that should be returned for each
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
                index = GetIndexRiakString(),
                q = GetQueryRiakString(),
                rows = (uint)Rows,
                start = (uint)Start,
                sort = Sort.ToRiakString(),
                filter = GetFilterRiakString(),
                presort = PreSort.HasValue ? PreSort.Value.ToString().ToLower().ToRiakString() : null,
                op = DefaultOperation.HasValue ? DefaultOperation.Value.ToString().ToLower().ToRiakString() : null
            };

            if (ReturnFields != null)
            {
                msg.fl.AddRange(ReturnFields.Select(x => x.ToRiakString()));
            }

            return msg;
        }

        private byte[] GetIndexRiakString()
        {
            byte[] indexRiakString = null;

            if (solrIndex != null)
            {
                indexRiakString = solrIndex.ToRiakString();
            }
            else
            {
                indexRiakString = Query.Index.ToRiakString();
            }

            return indexRiakString;
        }

        private byte[] GetQueryRiakString()
        {
            byte[] queryRiakString = null;

            if (solrQuery != null)
            {
                queryRiakString = solrQuery.ToRiakString();
            }
            else
            {
                queryRiakString = Query.ToString().ToRiakString();
            }

            return queryRiakString;
        }

        private byte[] GetFilterRiakString()
        {
            byte[] filterRiakString = null;

            if (solrFilter != null)
            {
                filterRiakString = solrFilter.ToRiakString();
            }
            else if (Filter != null)
            {
                filterRiakString = Filter.ToString().ToRiakString();
            }

            return filterRiakString;
        }
    }
}
