// <copyright file="BucketKeyInputSerializationTests.cs" company="Basho Technologies, Inc.">
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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using RiakClient.Models;
using RiakClient.Models.MapReduce.Inputs;

namespace RiakClientTests.Models.MapReduce.Inputs
{
    [TestFixture, UnitTest]
    public class BucketKeyInputSerializationTests : MapReduceSerializationTestsBase
    {
        private const string SerializedRiakBucketKeyInput =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"\"]," +
            "[\"foo\",\"baz\",\"\"]," +
            "[\"dooby\",\"scooby\",\"\"]," +
            "[\"foo\",\"baz\",\"\"]," +
            "[\"dooby\",\"scooby\",\"\"]]";

        private const string SerializedRiakBucketKeyInputWithTypes =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"qux_type\"]," +
            "[\"foo\",\"baz\",\"qux_type\"]," +
            "[\"dooby\",\"scooby\",\"dog_type\"]," +
            "[\"foo\",\"baz\",\"qux_type\"]," +
            "[\"dooby\",\"scooby\",\"dog_type\"]]";

        [Test]
        public void RiakBucketKeyInputSerializesCorrectlyOldInterface()
        {
            var inputList = new List<Tuple<string, string>>
            { 
                new Tuple<string, string>("foo", "baz"),
                new Tuple<string, string>("dooby", "scooby")
            };

#pragma warning disable 612, 618
            var input = new RiakBucketKeyInput()
                .Add("foo", "bar")
                .Add(inputList)
                .Add(inputList[0], inputList[1]);
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, SerializedRiakBucketKeyInput);
        }

        [Test]
        public void RiakBucketKeyInputSerializesCorrectly()
        {
            var inputList = new List<RiakObjectId>
            {
                new RiakObjectId("foo", "baz"),
                new RiakObjectId("dooby", "scooby")
            };

            var input = new RiakBucketKeyInput()
                .Add(new RiakObjectId("foo", "bar"))
                .Add(inputList)
                .Add(inputList[0], inputList[1]);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s, SerializedRiakBucketKeyInput);
        }

        [Test]
        public void RiakBucketKeyInputWithTypesSerializesCorrectly()
        {
            var inputList = new List<RiakObjectId>
            {
                new RiakObjectId("qux_type", "foo", "baz"),
                new RiakObjectId("dog_type", "dooby", "scooby")
            };

            var input = new RiakBucketKeyInput()
                .Add(new RiakObjectId("qux_type", "foo", "bar"))
                .Add(inputList)
                .Add(inputList[0], inputList[1]);

            var s = Serialize(input.WriteJson);
            Assert.AreEqual(s,
                SerializedRiakBucketKeyInputWithTypes);
        }

        [Test]
        public void FromRiakObjectIdsHelperMethodSerializesCorrectly()
        {
            var ids = new List<RiakObjectId>
            {
                new RiakObjectId("bazType", "foo", "bar"),
                new RiakObjectId("bazType", "foo", "baz"),
                new RiakObjectId("bazType", "dooby", "scooby")
            };

            var input = RiakBucketKeyInput.FromRiakObjectIds(ids);
            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s,
                "\"inputs\":[[\"foo\",\"bar\",\"bazType\"],[\"foo\",\"baz\",\"bazType\"],[\"dooby\",\"scooby\",\"bazType\"]]");
        }
    }
}

