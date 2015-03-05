// <copyright file="RiakRestResponse.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Represents a Riak HTTP REST Response
    /// </summary>
    public class RiakRestResponse
    {
        /// <summary>
        /// The content-type of the HTTP response.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The content length of the HTTP response.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// The content-encoding of the HTTP response.
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// The body of the HTTP response.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The <see cref="HttpStatusCode"/> of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The headers of the HTTP response.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The error message (if any) of the HTTP response.
        /// </summary>
        /// <remarks>Not used anymore.</remarks>
        public string ErrorMessage { get; set; }
    }
}
