// <copyright file="Updates.cs" company="Basho Technologies, Inc.">
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
// </copyright>

namespace RiakClientExamples.Dev.Using
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    /*
     * http://docs.basho.com/riak/latest/dev/using/updates/
     */
    public sealed class Updates : ExampleBase
    {
        [Test]
        public void ReadingObjects()
        {
            id = new RiakObjectId("animals", "dogs", "rufus");
            Assert.AreEqual("animals", id.BucketType);
            Assert.AreEqual("dogs", id.Bucket);
            Assert.AreEqual("rufus", id.Key);

            rslt = client.Get(id);
            Assert.AreEqual(ResultCode.NotFound, rslt.ResultCode);
        }
    }
}
