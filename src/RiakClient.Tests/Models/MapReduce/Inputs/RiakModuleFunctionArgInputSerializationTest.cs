// Copyright (c) 2011 - 2014 OJ Reeves & Jeremiah Peschka
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

using NUnit.Framework;
using RiakClient.Models.MapReduce.Inputs;

namespace RiakClient.Tests.Models.MapReduce.Inputs
{
    [TestFixture]
    public class RiakModuleFunctionArgInputSerializationTest : MapReduceSerializationTestsBase
    {
        [Test]
        public void EnsureMFAInputSeralizesCorrectly()
        {
            var input = new RiakModuleFunctionArgInput("my_mod", "my_fun", new[] { "arg1", "arg2", "arg3" });

            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"module\":\"my_mod\",\"function\":\"my_fun\",\"arg\":[\"arg1\",\"arg2\",\"arg3\"]}", json);
        }

        [Test]
        public void EnsureMFAInputWithNoArgsSeralizesCorrectly()
        {
            var input = new RiakModuleFunctionArgInput("my_mod", "my_fun", new string[]{});

            var json = Serialize(input.WriteJson);
            Assert.AreEqual("\"inputs\":{\"module\":\"my_mod\",\"function\":\"my_fun\",\"arg\":[]}", json);
        }
    }
}

