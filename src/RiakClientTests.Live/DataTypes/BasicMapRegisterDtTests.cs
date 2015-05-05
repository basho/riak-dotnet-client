// <copyright file="BasicMapRegisterDtTests.cs" company="Basho Technologies, Inc.">
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient.Messages;
using RiakClient.Models;

#pragma warning disable 618

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

#pragma warning restore 618