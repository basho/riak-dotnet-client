// <copyright file="TestBase.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Config;
    using RiakClient.Models;
    using RiakClient.Util;

    [TestFixture, IntegrationTest]
    public abstract class TestBase
    {
        protected static readonly Random R = new Random();

        protected readonly IList<string> Keys = new List<string>();

        protected IRiakEndPoint cluster;
        protected IRiakClient client;
        protected IRiakClusterConfiguration clusterConfig;

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

        protected abstract RiakString BucketType
        {
            get;
        }

        protected abstract RiakString Bucket
        {
            get;
        }

        [TestFixtureSetUp]
        protected virtual void TestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        protected virtual void TestFixtureTearDown()
        {
            foreach (string key in Keys)
            {
                var id = new RiakObjectId(BucketType, Bucket, key);
                client.Delete(id);
            }
        }
    }
}