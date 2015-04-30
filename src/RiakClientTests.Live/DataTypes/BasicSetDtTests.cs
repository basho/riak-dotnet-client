// <copyright file="BasicSetDtTests.cs" company="Basho Technologies, Inc.">
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
using RiakClient.Models;

namespace RiakClientTests.Live.DataTypes
{
    [TestFixture]
    public class BasicSetDtTests : DataTypeTestsBase
    {
        [Test]
        public void TestSetOperations()
        {
            string key = GetRandomKey();

            var id = new RiakObjectId(BucketTypeNames.Sets, Bucket, key);
            var initialSet = Client.DtFetchSet(id);

            Assert.IsNull(initialSet.Context);
            Assert.IsEmpty(initialSet.Values);

            // Single Add
            var add = new List<string> { "foo" };
            var updatedSet1 = Client.DtUpdateSet(id, Serializer, initialSet.Context, add, null);
            var valuesAsStrings1 = updatedSet1.GetObjects(Deserializer).ToList();

            Assert.AreEqual(1, updatedSet1.Values.Count);
            Assert.Contains("foo", valuesAsStrings1);

            // Many Add
            var manyAdds = new List<string> { "foo", "bar", "baz", "qux" };
            var updatedSet2 = Client.DtUpdateSet(id, Serializer, updatedSet1.Context, manyAdds, null);
            var valuesAsStrings2 = updatedSet2.GetObjects(Deserializer).ToList();

            Assert.AreEqual(4, updatedSet2.Values.Count);
            Assert.Contains("foo", valuesAsStrings2);
            Assert.Contains("bar", valuesAsStrings2);
            Assert.Contains("baz", valuesAsStrings2);
            Assert.Contains("qux", valuesAsStrings2);

            // Single Remove
            var remove = new List<string> { "baz" };
            var updatedSet3 = Client.DtUpdateSet(id, Serializer, updatedSet2.Context, null, remove);
            var valuesAsStrings3 = updatedSet3.GetObjects(Deserializer).ToList();

            Assert.AreEqual(3, updatedSet3.Values.Count);
            Assert.Contains("foo", valuesAsStrings3);
            Assert.Contains("bar", valuesAsStrings3);
            Assert.Contains("qux", valuesAsStrings3);

            // Many Remove
            var manyRemove = new List<string> { "foo", "bar", "qux" };
            var updatedSet4 = Client.DtUpdateSet(id, Serializer, updatedSet3.Context, null, manyRemove);

            Assert.AreEqual(0, updatedSet4.Values.Count);
        }
    }
}
