using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Messages;
using RiakClient.Models;

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
    public class BasicMapRegisterDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestRegisterOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string registerName = "Name";

            var registerMapUpdate = new MapUpdate
            {
                register_op = Serializer.Invoke("Alex"),
                field = new MapField { name = Serializer.Invoke(registerName), type = MapField.MapFieldType.REGISTER }
            };

            var updatedMap1 = Client.DtUpdateMap(id, Serializer, null, null, new List<MapUpdate> { registerMapUpdate });
            Assert.AreEqual("Alex",
                Deserializer.Invoke(updatedMap1.Values.Single(s => s.Field.Name == registerName).RegisterValue));

            registerMapUpdate.register_op = Serializer.Invoke("Luke");
            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null,
                new List<MapUpdate> { registerMapUpdate });
            Assert.AreEqual("Luke",
                Deserializer.Invoke(updatedMap2.Values.Single(s => s.Field.Name == registerName).RegisterValue));
        }
    }
}