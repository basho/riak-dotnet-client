// <copyright file="BasicMapFlagDtTests.cs" company="Basho Technologies, Inc.">
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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RiakClient;
using RiakClient.Messages;
using RiakClient.Models;

#pragma warning disable 618

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture, IntegrationTest]
    public class BasicMapFlagDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestFlagOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Maps, Bucket, key);
            const string flagName = "Name";
            byte[] flagNameBytes = Serializer(flagName);

            var options = new RiakDtUpdateOptions();
            options.SetIncludeContext(true);
            options.SetTimeout(new Timeout(TimeSpan.FromSeconds(60)));

            var flagMapUpdate = new MapUpdate
            {
                flag_op = MapUpdate.FlagOp.ENABLE,
                field = new MapField { name = flagNameBytes, type = MapField.MapFieldType.FLAG }
            };
            var update = new List<MapUpdate> { flagMapUpdate };
            var updatedMap1 = Client.DtUpdateMap(id, Serializer, null, null, update, options);

            Assert.True(updatedMap1.Result.IsSuccess, updatedMap1.Result.ErrorMessage);
            var mapEntry = updatedMap1.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsTrue(mapEntry.FlagValue.Value);

            flagMapUpdate.flag_op = MapUpdate.FlagOp.DISABLE;
            var updatedMap2 = Client.DtUpdateMap(id, Serializer, updatedMap1.Context, null, update, options);

            Assert.True(updatedMap2.Result.IsSuccess, updatedMap2.Result.ErrorMessage);
            mapEntry = updatedMap2.Values.Single(s => s.Field.Name == flagName);
            Assert.NotNull(mapEntry.FlagValue);
            Assert.IsFalse(mapEntry.FlagValue.Value);
        }
    }
}

#pragma warning restore 618