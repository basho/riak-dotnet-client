// <copyright file="UpdateMapTests.cs" company="Basho Technologies, Inc.">
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
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Models;

    public class UpdateMapTests : TestBase
    {
        private const string BucketType = "maps";
        private const string Bucket = "map_tests";
        private readonly IList<string> keys = new List<string>();

        [Test]
        public void Fetching_A_Map_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveMap(key);

            var fetch = new FetchMap.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            MapResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNotEmpty(response.Context);
            Assert.IsNotNull(response.Map);
            Assert.AreEqual(1, response.Map.Counters["counter_1"]);
            Assert.AreEqual((RiakString)"value_1", (RiakString)response.Map.Sets["set_1"][0]);
            Assert.AreEqual((RiakString)"register_value_1", (RiakString)response.Map.Registers["register_1"]);
            Assert.AreEqual(true, response.Map.Flags["flag_1"]);

            Map map2 = response.Map.Maps["map_2"];
            Assert.AreEqual(2, map2.Counters["counter_1"]);
            Assert.AreEqual(RiakString.ToBytes("value_1"), map2.Sets["set_1"][0]);
            Assert.AreEqual(RiakString.ToBytes("register_value_1"), map2.Registers["register_1"]);
            Assert.AreEqual(true, map2.Flags["flag_1"]);

            Map map3 = map2.Maps["map_3"];
            Assert.AreEqual(3, map3.Counters["counter_1"]);
        }

        [Test]
        public void Can_Remove_Data_From_A_Map()
        {
            string key = Guid.NewGuid().ToString();
            byte[] context = SaveMap(key);

            var mapOp = new UpdateMap.MapOperation();
            mapOp.RemoveCounter("counter_1");

            var update = new UpdateMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithMapOperation(mapOp)
                .WithContext(context)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            MapResponse response = update.Response;
            Assert.False(response.Map.Counters.ContainsKey("counter_1"));
        }

        [Test]
        public void Fetching_An_Unknown_Map_Results_In_Not_Found()
        {
            var fetch = new FetchMap.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            MapResponse response = fetch.Response;
            Assert.AreEqual(MapResponse.NotFoundResponse, response);
        }

        protected override void TestFixtureTearDown()
        {
            foreach (string key in keys)
            {
                var id = new RiakObjectId(BucketType, Bucket, key);
                client.Delete(id);
            }
        }

        private byte[] SaveMap(string key)
        {
            keys.Add(key);

            var mapOp = new UpdateMap.MapOperation();
            mapOp.IncrementCounter("counter_1", 1)
                .AddToSet("set_1", "value_1")
                .SetRegister("register_1", "register_value_1")
                .SetFlag("flag_1", true);

            var map_2 = mapOp.Map("map_2");
            map_2.IncrementCounter("counter_1", 2)
                .AddToSet("set_1", "value_1")
                .SetRegister("register_1", "register_value_1")
                .SetFlag("flag_1", true);

            var map_3 = map_2.Map("map_3");
            map_3.IncrementCounter("counter_1", 3);

            var update = new UpdateMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithMapOperation(mapOp)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            return update.Response.Context;
        }
    }
}