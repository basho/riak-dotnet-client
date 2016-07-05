// <copyright file="FetchMapTests.cs" company="Basho Technologies, Inc.">
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

namespace Test.Unit.CRDT
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class FetchMapTests
    {
        private static readonly RiakString BucketType = "maps";
        private static readonly RiakString Bucket = "myBucket";
        private static readonly RiakString Key = "map_1";

        [Test]
        public void Should_Build_DtFetchReq_Correctly()
        {
            var fetch = new FetchMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithR((Quorum)1)
                .WithPR((Quorum)2)
                .WithNotFoundOK(true)
                .WithBasicQuorum(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            DtFetchReq protobuf = (DtFetchReq)fetch.ConstructRequest(false);

            Assert.AreEqual(BucketType, (RiakString)protobuf.type);
            Assert.AreEqual(Bucket, (RiakString)protobuf.bucket);
            Assert.AreEqual(Key, (RiakString)protobuf.key);
            Assert.AreEqual(1, protobuf.r);
            Assert.AreEqual(2, protobuf.pr);
            Assert.AreEqual(true, protobuf.notfound_ok);
            Assert.AreEqual(true, protobuf.basic_quorum);
            Assert.AreEqual(20000, protobuf.timeout);
        }

        [Test]
        public void Should_Construct_MapResponse_From_DtFetchResp()
        {
            var key = new RiakString("riak_generated_key");
            var context = new RiakString("1234");

            var fetchResp = new DtFetchResp();
            fetchResp.type = DtFetchResp.DataType.MAP;
            fetchResp.value = new DtValue();
            fetchResp.context = context;

            Func<IEnumerable<MapEntry>> createMapEntries = () =>
            {
                var mapEntries = new List<MapEntry>();

                var mapField = new MapField();
                mapField.type = MapField.MapFieldType.COUNTER;
                mapField.name = new RiakString("counter_1");
                var mapEntry = new MapEntry();
                mapEntry.field = mapField;
                mapEntry.counter_value = 50;
                mapEntries.Add(mapEntry);

                mapField = new MapField();
                mapField.type = MapField.MapFieldType.SET;
                mapField.name = new RiakString("set_1");
                mapEntry = new MapEntry();
                mapEntry.field = mapField;
                mapEntry.set_value.Add(RiakString.ToBytes("value_1"));
                mapEntry.set_value.Add(RiakString.ToBytes("value_2"));
                mapEntries.Add(mapEntry);

                mapField = new MapField();
                mapField.type = MapField.MapFieldType.REGISTER;
                mapField.name = new RiakString("register_1");
                mapEntry = new MapEntry();
                mapEntry.field = mapField;
                mapEntry.register_value = RiakString.ToBytes("1234");
                mapEntries.Add(mapEntry);

                mapField = new MapField();
                mapField.type = MapField.MapFieldType.FLAG;
                mapField.name = new RiakString("flag_1");
                mapEntry = new MapEntry();
                mapEntry.field = mapField;
                mapEntry.flag_value = true;
                mapEntries.Add(mapEntry);

                return mapEntries;
            };

            fetchResp.value.map_value.AddRange(createMapEntries());

            var map_1_field = new MapField();
            map_1_field.type = MapField.MapFieldType.MAP;
            map_1_field.name = new RiakString("map_1");
            var map_1_entry = new MapEntry();
            map_1_entry.field = map_1_field;
            map_1_entry.map_value.AddRange(createMapEntries());

            fetchResp.value.map_value.Add(map_1_entry);

            Action<Map> verifyMap = (map) =>
            {
                Assert.AreEqual(50, map.Counters["counter_1"]);
                Assert.AreEqual(RiakString.ToBytes("value_1"), map.Sets["set_1"][0]);
                Assert.AreEqual(RiakString.ToBytes("value_2"), map.Sets["set_1"][1]);
                Assert.AreEqual(RiakString.ToBytes("1234"), map.Registers["register_1"]);
                Assert.IsTrue(map.Flags["flag_1"]);
            };

            var fetch = new FetchMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .Build();

            fetch.OnSuccess(fetchResp);

            MapResponse response = fetch.Response;

            Assert.NotNull(response);
            Assert.AreEqual(key, response.Key);
            Assert.AreEqual(RiakString.ToBytes(context), response.Context);

            verifyMap(response.Value);
            verifyMap(response.Value.Maps["map_1"]);
        }
    }
}
