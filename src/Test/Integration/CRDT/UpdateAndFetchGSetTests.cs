// <copyright file="UpdateAndFetchGSetTests.cs" company="Basho Technologies, Inc.">
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

    public class UpdateAndFetchGSetTests : TestBase
    {
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchGSetTests));

        private static readonly ISet<byte[]> DefaultAdds = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("add_1"),
                Encoding.UTF8.GetBytes("add_2")
            };

        protected override RiakString BucketType
        {
            get { return new RiakString("gsets"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("gset_tests"); }
        }

        public override void TestFixtureSetUp()
        {
            var rslt = client.GetBucketProperties(BucketType, Bucket);
            if (rslt.IsSuccess == false)
            {
                Assert.Ignore("{0} bucket type not present. Ignoring suite.", BucketType);
            }
        }

        [Test]
        public void Fetching_A_GSet_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveGSet(key);

            var fetch = new FetchSet.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNotNull(response.Context);
            Assert.IsNotEmpty(response.Context);
            Assert.AreEqual(DefaultAdds, response.Value);
        }

        [Test]
        public void Can_Update_A_GSet()
        {
            string key = Guid.NewGuid().ToString();
            SetResponse resp = SaveGSet(key);

            var add_3 = new RiakString("add_3");
            var adds = new HashSet<string> { add_3 };

            var update = new UpdateGSet.Builder(adds)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithContext(resp.Context)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = update.Response;
            bool found_add_3 = false;
            foreach (RiakString value in response.Value)
            {
                if (value.Equals(add_3))
                {
                    found_add_3 = true;
                }
            }

            Assert.True(found_add_3);
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            SetResponse r = SaveGSet();
            Assert.True(EnumerableUtil.NotNullOrEmpty((string)r.Key));
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_GSet_Results_In_Not_Found()
        {
            var fetch = new FetchSet.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            SetResponse response = fetch.Response;
            Assert.IsTrue(response.NotFound);
        }

        private SetResponse SaveGSet(string key = null)
        {
            var updateBuilder = new UpdateGSet.Builder(DefaultAdds)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            UpdateGSet cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = cmd.Response;
            Keys.Add(response.Key);

            Assert.True(EnumerableUtil.NotNullOrEmpty(response.Context));

            return response;
        }
    }
}
