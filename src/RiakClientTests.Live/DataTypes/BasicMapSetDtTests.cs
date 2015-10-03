#pragma warning disable 618

namespace RiakClientTests.Live.DataTypes
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture, IntegrationTest]
    public class BasicMapSetDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestMapSetOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string setName = "Name";

            var setMapUpdate = new MapUpdate
            {
                set_op = new SetOp(),
                field = new MapField { name = Serializer.Invoke(setName), type = MapField.MapFieldType.SET }
            };

            var addSet = new List<string> { "Alex" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet);

            var updatedMap1 = Client.DtUpdateMap(id, Serializer, null, null, new List<MapUpdate> { setMapUpdate });
            var setValues1 =
                updatedMap1.Values.Single(s => s.Field.Name == setName)
                    .SetValue.Select(v => Deserializer.Invoke(v))
                    .ToList();
            Assert.Contains("Alex", setValues1);
            Assert.AreEqual(1, setValues1.Count);

            setMapUpdate.set_op = new SetOp();
            var removeSet = addSet;
            var addSet2 = new List<string> { "Luke", "Jeremiah" }.Select(s => Serializer.Invoke(s)).ToList();
            setMapUpdate.set_op.adds.AddRange(addSet2);
            setMapUpdate.set_op.removes.AddRange(removeSet);

            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null,
                new List<MapUpdate> { setMapUpdate });
            var setValues2 =
                updatedMap2.Values.Single(s => s.Field.Name == setName)
                    .SetValue.Select(v => Deserializer.Invoke(v))
                    .ToList();
            Assert.Contains("Luke", setValues2);
            Assert.Contains("Jeremiah", setValues2);
            Assert.AreEqual(2, setValues2.Count);
        }
    }
}

#pragma warning restore 618
