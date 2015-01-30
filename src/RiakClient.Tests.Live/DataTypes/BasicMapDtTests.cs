using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Messages;
using RiakClient.Models;
using RiakClient.Models.RiakDt;

namespace RiakClient.Tests.Live.DataTypes
{
    [TestFixture]
    public class BasicMapDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestBasicMapOperations()
        {
            string key = GetRandomKey();

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
                field = new MapField { name = Serializer.Invoke(counterName), type = MapField.MapFieldType.COUNTER }
            };

            var updatedMap1 = Client.DtUpdateMap(id, Serializer, initialMap.Context, null,
                new List<MapUpdate> { counterMapUpdate });

            Assert.IsNotNull(updatedMap1.Context);
            Assert.IsNotEmpty(updatedMap1.Values);
            Assert.AreEqual(1, updatedMap1.Values.Single(s => s.Field.Name == counterName).Counter.Value);


            // Increment Counter
            // [ Score => 5 ]
            counterMapUpdate.counter_op.increment = 4;
            RiakDtMapResult updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null,
                new List<MapUpdate> { counterMapUpdate });
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
                field =
                    new MapField { name = Serializer.Invoke(innerMapCounterName), type = MapField.MapFieldType.COUNTER }
            };

            var parentMapUpdate = new MapUpdate
            {
                field = new MapField { name = Serializer.Invoke(innerMapName), type = MapField.MapFieldType.MAP },
                map_op = new MapOp()
            };

            parentMapUpdate.map_op.updates.Add(innerCounterMapUpdate);
            counterMapUpdate.counter_op.increment = -10;

            var updatedMap3 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null,
                new List<MapUpdate> { parentMapUpdate, counterMapUpdate });

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
            var updatedMap4 = Client.DtUpdateMap(id, Serializer, updatedMap3.Context, removes);

            innerMapField = updatedMap4.Values.Single(entry => entry.Field.Name == innerMapName);
            innerMapCounterField = innerMapField.MapValue.Single(entry => entry.Field.Name == innerMapCounterName);
            Assert.AreEqual(1, updatedMap4.Values.Count);
            Assert.AreEqual(42, innerMapCounterField.Counter.Value);
        }
    }
}