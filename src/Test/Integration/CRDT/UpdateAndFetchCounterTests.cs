// <copyright file="UpdateAndFetchCounterTests.cs" company="Basho Technologies, Inc.">
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
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Models;

    public class UpdateAndFetchCounterTests : TestBase
    {
        private const string BucketType = "counters";
        private const string Bucket = "counter_tests";
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchCounterTests));

        private static readonly long DefaultIncrement = 10;
        private readonly IList<string> keys = new List<string>();

        [Test]
        public void Fetching_A_Counter_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveCounter(key);

            var fetch = new FetchCounter.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            CounterResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNotEmpty(response.Context);
            Assert.AreEqual(DefaultIncrement, response.Value);
        }

        [Test]
        public void Can_Update_A_Counter()
        {
            string key = Guid.NewGuid().ToString();
            CounterResponse r = SaveCounter(key);

            var update = new UpdateCounter.Builder(DefaultIncrement)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithContext(r.Context)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            CounterResponse response = update.Response;
            Assert.AreEqual(20, response.Value);
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            CounterResponse r = SaveCounter();
            Assert.IsNotNullOrEmpty(r.Key);
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_Counter_Results_In_Not_Found()
        {
            var fetch = new FetchCounter.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            CounterResponse response = fetch.Response;
            Assert.IsTrue(response.NotFound);
        }

        protected override void TestFixtureTearDown()
        {
            foreach (string key in keys)
            {
                var id = new RiakObjectId(BucketType, Bucket, key);
                client.Delete(id);
            }
        }

        private CounterResponse SaveCounter(string key = null)
        {
            var updateBuilder = new UpdateCounter.Builder(DefaultIncrement)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            UpdateCounter cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            CounterResponse response = cmd.Response;
            keys.Add(response.Key);
            return response;
        }
    }
}