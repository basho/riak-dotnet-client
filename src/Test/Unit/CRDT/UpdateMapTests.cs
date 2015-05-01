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
        [Test]
        public void Should_Build_DtUpdateReq_Correctly()
        {
            const string BucketType = "maps";
            const string Bucket = "myBucket";
            const string Key = "map_1";

            byte[] context = Encoding.UTF8.GetBytes("test-context");

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
                .WithContext(context)
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
            Assert.AreEqual(context, protobuf.context);

            MapOp mapOpMsg = protobuf.op.map_op;

            VerifyRemoves(mapOpMsg.removes);
            MapUpdate innerMapUpdate = VerifyUpdates(mapOpMsg.updates, true);
            VerifyRemoves(innerMapUpdate.map_op.removes);
            VerifyUpdates(innerMapUpdate.map_op.updates, false);
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