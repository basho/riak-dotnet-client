// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
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

using CorrugatedIron.Config;
using CorrugatedIron.Util;

namespace CorrugatedIron.Tests.Live.MapReduce
{
    public class RiakMapReduceTestBase
    {
        protected const string MrContentType = RiakConstants.ContentTypes.ApplicationJson;
        protected const string EmptyBody = "{}";
        protected string Bucket = "fluent_key_bucket";
        protected IRiakClient Client;
        protected IRiakEndPoint Cluster;
        protected IRiakClusterConfiguration ClusterConfig;

        public RiakMapReduceTestBase(string section = "riak1NodeConfiguration")
        {
            RiakClient.DisableListKeysWarning = true;
            ClusterConfig = RiakClusterConfiguration.LoadFromConfig(section);
        }
    }
}