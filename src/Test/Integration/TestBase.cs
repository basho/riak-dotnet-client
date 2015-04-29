// <copyright file="TestBase.cs" company="Basho Technologies, Inc.">
// Copyright © 2015 - Basho Technologies, Inc.
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

namespace Test.Integration
{
    using System;
    using System.Runtime.CompilerServices;
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Config;

    [TestFixture]
    public abstract class TestBase
    {
        protected static readonly Random R = new Random();
        protected IRiakEndPoint cluster;
        protected IRiakClient client;
        protected IRiakClusterConfiguration clusterConfig;

        private static readonly ILog Log = Logging.GetLogger(typeof(TestBase));

        static TestBase()
        {
            RiakClient.DisableListKeysWarning = true;
            RiakClient.DisableListBucketsWarning = true;
        }

        public TestBase()
        {
#if NOAUTH
            cluster = RiakCluster.FromConfig("riak1NodeNoAuthConfiguration");
#else
            if (MonoUtil.IsRunningOnMono)
            {
                cluster = RiakCluster.FromConfig("riak1NodeNoAuthConfiguration");
            }
            else
            {
                cluster = RiakCluster.FromConfig("riak1NodeConfiguration");
            }
#endif
            client = cluster.CreateClient();
        }

        [TestFixtureSetUp]
        protected virtual void TestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        protected virtual void TestFixtureTearDown()
        {
        }

        protected string GetRandomKey([CallerMemberName] string memberName = "")
        {
            var key = string.Format("{0}_{1}", memberName, R.Next());
            Log.DebugFormat("Using {0} for {1}() key", key, memberName);
            return key;
        }
    }
}