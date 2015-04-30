// <copyright file="BasicMapSetDtTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Messages;
using RiakClient.Models;

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
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
