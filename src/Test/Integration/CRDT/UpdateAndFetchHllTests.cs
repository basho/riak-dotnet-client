// <copyright file="UpdateAndFetchHllTests.cs" company="Basho Technologies, Inc.">
// Copyright 2016 - Basho Technologies, Inc.
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

namespace Test.Integration.CRDT
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Util;

    [TestFixture, IntegrationHllTest]
    public class UpdateAndFetchHllTests : TestBase
    {
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchHllTests));

        private static readonly ISet<byte[]> DefaultAdds = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("add_1"),
                Encoding.UTF8.GetBytes("add_2")
            };

        protected override RiakString BucketType
        {
            get { return new RiakString("hlls"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("hll_tests"); }
        }

        [Test]
        public void Fetching_A_Hll_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveHll(key);

            var fetch = new FetchHll.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            HllResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNull(response.Context);
            Assert.AreEqual(2, response.Cardinality);
        }

        [Test]
        public void Can_Update_A_Hll()
        {
            string key = Guid.NewGuid().ToString();
            SaveHll(key);

            var add_3 = new RiakString("add_3");
            var adds = new HashSet<string> { add_3 };

            var update = new UpdateHll.Builder(adds)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            HllResponse response = update.Response;
            Assert.AreEqual(3, response.Cardinality);
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            HllResponse r = SaveHll();
            Assert.True(EnumerableUtil.NotNullOrEmpty((string)r.Key));
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_Hll_Results_In_Not_Found()
        {
            var fetch = new FetchHll.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            HllResponse response = fetch.Response;
            Assert.IsTrue(response.NotFound);
        }

        private HllResponse SaveHll(string key = null)
        {
            var updateBuilder = new UpdateHll.Builder(DefaultAdds)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            UpdateHll cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            HllResponse response = cmd.Response;
            Keys.Add(response.Key);
            Assert.IsNull(response.Context);
            return response;
        }
    }
}
