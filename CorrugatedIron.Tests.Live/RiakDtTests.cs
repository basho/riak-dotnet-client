// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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
using System.Security.Cryptography;
using System.Text;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{

    [TestFixture]
    public class RiakDtTests : LiveRiakConnectionTestBase
    {
        private const string SetBucketType = "sets";
        private const string CounterBucketType = "counters";
        private const string MapBucketType = "maps";
        private const string Bucket = "riak_dt_bucket";
        private readonly DeserializeObject<string> _decoder = (b, type) => Encoding.UTF8.GetString(b);
        private readonly SerializeObjectToByteArray<string> _encoder = s => Encoding.UTF8.GetBytes(s);
        private readonly Random _random = new Random();
        
        [Test]
        public void TestSetOperations()
        {
            var updateOptions = new RiakDtUpdateOptions().SetIncludeContext(true).SetReturnBody(true);
            var key = "TestSetOperations_" + _random.Next();
            Console.WriteLine("Using {0} for TestSetOperations() key", key);
            
            var id = new RiakObjectId(SetBucketType, Bucket, key);
            var initialSet = Client.DtFetchSet(id, new RiakDtFetchOptions().SetIncludeContext(true));

            Assert.IsNull(initialSet.Context);
            Assert.IsEmpty(initialSet.Values);
             
            // Single Add
            var add = new List<string> { "foo" };
            var updatedSet1 = Client.DtUpdateSet(id, _encoder, initialSet.Context, add, null, updateOptions);
            var valuesAsStrings1 = updatedSet1.GetObjects(_decoder).ToList();

            Assert.AreEqual(1, updatedSet1.Values.Count);
            Assert.Contains("foo", valuesAsStrings1);

            // Many Add
            var manyAdds = new List<string> { "foo", "bar", "baz", "qux" };
            var updatedSet2 = Client.DtUpdateSet(id, _encoder, initialSet.Context, manyAdds, null, updateOptions);
            var valuesAsStrings2 = updatedSet2.GetObjects(_decoder).ToList();

            Assert.AreEqual(4, updatedSet2.Values.Count);
            Assert.Contains("foo", valuesAsStrings2);
            Assert.Contains("bar", valuesAsStrings2);
            Assert.Contains("baz", valuesAsStrings2);
            Assert.Contains("qux", valuesAsStrings2);

            // Single Remove
            var remove = new List<string> { "baz" };
            var updatedSet3 = Client.DtUpdateSet(id, _encoder, initialSet.Context, null, remove, updateOptions);
            var valuesAsStrings3 = updatedSet3.GetObjects(_decoder).ToList();

            Assert.AreEqual(3, updatedSet3.Values.Count);
            Assert.Contains("foo", valuesAsStrings3);
            Assert.Contains("bar", valuesAsStrings3);
            Assert.Contains("qux", valuesAsStrings3);

            // Many Remove
            var manyRemove = new List<string> { "foo", "bar", "qux" };
            var updatedSet4 = Client.DtUpdateSet(id, _encoder, initialSet.Context, null, manyRemove, updateOptions);

            Assert.AreEqual(0, updatedSet4.Values.Count);
        }

        [Test]
        [Ignore("Not Implemented")]
        public void TestCounterOperations()
        {

        }

        [Test]
        [Ignore("Not Implemented")]
        public void TestMapOperations()
        {

        }

        /// <summary>
        /// The tearing of the down, it is done here.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
    }
}
