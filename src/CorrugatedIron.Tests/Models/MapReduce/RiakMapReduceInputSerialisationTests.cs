// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
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
using System.IO;
using System.Text;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce.Inputs;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Models.MapReduce
{
    [TestFixture]
    public class RiakMapReduceInputSerialisationTests
    {
        private const string SerializedRiakBucketKeyKeyDataInput =
            "\"inputs\":[" +
            "[\"foo\",\"bar\",\"baz\"]," +
            "[\"foo\",\"baz\",130]," +
            "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0}]," +
            "[\"foo\",\"baz\",130]," +
            "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0}]]";

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

        private static string Serialize(Func<JsonWriter, JsonWriter> doWrite)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                doWrite(writer);
            }

            return sb.ToString();
        }

        [Test]
        public void RiakBucketKeyInputSeralisesCorrectlyOldInterface()
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
        public void RiakBucketKeyInputSeralisesCorrectly()
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
        public void RiakBucketKeyInputWithTypesSeralisesCorrectly()
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
        public void FromRiakObjectIdsHelperMethodSeralisesCorrectly()
        {
            var ids = new List<RiakObjectId> { 
                new RiakObjectId("bazType", "foo", "bar"),
                new RiakObjectId("bazType", "foo", "baz"),
                new RiakObjectId("bazType", "dooby", "scooby")};

            var input = RiakBucketKeyInput.FromRiakObjectIds(ids);
            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s,
                "\"inputs\":[[\"foo\",\"bar\",\"bazType\"],[\"foo\",\"baz\",\"bazType\"],[\"dooby\",\"scooby\",\"bazType\"]]");
        }

        [Test]
        public void RiakBucketKeyKeyDataInputSeralisesCorrectlyOldInterface()
        {
            var inputs = new List<Tuple<string, string, object>>
            {
                new Tuple<string, string, object>("foo", "baz", 130),
                new Tuple<string, string, object>("dooby", "scooby", new {la = "ding", ray = "me", wit = 0})
            };

#pragma warning disable 612, 618
            var input = new RiakBucketKeyKeyDataInput()
                .Add("foo", "bar", "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder
#pragma warning restore 612, 618

            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, SerializedRiakBucketKeyKeyDataInput);
        }

        [Test]
        public void RiakBucketKeyKeyDataInputSeralisesCorrectly()
        {
            var inputs = new List<Tuple<RiakObjectId, object>>
            {
                new Tuple<RiakObjectId, object>(new RiakObjectId("foo", "baz"), 130),
                new Tuple<RiakObjectId, object>(new RiakObjectId("dooby", "scooby"),
                    new {la = "ding", ray = "me", wit = 0})
            };

            var input = new RiakBucketKeyKeyDataInput()
                .Add(new RiakObjectId("foo", "bar"), "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder


            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, SerializedRiakBucketKeyKeyDataInput);
        }

        [Test]
        public void RiakBucketKeyKeyDataInputWithTypeSeralisesCorrectly()
        {
            var inputs = new List<Tuple<RiakObjectId, object>>
            {
                new Tuple<RiakObjectId, object>(new RiakObjectId("qux_type", "foo", "baz"), 130),
                new Tuple<RiakObjectId, object>(new RiakObjectId("dog_type", "dooby", "scooby"),
                    new {la = "ding", ray = "me", wit = 0})
            };

            var input = new RiakBucketKeyKeyDataInput()
                .Add(new RiakObjectId("qux_type", "foo", "bar"), "")
                .Add(new RiakObjectId("qux_type", "foo", "bar"), "baz")
                .Add(inputs) // IEnumerable based builder
                .Add(inputs[0], inputs[1]); // Params based builder


            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s,
                "\"inputs\":[" +
                "[\"foo\",\"bar\",\"\",\"qux_type\"]," +
                "[\"foo\",\"bar\",\"baz\",\"qux_type\"]," +
                "[\"foo\",\"baz\",130,\"qux_type\"]," +
                "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0},\"dog_type\"]," +
                "[\"foo\",\"baz\",130,\"qux_type\"]," +
                "[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0},\"dog_type\"]]");
        }
    }
}