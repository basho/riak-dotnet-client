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
    using System.Diagnostics;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Util;

    /*
     * http://docs.basho.com/riak/latest/dev/using/basics/
     * 
     * Note: You must create the following bucket types before running:
     * riak-admin bucket-type create animals; riak-admin bucket-type activate animals
     * riak-admin bucket-type create quotes; riak-admin bucket-type activate quotes
     * riak-admin bucket-type create sports; riak-admin bucket-type activate sports
     * riak-admin bucket-type create cars; riak-admin bucket-type activate cars
     * riak-admin bucket-type create users; riak-admin bucket-type activate users
     */
    public sealed class DevUsingBasicsExamples : IDisposable
    {
        private readonly IRiakEndPoint endpoint;
        private IRiakClient client;
        private RiakObjectId id;

        public DevUsingBasicsExamples()
        {
            endpoint = RiakCluster.FromConfig("riakConfig");
        }

        [SetUp]
        public void CreateClient()
        {
            client = endpoint.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            if (id != null)
            {
                var rslt = client.Delete(id);
                Assert.IsTrue(rslt.IsSuccess);
            }
        }

        [Test]
        public void Example_ReadingObjects()
        {
            id = new RiakObjectId("animals", "dogs", "rufus");
            Assert.AreEqual("animals", id.BucketType);
            Assert.AreEqual("dogs", id.Bucket);
            Assert.AreEqual("rufus", id.Key);

            var rslt = client.Get(id);
            Assert.IsFalse(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
            Assert.AreEqual(ResultCode.NotFound, rslt.ResultCode);
        }

        [Test]
        public void Example_WritingObjects()
        {
            id = PutRufus();
        }

        [Test]
        public void Example_ContentTypes()
        {
            id = new RiakObjectId("animals", "dogs", "fido");
            var fido = new { breed = "dalmatian", size = "large" };
            var obj = new RiakObject(id, fido);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);

            rslt = client.Get(id);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
            Assert.NotNull(rslt.Value);
            Assert.AreEqual(RiakConstants.ContentTypes.ApplicationJson, rslt.Value.ContentType);

            string json = Encoding.UTF8.GetString(rslt.Value.Value);
            Assert.AreEqual(@"{""breed"":""dalmatian"",""size"":""large""}", json);
        }

        [Test]
        public void Example_ReadParameters()
        {
            RiakObjectId id = PutRufus();

            var opts = new RiakGetOptions();
            opts.SetR(3);
            var rslt = client.Get(id, opts);
            Debug.WriteLine(Encoding.UTF8.GetString(rslt.Value.Value));
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
        }

        [Test]
        public void Example_StoringAndSpecifyingContentType()
        {
            id = new RiakObjectId("quotes", "oscar_wilde", "genius");
            var obj = new RiakObject(
                id,
                "I have nothing to declare but my genius",
                RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
        }

        [Test]
        public void Example_CausalContext()
        {
            id = new RiakObjectId("sports", "nba", "champion");
            var obj = new RiakObject(id, "Washington Generals",
                RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);

            rslt = client.Get(id);
            Assert.IsTrue(rslt.IsSuccess);
            obj = rslt.Value;
            obj.SetObject("Harlem Globetrotters", RiakConstants.ContentTypes.TextPlain);
            rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
            Assert.IsTrue(EnumerableUtil.IsNullOrEmpty(rslt.Value.Siblings));
            Assert.AreEqual("Harlem Globetrotters", Encoding.UTF8.GetString(rslt.Value.Value));
        }

        private RiakObjectId PutRufus()
        {
            id = new RiakObjectId("animals", "dogs", "rufus");
            var obj = new RiakObject(id, "WOOF!", RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            Assert.IsTrue(rslt.IsSuccess, "Error: {0}", rslt.ErrorMessage);
            return id;
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
