// <copyright file="RiakRestRequest.cs" company="Basho Technologies, Inc.">
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

namespace RiakClient.Models.Rest
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Riak HTTP REST Request
    /// </summary>
    public class RiakRestRequest
    {
        private readonly string uri;
        private readonly string method;
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();
        private readonly IDictionary<string, string> queryParams = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RiakRestRequest"/> class.
        /// </summary>
        /// <param name="uri">The URI to make the request to.</param>
        /// <param name="method">A string representing the HTTP method to make the request with.</param>
        public RiakRestRequest(string uri, string method)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentNullException("uri");
            }

            this.uri = uri;

            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentNullException("method");
            }

            this.method = method;

            Timeout = RiakConstants.Defaults.Rest.Timeout;
            Cache = false;
        }

        /// <summary>
        /// The URI to make the request to.
        /// </summary>
        public string Uri
        {
            get { return uri; }
        }

        /// <summary>
        /// A string representing the HTTP method to make the request with.
        /// </summary>
        public string Method
        {
            get { return method; }
        }

        /// <summary>
        /// The content-type of the request.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The body of the HTTP request, if any.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// The HTTP headers of the request.
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get { return headers; }
        }

        /// <summary>
        /// The query parameters of the HTTP request.
        /// </summary>
        public IDictionary<string, string> QueryParams
        {
            get { return queryParams; }
        }

        /// <summary>
        /// The timeout for the request.
        /// </summary>
        public Timeout Timeout { get; set; }

        /// <summary>
        /// The option to accept a cached version or not.
        /// </summary>
        /// <remarks>Not used.</remarks>
        public bool Cache { get; set; }

        /// <summary>
        /// Add a query parameter to the <see cref="QueryParams"/> collection.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakRestRequest AddQueryParam(string key, string value)
        {
            queryParams.Add(key, value);
            return this;
        }

        /// <summary>
        /// Add a header to the <see cref="Headers"/> collection.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value.</param>
        /// <returns>A reference to this updated instance, for fluent chaining.</returns>
        public RiakRestRequest AddHeader(string key, string value)
        {
            headers.Add(key, value);
            return this;
        }
    }
}
