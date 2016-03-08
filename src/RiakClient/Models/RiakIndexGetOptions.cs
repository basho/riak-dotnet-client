// <copyright file="RiakIndexGetOptions.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

    /// <summary>
    /// Represents a collection of optional properties when executing secondary index queries.
    /// Each property changes the semantics of the operation slightly. 
    /// </summary>
    public class RiakIndexGetOptions : RiakOptions<RiakIndexGetOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RiakIndexGetOptions" /> class.
        /// </summary>
        public RiakIndexGetOptions()
        {
            MaxResults = null;
        }

        /// <summary>
        /// The option to return the index keys with the Riak object keys.
        /// </summary>
        public bool? ReturnTerms { get; private set; }

        /// <summary>
        /// The option to stream results back as they are available, instead of letting Riak
        /// aggregate the entire result set before returning them.
        /// </summary>
        /// <remarks>
        /// This property is not typically modified, please use the "Stream" version of any 
        /// SecondaryIndex interface to stream queries.
        /// </remarks>
        public bool? Stream { get; private set; }

        /// <summary>
        /// The maximum number of results returned by the query.
        /// </summary>
        public int? MaxResults { get; private set; }

        /// <summary>
        /// The continuation string for this query. 
        /// Used to page results when combined with <see cref="MaxResults"/>.
        /// </summary>
        public string Continuation { get; private set; }

        /// <summary>
        /// The option to sort, or not sort, the results of a non-paginated secondary index query.
        /// If <see cref="MaxResults"/> is set, this property is ignored.
        /// By default results are sorted first by index value, then by key value.
        /// </summary>
        public bool? PaginationSort { get; private set; }

        /// <summary>
        /// The option to filter result terms with a Regex.
        /// </summary>
        public string TermRegex { get; private set; }

        /// <summary>
        /// Fluent setter for the <see cref="ReturnTerms"/> property.
        /// The option to return the index keys with the Riak object keys.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakIndexGetOptions SetReturnTerms(bool value)
        {
            ReturnTerms = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="Stream"/> property.
        /// The option to stream results back as they are available, instead of letting Riak
        /// aggregate the entire result set before returning them.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        /// <remarks>
        /// This property is not typically modified by the end user, 
        /// please use the "Stream" version of any SecondaryIndex interface to stream queries.
        /// </remarks>
        public RiakIndexGetOptions SetStream(bool value)
        {
            Stream = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="MaxResults"/> property.
        /// The maximum number of results returned by the query.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakIndexGetOptions SetMaxResults(int value)
        {
            MaxResults = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="Continuation"/> property.
        /// The continuation string for this query. 
        /// Used to page results when combined with <see cref="MaxResults"/>.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakIndexGetOptions SetContinuation(BigInteger value)
        {
            Continuation = value.ToString();
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="Continuation"/> property.
        /// The continuation string for this query. 
        /// Used to page results when combined with <see cref="MaxResults"/>.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakIndexGetOptions SetContinuation(string value)
        {
            Continuation = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="TermRegex"/> property.
        /// The option to filter result terms with a Regex.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
        public RiakIndexGetOptions SetTermRegex(string value)
        {
            TermRegex = value;
            return this;
        }

        /// <summary>
        /// Fluent setter for the <see cref="PaginationSort"/> property.
        /// The option to sort, or not sort, the results of a non-paginated secondary index query.
        /// If <see cref="MaxResults"/> is set, this property is ignored.
        /// By default results are sorted first by index value, then by key value.
        /// </summary>
        /// <param name="value">The value to set the property to.</param>
        /// <returns>A reference to the current options object.</returns>
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
                request.max_results = (uint)MaxResults.Value;
            }

            if (!string.IsNullOrEmpty(Continuation))
            {
                request.continuation = Continuation.ToRiakString();
            }

            request.timeoutSpecified = false;
            if (Timeout.HasValue)
            {
                request.timeout = (uint)Timeout;
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
