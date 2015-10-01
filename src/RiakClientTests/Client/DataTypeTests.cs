#pragma warning disable 618

namespace RiakClientTests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RiakClient.Extensions;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture, UnitTest]
    public class DataTypeTests : ClientTestBase
    {
        private readonly SerializeObjectToByteArray<string> Serializer = s => Encoding.UTF8.GetBytes(s);

        [Test]
        public void DtUpdateMapWithRecursiveDataWithoutContext_ThrowsException()
        {
            var nestedRemoves = new List<MapField>
            {
                new MapField { name = "field_name".ToRiakString(), type = MapField.MapFieldType.SET }
            };

            var mapUpdateNested = new MapUpdate { map_op = new MapOp() };
            mapUpdateNested.map_op.removes.AddRange(nestedRemoves);

            var map_op = new MapOp();
            map_op.updates.Add(mapUpdateNested);

            var mapUpdate = new MapUpdate { map_op = new MapOp() };
            mapUpdate.map_op.updates.Add(mapUpdateNested);

            var updates = new List<MapUpdate> { mapUpdate };

            Assert.Throws<ArgumentNullException>(
                () => Client.DtUpdateMap(
                        "bucketType", "bucket", "key", Serializer, (byte[])null, null, updates, null)
            );
        }
    }
}

#pragma warning restore 618
