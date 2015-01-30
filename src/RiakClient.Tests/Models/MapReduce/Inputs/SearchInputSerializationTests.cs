// <copyright file="SearchInputSerializationTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

using NUnit.Framework;
using RiakClient.Models.MapReduce.Inputs;

namespace RiakClient.Tests.Models.MapReduce.Inputs
{
    [TestFixture]
    public class SearchInputSerializationTests : MapReduceSerializationTestsBase
    {
        [Test]
        public void TestLegacySerialization()
        {
#pragma warning disable 618
            var input = new RiakBucketSearchInput("my_bucket", "my_query");
#pragma warning restore 618
            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"bucket\":\"my_bucket\",\"query\":\"my_query\"}", json);
        }

        [Test]
        public void TestYokozunaSerialization()
        {
            var input = new RiakSearchInput("my_index", "my_query");
            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"index\":\"my_index\",\"query\":\"my_query\"}", json);
        }
    }
}
