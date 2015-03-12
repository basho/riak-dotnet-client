// <copyright file="Basics.cs" company="Basho Technologies, Inc.">
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
    public sealed class Basics : ExampleBase
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

        [Test]
        public void WritingObjects()
        {
            id = PutRufus();
        }

        [Test]
        public void ContentTypes()
        {
            id = new RiakObjectId("animals", "dogs", "fido");
            var fido = new { breed = "dalmatian", size = "large" };
            var obj = new RiakObject(id, fido);
            var rslt = client.Put(obj);
            CheckResult(rslt);

            rslt = client.Get(id);
            CheckResult(rslt);
            Assert.NotNull(rslt.Value);
            Assert.AreEqual(RiakConstants.ContentTypes.ApplicationJson, rslt.Value.ContentType);

            string json = Encoding.UTF8.GetString(rslt.Value.Value);
            Assert.AreEqual(@"{""breed"":""dalmatian"",""size"":""large""}", json);
        }

        [Test]
        public void ReadParameters()
        {
            RiakObjectId id = PutRufus();

            var opts = new RiakGetOptions();
            opts.SetR(3);
            var rslt = client.Get(id, opts);
            CheckResult(rslt);
            Debug.WriteLine(Encoding.UTF8.GetString(rslt.Value.Value));
        }

        [Test]
        public void StoringAndSpecifyingContentType()
        {
            id = new RiakObjectId("quotes", "oscar_wilde", "genius");
            var obj = new RiakObject(
                id,
                "I have nothing to declare but my genius",
                RiakConstants.ContentTypes.TextPlain);
            rslt = client.Put(obj);
        }

        [Test]
        public void CausalContext()
        {
            id = new RiakObjectId("sports", "nba", "champion");
            var obj = new RiakObject(id, "Washington Generals",
                RiakConstants.ContentTypes.TextPlain);
            CheckResult(client.Put(obj));

            var rslt = client.Get(id);
            CheckResult(rslt);

            obj = rslt.Value;
            obj.SetObject("Harlem Globetrotters", RiakConstants.ContentTypes.TextPlain);
            rslt = client.Put(obj);
            CheckResult(rslt);

            Assert.IsTrue(EnumerableUtil.IsNullOrEmpty(rslt.Value.Siblings));
            Assert.AreEqual("Harlem Globetrotters", Encoding.UTF8.GetString(rslt.Value.Value));
        }

        [Test]
        public void RandomKeyGeneratedByRiak()
        {
            id = new RiakObjectId("users", "random_user_keys", null);
            var obj = new RiakObject(id, @"{'user':'data'}",
                RiakConstants.ContentTypes.ApplicationJson);
            var rslt = client.Put(obj);
            CheckResult(rslt);
            Debug.WriteLine(format: "Generated key: {0}", args: rslt.Value.Key);
        }

        [Test]
        public void DeleteAnObject()
        {
            id = new RiakObjectId("users", "random_user_keys", null);
            var obj = new RiakObject(id, @"{'user':'data'}",
                RiakConstants.ContentTypes.ApplicationJson);
            var rslt = client.Put(obj);
            CheckResult(rslt);

            string key = rslt.Value.Key;
            Debug.WriteLine(format: "Generated key: {0}", args: key);

            id = new RiakObjectId("users", "random_user_keys", key);
            var del_rslt = client.Delete(id);
            CheckResult(del_rslt);
        }

        [Test]
        public void GetBucketTypeProperties()
        {
            var rslt = client.GetBucketProperties("n_val_of_5", "any_bucket_name");
            CheckResult(rslt);
            RiakBucketProperties props = rslt.Value;
            Assert.AreEqual(5, props.NVal);
        }

        private RiakObjectId PutRufus()
        {
            id = new RiakObjectId("animals", "dogs", "rufus");
            var obj = new RiakObject(id, "WOOF!", RiakConstants.ContentTypes.TextPlain);
            CheckResult(client.Put(obj));
            return id;
        }
    }
}
