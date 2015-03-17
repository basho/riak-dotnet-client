// <copyright file="BucketTypes.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientExamples.Dev.Advanced
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    /*
     * http://docs.basho.com/riak/latest/dev/advanced/bucket-types/
     */
    public sealed class BucketTypes : ExampleBase
    {
        [Test]
        public void UsingDefaultBucket()
        {
            var id = new RiakObjectId("my_bucket", "my_key");
            var rslt = client.Get(id);
            CheckResult(rslt, true);
        }

        [Test]
        public void UsingBucketType()
        {
            var id1 = new RiakObjectId("users", "my_bucket", "my_key");
            var id2 = new RiakObjectId("users", "my_bucket", "my_key");
            var rslt1 = client.Get(id1);
            CheckResult(rslt1, true);
            var rslt2 = client.Get(id2);
            CheckResult(rslt2, true);
        }

        [Test]
        public void DefaultBucketCanBeSpecifiedAsType()
        {
            id = new RiakObjectId("default", "my_bucket", "my_key");
            var obj1 = new RiakObject(id, "value", RiakConstants.ContentTypes.TextPlain);
            var putRslt = client.Put(obj1);
            CheckResult(putRslt);

            var id2 = new RiakObjectId("my_bucket", "my_key");
            var getRslt = client.Get(id2);
            CheckResult(getRslt);

            RiakObject obj2 = getRslt.Value;
            Assert.AreEqual(obj1.Value, obj2.Value);
        }

        [Test]
        public void CreatingBucketInSpecificBucketType()
        {
            id = new RiakObjectId("no_siblings", "sensitive_user_data", "user19735");
            var obj = new RiakObject(id, "{\"name\":\"Bob\"}");
            var rslt = client.Put(obj);
            CheckResult(rslt);
        }

        [Test]
        public void CreatingBucketInSpecificBucketTypeExampleTwo()
        {
            id = new RiakObjectId("no_siblings", "old_memes", "all_your_base");
            var obj = new RiakObject(id, "all your base are belong to us",
                RiakConstants.ContentTypes.TextPlain);
            var rslt = client.Put(obj);
            CheckResult(rslt);
        }
    }
}
