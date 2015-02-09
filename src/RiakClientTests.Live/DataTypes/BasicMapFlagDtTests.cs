using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Messages;
using RiakClient.Models;

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
    public class BasicMapFlagDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestFlagOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string flagName = "Name";

            var flagMapUpdate = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.DISABLE,
                field = new MapField { name = Serializer.Invoke(flagName), type = MapField.MapFieldType.FLAG }
            };

            var updatedMap1 = Client.DtUpdateMap(id, Serializer, null, null, new List<MapUpdate> { flagMapUpdate });

            Assert.True(updatedMap1.Result.IsSuccess, updatedMap1.Result.ErrorMessage);
            var mapEntry = updatedMap1.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsFalse(mapEntry.FlagValue.Value);

            var flagMapUpdate2 = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.ENABLE,
                field = new MapField { name = Serializer.Invoke(flagName), type = MapField.MapFieldType.FLAG }
            };

            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null,
                new List<MapUpdate> { flagMapUpdate2 });

            Assert.True(updatedMap2.Result.IsSuccess, updatedMap2.Result.ErrorMessage);
            mapEntry = updatedMap2.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsTrue(mapEntry.FlagValue.Value);
        }
    }
}