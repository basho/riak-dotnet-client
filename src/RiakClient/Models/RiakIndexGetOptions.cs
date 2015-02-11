// <copyright file="RiakIndexGetOptions.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models
{
    using System.Numerics;
    using System.Runtime.InteropServices;
    using Extensions;
    using Messages;

    [ComVisible(false)]
    public class RiakIndexGetOptions : RiakOptions<RiakIndexGetOptions>
    {
        public RiakIndexGetOptions()
        {
            MaxResults = null;
        }

        public bool? ReturnTerms { get; private set; }

        public bool? Stream { get; private set; }

        public uint? MaxResults { get; private set; }

        public string Continuation { get; private set; }

        public bool? PaginationSort { get; private set; }

        public string TermRegex { get; private set; }

        public RiakIndexGetOptions SetReturnTerms(bool value)
        {
            ReturnTerms = value;
            return this;
        }

        public RiakIndexGetOptions SetStream(bool value)
        {
            Stream = value;
            return this;
        }

        public RiakIndexGetOptions SetMaxResults(uint value)
        {
            MaxResults = value;
            return this;
        }

        public RiakIndexGetOptions SetContinuation(BigInteger value)
        {
            Continuation = value.ToString();
            return this;
        }

        public RiakIndexGetOptions SetContinuation(string value)
        {
            Continuation = value;
            return this;
        }

        public RiakIndexGetOptions SetTermRegex(string value)
        {
            TermRegex = value;
            return this;
        }

        public RiakIndexGetOptions SetPaginationSort(bool value)
        {
            PaginationSort = value;
            return this;
        }

        internal void Populate(RpbIndexReq request)
        {
            if (ReturnTerms.HasValue)
            {
                request.return_terms = ReturnTerms.Value;
            }

            if (Stream.HasValue)
            {
                request.stream = Stream.Value;
            }

            if (MaxResults.HasValue)
            {
                request.max_results = MaxResults.Value;
            }

            if (!string.IsNullOrEmpty(Continuation))
            {
                request.continuation = Continuation.ToRiakString();
            }

            if (Timeout.HasValue)
            {
                request.timeout = (uint)Timeout.Value;
            }

            if (!string.IsNullOrEmpty(TermRegex))
            {
                request.term_regex = TermRegex.ToRiakString();
            }

            if (PaginationSort.HasValue)
            {
                request.pagination_sort = PaginationSort.Value;
            }
        }
    }
}
