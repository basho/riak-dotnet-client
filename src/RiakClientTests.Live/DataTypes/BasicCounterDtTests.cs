// <copyright file="BasicCounterDtTests.cs" company="Basho Technologies, Inc.">
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

using NUnit.Framework;
using RiakClient.Models;

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
    public class BasicCounterDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestCounterOperations()
        {
            string key = GetRandomKey();

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

    }
}