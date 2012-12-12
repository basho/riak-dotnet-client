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

using CorrugatedIron.Util;
using System.Collections.Generic;

namespace CorrugatedIron.Models.Rest
{
    public class RiakRestRequest
    {
        public string Uri { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public byte[] Body { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public Dictionary<string, string> QueryParams { get; private set; }
        public int Timeout { get; set; }
        public bool Cache { get; set; }

        public RiakRestRequest(string uri, string method)
        {
            Uri = uri;
            Method = method;
            Headers = new Dictionary<string, string>();
            QueryParams = new Dictionary<string, string>();
            Timeout = RiakConstants.Defaults.Rest.Timeout;
            Cache = false;
        }

        public RiakRestRequest AddQueryParam(string key, string value)
        {
            QueryParams.Add(key, value);
            return this;
        }

        public RiakRestRequest AddHeader(string key, string value)
        {
            Headers.Add(key, value);
            return this;
        }
    }
}