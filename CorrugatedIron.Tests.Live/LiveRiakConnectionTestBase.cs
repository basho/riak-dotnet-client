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

using System;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LiveRiakConnectionTests
{
    public class LiveRiakConnectionTestBase
    {
        protected const int TestClientId = 42;
        protected readonly static byte[] ClientId;
        protected const string TestHost = "riak-test";
        protected const int TestPbcPort = 8081;
        protected const int TestHttpPort = 8091;
        protected const string TestBucket = "test_bucket";
        protected const string TestKey = "test_json";
        protected const string TestJson = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}";
        protected const string MapReduceBucket = "map_reduce_bucket";
        protected const string MultiBucket = "test_multi_bucket";
        protected const string MultiKey = "test_multi_key";
        protected const string MultiBodyOne = @"{""dishes"": 9}";
        protected const string MultiBodyTwo = @"{""dishes"": 11}";
        protected const string PropertiesTestBucket = @"propertiestestbucket";

        protected IRiakCluster Cluster;
        protected IRiakClient Client;
        protected IRiakClusterConfiguration ClusterConfig;
        protected Func<IRiakClient> ClientGenerator;

        static LiveRiakConnectionTestBase()
        {
            ClientId = RiakConnection.ToClientId(TestClientId);
        }

        public LiveRiakConnectionTestBase(string section = "riak3NodeConfiguration")
        {
            ClusterConfig = RiakClusterConfiguration.LoadFromConfig(section);
        }

        [SetUp]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakNodeFactory(), new RiakConnectionFactory());
            ClientGenerator = () => new RiakClient(Cluster);
            Client = ClientGenerator();
        }

        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(TestBucket);
            Cluster.Dispose();
        }
    }
}
