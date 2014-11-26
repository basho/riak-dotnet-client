// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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
using System.Text;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Models.RiakDt;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{

    [TestFixture]
    public class RawRiakDtTests : LiveRiakConnectionTestBase
    {
        private const string Bucket = "riak_dt_bucket";
        private readonly DeserializeObject<string> _deserializer = (b, type) => Encoding.UTF8.GetString(b);
        private readonly SerializeObjectToByteArray<string> _serializer = s => Encoding.UTF8.GetBytes(s);
        private readonly Random _random = new Random();

        [Test]
        public void TestSetOperations()
        {
            var key = "TestSetOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestSetOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Sets, Bucket, key);
            var initialSet = Client.DtFetchSet(id);

            Assert.IsNull(initialSet.Context);
            Assert.IsEmpty(initialSet.Values);

            // Single Add
            var add = new List<string> { "foo" };
            var updatedSet1 = Client.DtUpdateSet(id, _serializer, initialSet.Context, add, null);
            var valuesAsStrings1 = updatedSet1.GetObjects(_deserializer).ToList();

            Assert.AreEqual(1, updatedSet1.Values.Count);
            Assert.Contains("foo", valuesAsStrings1);

            // Many Add
            var manyAdds = new List<string> { "foo", "bar", "baz", "qux" };
            var updatedSet2 = Client.DtUpdateSet(id, _serializer, updatedSet1.Context, manyAdds, null);
            var valuesAsStrings2 = updatedSet2.GetObjects(_deserializer).ToList();

            Assert.AreEqual(4, updatedSet2.Values.Count);
            Assert.Contains("foo", valuesAsStrings2);
            Assert.Contains("bar", valuesAsStrings2);
            Assert.Contains("baz", valuesAsStrings2);
            Assert.Contains("qux", valuesAsStrings2);

            // Single Remove
            var remove = new List<string> { "baz" };
            var updatedSet3 = Client.DtUpdateSet(id, _serializer, updatedSet2.Context, null, remove);
            var valuesAsStrings3 = updatedSet3.GetObjects(_deserializer).ToList();

            Assert.AreEqual(3, updatedSet3.Values.Count);
            Assert.Contains("foo", valuesAsStrings3);
            Assert.Contains("bar", valuesAsStrings3);
            Assert.Contains("qux", valuesAsStrings3);

            // Many Remove
            var manyRemove = new List<string> { "foo", "bar", "qux" };
            var updatedSet4 = Client.DtUpdateSet(id, _serializer, updatedSet3.Context, null, manyRemove);

            Assert.AreEqual(0, updatedSet4.Values.Count);
        }

        [Test]
        public void TestCounterOperations()
        {
            var key = "TestCounterOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestCounterOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Counters, Bucket, key);

            // Fetch empty
            var initialCounter = Client.DtFetchCounter(id);

            Assert.IsFalse(initialCounter.Value.HasValue);

            // Increment one
            var updatedCounter1 = Client.DtUpdateCounter(id, 1);
            Assert.AreEqual(1, updatedCounter1.Value);

            // Increment many
            var updatedCounter2 = Client.DtUpdateCounter(id, 4);
            Assert.AreEqual(5, updatedCounter2.Value);

            // Fetch non-empty counter
            var incrementedCounter = Client.DtFetchCounter(id);
            Assert.AreEqual(5, incrementedCounter.Value);

            // Decrement one
            var updatedCounter3 = Client.DtUpdateCounter(id, -1);
            Assert.AreEqual(4, updatedCounter3.Value);

            // Decrement many
            var updatedCounter4 = Client.DtUpdateCounter(id, -4);
            Assert.AreEqual(0, updatedCounter4.Value);
        }

        [Test]
        public void TestBasicMapOperations()
        {
            var key = "TestMapOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestMapOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);

            // Fetch empty
            // []
            var initialMap = Client.DtFetchMap(id);
            Assert.IsNull(initialMap.Context);
            Assert.IsEmpty(initialMap.Values);

            // Add Counter
            // [ Score => 1 ]
            const string counterName = "Score";

            var counterMapUpdate = new MapUpdate
            {
                counter_op = new CounterOp { increment = 1 },
                field = new MapField { name = _serializer.Invoke(counterName), type = MapField.MapFieldType.COUNTER }
            };

            var updatedMap1 = Client.DtUpdateMap(id, _serializer, initialMap.Context, null, new List<MapUpdate> { counterMapUpdate });

            Assert.IsNotNull(updatedMap1.Context);
            Assert.IsNotEmpty(updatedMap1.Values);
            Assert.AreEqual(1, updatedMap1.Values.Single(s => s.Field.Name == counterName).Counter.Value);


            // Increment Counter
            // [ Score => 5 ]
            counterMapUpdate.counter_op.increment = 4;
            RiakDtMapResult updatedMap2 = Client.DtUpdateMap(id, _serializer, updatedMap1.Context, null, new List<MapUpdate> { counterMapUpdate });
            var counterMapField = updatedMap2.Values.Single(s => s.Field.Name == counterName);
            Assert.AreEqual(5, counterMapField.Counter.Value);


            // Add an embedded map with new counter
            // decrement "Score" counter by 10
            // [ Score => -5, subMap [ InnerScore => 42 ]]
            const string innerMapName = "subMap";
            const string innerMapCounterName = "InnerScore";
            var innerCounterMapUpdate = new MapUpdate
            {
                counter_op = new CounterOp { increment = 42 },
                field = new MapField { name = _serializer.Invoke(innerMapCounterName), type = MapField.MapFieldType.COUNTER }
            };

            var parentMapUpdate = new MapUpdate
            {
                field = new MapField { name = _serializer.Invoke(innerMapName), type = MapField.MapFieldType.MAP },
                map_op = new MapOp()
            };

            parentMapUpdate.map_op.updates.Add(innerCounterMapUpdate);
            counterMapUpdate.counter_op.increment = -10;

            var updatedMap3 = Client.DtUpdateMap(id, _serializer, updatedMap1.Context, null, new List<MapUpdate> { parentMapUpdate, counterMapUpdate });

            counterMapField = updatedMap3.Values.Single(entry => entry.Field.Name == counterName);
            var innerMapField = updatedMap3.Values.Single(entry => entry.Field.Name == innerMapName);
            var innerMapCounterField = innerMapField.MapValue.Single(entry => entry.Field.Name == innerMapCounterName);

            Assert.AreEqual(-5, counterMapField.Counter.Value);
            Assert.AreEqual(42, innerMapCounterField.Counter.Value);
            Assert.AreEqual(2, updatedMap3.Values.Count);
            Assert.AreEqual(1, innerMapField.MapValue.Count);


            // Remove Counter from map
            // [ subMap [ InnerScore => 42 ]]
            var removes = new List<RiakDtMapField> { new RiakDtMapField(counterMapField.Field.ToMapField()) };
            var updatedMap4 = Client.DtUpdateMap(id, _serializer, updatedMap3.Context, removes);

            innerMapField = updatedMap4.Values.Single(entry => entry.Field.Name == innerMapName);
            innerMapCounterField = innerMapField.MapValue.Single(entry => entry.Field.Name == innerMapCounterName);
            Assert.AreEqual(1, updatedMap4.Values.Count);
            Assert.AreEqual(42, innerMapCounterField.Counter.Value);
        }

        [Test]
        public void TestRegisterOperations()
        {
            var key = "TestRegisterOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestRegisterOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string registerName = "Name";

            var registerMapUpdate = new MapUpdate
            {
                register_op = _serializer.Invoke("Alex"),
                field = new MapField { name = _serializer.Invoke(registerName), type = MapField.MapFieldType.REGISTER }
            };

            var updatedMap1 = Client.DtUpdateMap(id, _serializer, null, null, new List<MapUpdate> { registerMapUpdate });
            Assert.AreEqual("Alex", _deserializer.Invoke(updatedMap1.Values.Single(s => s.Field.Name == registerName).RegisterValue));

            registerMapUpdate.register_op = _serializer.Invoke("Luke");
            var updatedMap2 = Client.DtUpdateMap(id, _serializer, updatedMap1.Context, null, new List<MapUpdate> { registerMapUpdate });
            Assert.AreEqual("Luke", _deserializer.Invoke(updatedMap2.Values.Single(s => s.Field.Name == registerName).RegisterValue));
        }

        [Test]
        public void TestFlagOperations()
        {
            var key = "TestFlagOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestFlagOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string flagName = "Name";

            var flagMapUpdate = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField { name = _serializer.Invoke(flagName), type = MapField.MapFieldType.FLAG }
            };

            var updatedMap1 = Client.DtUpdateMap(id, _serializer, null, null, new List<MapUpdate> { flagMapUpdate });
            
            Assert.True(updatedMap1.Result.IsSuccess, updatedMap1.Result.ErrorMessage);
            var mapEntry = updatedMap1.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsFalse(mapEntry.FlagValue.Value);

            var flagMapUpdate2 = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.ENABLE,
                field = new MapField { name = _serializer.Invoke(flagName), type = MapField.MapFieldType.FLAG }
            };

            var updatedMap2 = Client.DtUpdateMap(id, _serializer, updatedMap1.Context, null, new List<MapUpdate> { flagMapUpdate2 });

            Assert.True(updatedMap2.Result.IsSuccess, updatedMap2.Result.ErrorMessage);
            mapEntry = updatedMap2.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsTrue(mapEntry.FlagValue.Value);
        }

        [Test]
        public void TestMapSetOperations()
        {
            var key = "TestMapSetOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestMapSetOperations() key", key);

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string setName = "Name";

            var setMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField {name = _serializer.Invoke(setName), type = MapField.MapFieldType.SET}
            };
            var addSet = new List<string> {"Alex"}.Select(s => _serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet);

            var updatedMap1 = Client.DtUpdateMap(id, _serializer, null, null, new List<MapUpdate> {setMapUpdate});
            var setValues1 = updatedMap1.Values.Single(s => s.Field.Name == setName).SetValue.Select(v => _deserializer.Invoke(v)).ToList();
            Assert.Contains("Alex", setValues1);
            Assert.AreEqual(1, setValues1.Count);

            setMapUpdate.set_op = new SetOp();
            var removeSet = addSet;
            var addSet2 = new List<string> { "Luke", "Jeremiah" }.Select(s => _serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet2);
            setMapUpdate.set_op.removes.AddRange(removeSet);

            var updatedMap2 = Client.DtUpdateMap(id, _serializer, updatedMap1.Context, null, new List<MapUpdate> { setMapUpdate });
            var setValues2 = updatedMap2.Values.Single(s => s.Field.Name == setName).SetValue.Select(v => _deserializer.Invoke(v)).ToList();
            Assert.Contains("Luke", setValues2);
            Assert.Contains("Jeremiah", setValues2);
            Assert.AreEqual(2, setValues2.Count);

        }

        /// <summary>
        /// The tearing of the down, it is done here.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
    }
}
