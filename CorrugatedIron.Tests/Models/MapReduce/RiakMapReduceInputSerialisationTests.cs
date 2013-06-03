// Copyright (c) 2013 - OJ Reeves & Jeremiah Peschka
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
using System.IO;
using System.Text;
using CorrugatedIron.Models.MapReduce.Inputs;
using NUnit.Framework;
using Newtonsoft.Json;

namespace CorrugatedIron.Tests.Models.MapReduce
{
    [TestFixture]
    public class RiakMapReduceInputSerialisationTests
    {
        private static string Serialize(Func<JsonWriter, JsonWriter> doWrite)
        {
            var sb = new StringBuilder();

            using(var sw = new StringWriter(sb))
            using(JsonWriter writer = new JsonTextWriter(sw))
            {
                doWrite(writer);
            }

            return sb.ToString();
        }

        [Test]
        public void RiakBucketKeyInputSeralisesCorrectly()
        {
            var input = new RiakBucketKeyInput()
                .Add("foo", "bar")
                .Add("foo", "baz")
                .Add("dooby", "scooby");

            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, "\"inputs\":[[\"foo\",\"bar\"],[\"foo\",\"baz\"],[\"dooby\",\"scooby\"]]");
        }

        [Test]
        public void RiakBucketKeyKeyDataInputSeralisesCorrectly()
        {
            var input = new RiakBucketKeyKeyDataInput()
                .Add("foo", "bar", "baz")
                .Add("foo", "baz", 130)
                .Add("dooby", "scooby", new { la = "ding", ray = "me", wit = 0 });

            var s = Serialize(input.WriteJson);

            Assert.AreEqual(s, "\"inputs\":[[\"foo\",\"bar\",\"baz\"],[\"foo\",\"baz\",130],[\"dooby\",\"scooby\",{\"la\":\"ding\",\"ray\":\"me\",\"wit\":0}]]");
        }
    }
}
