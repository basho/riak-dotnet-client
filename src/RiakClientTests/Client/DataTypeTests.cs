// <copyright file="DataTypeTests.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RiakClient.Extensions;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture]
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