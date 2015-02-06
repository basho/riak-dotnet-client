// Copyright (c) 2015 - Basho Technologies, Inc.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Extensions;
using RiakClient.Messages;
using RiakClient.Models;
using RiakClient.Models.RiakDt;

namespace RiakClient.Tests.Live.DataTypes
{
    [TestFixture]
    public class MapDtEdgeCaseTests : DataTypeTestsBase
    {
        public static readonly byte[] NoContext = null;
        public static readonly List<RiakDtMapField> NoRemovals = null;
        public static readonly List<MapUpdate> NoUpdates = null;

        [Test]
        public void Test1()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string setName = "set";

            // New Map with Set
            var setMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            // Add X, Y to set
            var addSet = new List<string> { "X", "Y" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet);

            // Store
            var updatedMap1 = Client.DtUpdateMap(id, Serializer, NoContext, NoRemovals,
                new List<MapUpdate> { setMapUpdate });

            // Add Z
            var setMapUpdate2 = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            var addSet2 = new List<string> { "Z" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate2.set_op.adds.AddRange(addSet2);

            // Remove Set
            var fieldToRemove = new RiakDtMapField(setMapUpdate.field);

            // Store again
            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context,
                new List<RiakDtMapField> { fieldToRemove }, new List<MapUpdate> { setMapUpdate2 });

            Assert.AreEqual(1, updatedMap2.Values.Count);

            var set = updatedMap2.Values.Single(s => s.Field.Name == setName);
            var setValues = set.SetValue.Select(v => Deserializer.Invoke(v)).ToList();

            Assert.AreEqual(1, setValues.Count);
            Assert.Contains("Z", setValues);
        }

        [Test]
        public void Test2()
        {
            // New Map with Counter of value 5
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string counterName = "counter";

            var counterMapUpdate = new MapUpdate
            {
                counter_op = new CounterOp
                {
                    increment = 5
                },
                field = new MapField { name = Serializer.Invoke(counterName), type = MapField.MapFieldType.COUNTER }
            };

            // Store
            var updatedMap = Client.DtUpdateMap(id, Serializer, NoContext, NoRemovals,
                new List<MapUpdate> { counterMapUpdate });

            // Increment by 2
            var counterMapUpdate2 = new MapUpdate
            {
                counter_op = new CounterOp
                {
                    increment = 2
                },
                field = new MapField { name = Serializer.Invoke(counterName), type = MapField.MapFieldType.COUNTER }
            };

            // Remove field
            var fieldToRemove = new RiakDtMapField(counterMapUpdate.field);

            // Store
            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap.Context,
                new List<RiakDtMapField> { fieldToRemove }, new List<MapUpdate> { counterMapUpdate2 });

            Assert.AreEqual(1, updatedMap2.Values.Count);

            var counterField = updatedMap2.Values.Single(s => s.Field.Name == counterName);
            Assert.AreEqual(2, counterField.Counter.Value);
        }

        [Test]
        public void Test3()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);

            // New map w/ Nested Map w/ Nested Set; Add X,Y to set
            byte[] innerMapName = Serializer("innerMap");
            byte[] innerMapSetName = Serializer("innerMap_innerSet");
            byte[][] adds = { Serializer("X"), Serializer("Y") };

            var innerMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = innerMapSetName, type = MapField.MapFieldType.SET }
            };
            innerMapUpdate.set_op.adds.AddRange(adds);

            var parentMapUpdate = new MapUpdate
            {
                field = new MapField { name = innerMapName, type = MapField.MapFieldType.MAP },
                map_op = new MapOp()
            };

            parentMapUpdate.map_op.updates.Add(innerMapUpdate);

            var updatedMap = Client.DtUpdateMap(id, Serializer, NoContext, null, new List<MapUpdate> { parentMapUpdate });

            Assert.AreEqual(1, updatedMap.Values.Count);

            var innerMapField = updatedMap.Values.Single(s => s.Field.Name == innerMapName.FromRiakString());

            Assert.AreEqual(1, innerMapField.MapValue.Count);

            var innerMapSetField =
                innerMapField.MapValue.Single(entry => entry.Field.Name == innerMapSetName.FromRiakString());
            var setValues = innerMapSetField.SetValue.Select(v => Deserializer(v)).ToList();

            Assert.AreEqual(2, setValues.Count);
            Assert.Contains("X", setValues);
            Assert.Contains("Y", setValues);

            // Remove nested map, add Z to set
            var innerMapUpdate2 = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = innerMapSetName, type = MapField.MapFieldType.SET }
            };
            innerMapUpdate2.set_op.adds.Add(Serializer("Z"));

            var parentMapUpdate2 = new MapUpdate
            {
                field = new MapField { name = innerMapName, type = MapField.MapFieldType.MAP },
                map_op = new MapOp()
            };

            parentMapUpdate2.map_op.removes.Add(innerMapUpdate2.field);
            parentMapUpdate2.map_op.updates.Add(innerMapUpdate2);

            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap.Context, NoRemovals,
                new List<MapUpdate> { parentMapUpdate2 });

            Assert.AreEqual(1, updatedMap2.Values.Count);

            var innerMapField2 = updatedMap2.Values.Single(s => s.Field.Name == innerMapName.FromRiakString());

            Assert.AreEqual(1, innerMapField2.MapValue.Count);

            var innerMapSetField2 =
                innerMapField2.MapValue.Single(entry => entry.Field.Name == innerMapSetName.FromRiakString());
            var setValues2 = innerMapSetField2.SetValue.Select(v => Deserializer(v)).ToList();

            Assert.AreEqual(1, setValues2.Count);
            Assert.Contains("Z", setValues2);
        }


        [Test]
        public void Test4()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string setName = "set";

            // New Map with Set
            var setMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            // Add X, Y to set
            var addSet = new List<string> { "X", "Y" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet);

            // Store
            var updatedMap1 = Client.DtUpdateMap(id, Serializer, NoContext, NoRemovals,
                new List<MapUpdate> { setMapUpdate });

            // Add Z
            var setMapUpdate2 = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            var addSet2 = new List<string> { "Z" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate2.set_op.adds.AddRange(addSet2);

            // Remove Set
            var fieldToRemove = new RiakDtMapField(setMapUpdate.field);

            // Store again, no context
            Assert.Throws<ArgumentNullException>(() =>
                Client.DtUpdateMap(id, Serializer, NoContext,
                    new List<RiakDtMapField> { fieldToRemove },
                    new List<MapUpdate> { setMapUpdate2 }));
        }

        [Test]
        public void Test5()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string setName = "set";

            // New Map with Set
            var setMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            // Add X, Y to set
            var addSet = new List<string> { "X", "Y" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet);

            // Store
            var updatedMap1 = Client.DtUpdateMap(id, Serializer, NoContext, NoRemovals,
                new List<MapUpdate> { setMapUpdate });

            // Remove X from set
            var setMapUpdate2 = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            setMapUpdate2.set_op.removes.Add(Serializer("X"));

            // Store again, no context
            Assert.Throws<ArgumentNullException>(() =>
                Client.DtUpdateMap(id, Serializer, NoContext, NoRemovals, new List<MapUpdate> { setMapUpdate2 }));
        }
    }
}