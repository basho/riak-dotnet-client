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

namespace Test.Unit.CRDT
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;
    using RiakClient.Util;

    [TestFixture]
    public class UpdateMapTests
    {
        private const string BucketType = "maps";
        private const string Bucket = "myBucket";
        private const string Key = "map_1";
        private static readonly byte[] Context = Encoding.UTF8.GetBytes("test-context");

        [Test]
        public void Should_Build_DtUpdateReq_Correctly()
        {
            var mapOp = new UpdateMap.MapOperation()
                .IncrementCounter("counter_1", 50)
                .RemoveCounter("counter_2")
                .AddToSet("set_1", "set_value_1")
                .RemoveFromSet("set_2", "set_value_2")
                .RemoveSet("set_3")
                .SetRegister("register_1", "register_value_1")
                .RemoveRegister("register_2")
                .SetFlag("flag_1", true)
                .RemoveFlag("flag_2")
                .RemoveMap("map_3");

            mapOp.Map("map_2").IncrementCounter("counter_1", 50)
                .RemoveCounter("counter_2")
                .AddToSet("set_1", "set_value_1")
                .RemoveFromSet("set_2", "set_value_2")
                .RemoveSet("set_3")
                .SetRegister("register_1", "register_value_1")
                .RemoveRegister("register_2")
                .SetFlag("flag_1", true)
                .RemoveFlag("flag_2")
                .RemoveMap("map_3");

            var updateMapCommandBuilder = new UpdateMap.Builder();

            var q1 = new Quorum(1);
            var q2 = new Quorum(2);
            var q3 = new Quorum(3);

            updateMapCommandBuilder
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithMapOperation(mapOp)
                .WithContext(Context)
                .WithW(q3)
                .WithPW(q1)
                .WithDW(q2)
                .WithReturnBody(true)
                .WithIncludeContext(false)
                .WithTimeout(TimeSpan.FromSeconds(20));

            UpdateMap updateMapCommand = updateMapCommandBuilder.Build();

            DtUpdateReq protobuf = (DtUpdateReq)updateMapCommand.ConstructPbRequest();

            Assert.AreEqual(Encoding.UTF8.GetBytes(BucketType), protobuf.type);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Bucket), protobuf.bucket);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Key), protobuf.key);
            Assert.AreEqual(q3, protobuf.w);
            Assert.AreEqual(q1, protobuf.pw);
            Assert.AreEqual(q2, protobuf.dw);
            Assert.IsTrue(protobuf.return_body);
            Assert.IsFalse(protobuf.include_context);
            Assert.AreEqual(20000, protobuf.timeout);
            Assert.AreEqual(Context, protobuf.context);

            MapOp mapOpMsg = protobuf.op.map_op;

            VerifyRemoves(mapOpMsg.removes);
            MapUpdate innerMapUpdate = VerifyUpdates(mapOpMsg.updates, true);
            VerifyRemoves(innerMapUpdate.map_op.removes);
            VerifyUpdates(innerMapUpdate.map_op.updates, false);
        }

        [Test]
        public void Should_Construct_MapResponse_From_DtUpdateResp()
        {
            var key = new RiakString("riak_generated_key");
            var context = new RiakString("1234");

            var updateResp = new DtUpdateResp();
            updateResp.key = key;
            updateResp.context = context;

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

            updateResp.map_value.AddRange(createMapEntries());

            var map_1_field = new MapField();
            map_1_field.type = MapField.MapFieldType.MAP;
            map_1_field.name = new RiakString("map_1");
            var map_1_entry = new MapEntry();
            map_1_entry.field = map_1_field;
            map_1_entry.map_value.AddRange(createMapEntries());

            updateResp.map_value.Add(map_1_entry);

            Action<Map> verifyMap = (map) =>
            {
                Assert.AreEqual(50, map.Counters["counter_1"]);
                Assert.AreEqual(RiakString.ToBytes("value_1"), map.Sets["set_1"][0]);
                Assert.AreEqual(RiakString.ToBytes("value_2"), map.Sets["set_1"][1]);
                Assert.AreEqual(RiakString.ToBytes("1234"), map.Registers["register_1"]);
                Assert.IsTrue(map.Flags["flag_1"]);
            };

            var mapOp = new UpdateMap.MapOperation();

            var update = new UpdateMap.Builder()
                .WithBucketType("maps")
                .WithBucket("myBucket")
                .WithKey("map_1")
                .WithMapOperation(mapOp)
                .Build();

            update.OnSuccess(updateResp);

            MapResponse response = update.Response;

            Assert.NotNull(response);
            Assert.AreEqual(key, response.Key);
            Assert.AreEqual(RiakString.ToBytes(context), response.Context);

            verifyMap(response.Value);
            verifyMap(response.Value.Maps["map_1"]);
        }

        private static void VerifyRemoves(ICollection<MapField> mapFields)
        {
            Assert.AreEqual(5, mapFields.Count);

            bool counterRemoved = false;
            bool setRemoval = false;
            bool registerRemoved = false;
            bool flagRemoved = false;
            bool mapRemoved = false;

            foreach (MapField mapField in mapFields)
            {
                switch (mapField.type)
                {
                    case MapField.MapFieldType.COUNTER:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("counter_2"), mapField.name);
                        counterRemoved = true;
                        break;
                    case MapField.MapFieldType.SET:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("set_3"), mapField.name);
                        setRemoval = true;
                        break;
                    case MapField.MapFieldType.MAP:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("map_3"), mapField.name);
                        mapRemoved = true;
                        break;
                    case MapField.MapFieldType.REGISTER:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("register_2"), mapField.name);
                        registerRemoved = true;
                        break;
                    case MapField.MapFieldType.FLAG:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("flag_2"), mapField.name);
                        flagRemoved = true;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsTrue(counterRemoved);
            Assert.IsTrue(setRemoval);
            Assert.IsTrue(registerRemoved);
            Assert.IsTrue(flagRemoved);
            Assert.IsTrue(mapRemoved);
        }

        private static MapUpdate VerifyUpdates(IEnumerable<MapUpdate> updates, bool expectMapUpdate)
        {
            bool counterIncremented = false;
            bool setAddedTo = false;
            bool setRemovedFrom = false;
            bool registerSet = false;
            bool flagSet = false;
            bool mapAdded = false;
            MapUpdate mapUpdate = null;

            foreach (MapUpdate update in updates)
            {
                switch (update.field.type)
                {
                    case MapField.MapFieldType.COUNTER:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("counter_1"), update.field.name);
                        Assert.AreEqual(50, update.counter_op.increment);
                        counterIncremented = true;
                        break;
                    case MapField.MapFieldType.SET:
                        if (!EnumerableUtil.IsNullOrEmpty(update.set_op.adds))
                        {
                            Assert.AreEqual(Encoding.UTF8.GetBytes("set_1"), update.field.name);
                            Assert.AreEqual(Encoding.UTF8.GetBytes("set_value_1"), update.set_op.adds[0]);
                            setAddedTo = true;
                        }
                        else
                        {
                            Assert.AreEqual(Encoding.UTF8.GetBytes("set_2"), update.field.name);
                            Assert.AreEqual(Encoding.UTF8.GetBytes("set_value_2"), update.set_op.removes[0]);
                            setRemovedFrom = true;
                        }

                        break;
                    case MapField.MapFieldType.MAP:
                        if (expectMapUpdate)
                        {
                            Assert.AreEqual(Encoding.UTF8.GetBytes("map_2"), update.field.name);
                            mapAdded = true;
                            mapUpdate = update;
                        }

                        break;
                    case MapField.MapFieldType.REGISTER:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("register_1"), update.field.name);
                        Assert.AreEqual(Encoding.UTF8.GetBytes("register_value_1"), update.register_op);
                        registerSet = true;
                        break;
                    case MapField.MapFieldType.FLAG:
                        Assert.AreEqual(Encoding.UTF8.GetBytes("flag_1"), update.field.name);
                        Assert.AreEqual(MapUpdate.FlagOp.ENABLE, update.flag_op);
                        flagSet = true;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsTrue(counterIncremented);
            Assert.IsTrue(setAddedTo);
            Assert.IsTrue(setRemovedFrom);
            Assert.IsTrue(registerSet);
            Assert.IsTrue(flagSet);

            if (expectMapUpdate)
            {
                Assert.IsTrue(mapAdded);
            }
            else
            {
                Assert.IsFalse(mapAdded);
            }

            return mapUpdate;
        }
    }
}