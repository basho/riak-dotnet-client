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
    using Util;

    public class RiakRestRequest
    {
        private readonly string uri;
        private readonly string method;
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();
        private readonly IDictionary<string, string> queryParams = new Dictionary<string, string>();

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

        public string Uri
        {
            get { return uri; }
        }

        public string Method
        {
            get { return method; }
        }

        public string ContentType { get; set; }

        public byte[] Body { get; set; }

        public IDictionary<string, string> Headers
        {
            get { return headers; }
        }

        public IDictionary<string, string> QueryParams
        {
            get { return queryParams; }
        }

        public Timeout Timeout { get; set; }

        public bool Cache { get; set; }

        public RiakRestRequest AddQueryParam(string key, string value)
        {
            queryParams.Add(key, value);
            return this;
        }

        public RiakRestRequest AddHeader(string key, string value)
        {
            headers.Add(key, value);
            return this;
        }
    }
}
