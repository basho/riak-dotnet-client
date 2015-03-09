// <copyright file="DevUsingBasicsExamples.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    // http://docs.basho.com/riak/latest/dev/using/basics/
    public class DevUsingBasicsExamples : IDisposable
    {
        private readonly IRiakEndPoint endpoint;
        private IRiakClient client;

        public DevUsingBasicsExamples()
        {
            endpoint = RiakCluster.FromConfig("riakConfig");
        }

        [TestFixtureSetUp]
        public void CreateClient()
        {
            client = endpoint.CreateClient();
        }

        [Test]
        public void Example1_ReadingObjects()
        {
            var id = new RiakObjectId("animals", "dogs", "rufus");
            Assert.AreEqual("animals", id.BucketType);
            Assert.AreEqual("dogs", id.Bucket);
            Assert.AreEqual("rufus", id.Key);

            var rslt = client.Get(id);
            Assert.IsFalse(rslt.IsSuccess);
            Assert.AreEqual(ResultCode.NotFound, rslt.ResultCode);
        }

        [Test]
        public void Example2_WritingObjects()
        {
            var id = new RiakObjectId("animals", "dogs", "rufus");
            var obj = new RiakObject(id, "WOOF!", RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess);

            DeleteObject(id);
        }

        [Test]
        public void Example3_ContentTypes()
        {
            var id = new RiakObjectId("animals", "dogs", "fido");
            var fido = new { breed = "dalmatian", size = "large" };
            var obj = new RiakObject(id, fido);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess);

            rslt = client.Get(id);
            Assert.IsTrue(rslt.IsSuccess);
            Assert.NotNull(rslt.Value);
            Assert.AreEqual(RiakConstants.ContentTypes.ApplicationJson, rslt.Value.ContentType);

            string json = Encoding.UTF8.GetString(rslt.Value.Value);
            Assert.AreEqual(@"{""breed"":""dalmatian"",""size"":""large""}", json);

            DeleteObject(id);
        }

        private void DeleteObject(RiakObjectId id)
        {
            IRiakClient client = endpoint.CreateClient();
            client.Delete(id);
        }

        public void Dispose()
        {
            if (endpoint != null)
            {
                endpoint.Dispose();
            }
        }
    }
}
