// <copyright file="BucketTypeTests.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Live
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient.Models;

    [TestFixture]
    public class BucketTypeTests : LiveRiakConnectionTestBase
    {
        private const string Value = "value";

        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(TestBucketType, TestBucket);
        }

        [Test]
        public void TestKVOperations()
        {
            const string key = "bucket_type_test_key";
            var id = new RiakObjectId(TestBucketType, TestBucket, key);
            var obj = new RiakObject(id, Value);

            // put
            var putResult = Client.Put(obj, new RiakPutOptions().SetReturnBody(true).SetDw(3));
            putResult.IsSuccess.ShouldBeTrue();
            putResult.Value.BucketType.ShouldEqual(TestBucketType);

            // get
            var getResult = Client.Get(id);
            getResult.IsSuccess.ShouldBeTrue();
            getResult.Value.BucketType.ShouldEqual(TestBucketType);

            // delete
            var deleteResult = Client.Delete(id, new RiakDeleteOptions().SetDw(3));
            deleteResult.IsSuccess.ShouldBeTrue();

            // multiget
            var ids = new List<RiakObjectId>();
            for (int i = 0; i < 3; i++)
            {
                obj = new RiakObject(new RiakObjectId(TestBucketType, TestBucket, key + i), Value);
                Client.Put(obj, new RiakPutOptions().SetReturnBody(true).SetDw(3));
                ids.Add(obj.ToRiakObjectId());
            }

            var multiGetResult = Client.Get(ids).ToList();
            multiGetResult.All(r => r.IsSuccess).ShouldBeTrue();
            multiGetResult.All(r => r.Value.BucketType == TestBucketType).ShouldBeTrue();
        }

        [Test]
        public void TestListingOperations()
        {
            const string bucket1 = "bucket1";
            const string bucket2 = "bucket2";
            const string key = "1";

            Client.Put(new RiakObject(new RiakObjectId(TestBucketType, bucket1, key), Value),
                new RiakPutOptions().SetDw(3));
            Client.Put(new RiakObject(new RiakObjectId(TestBucketType, bucket2, key), Value),
                new RiakPutOptions().SetDw(3));

            // list keys
            var listKeys = Client.ListKeys(TestBucketType, bucket1);

            listKeys.IsSuccess.ShouldBeTrue();
            var keys = listKeys.Value.ToList();
            keys.Count.ShouldEqual(1);
            keys.First().ShouldEqual(key);

            // list buckets
            var listBuckets = Client.ListBuckets(TestBucketType);

            listBuckets.IsSuccess.ShouldBeTrue();
            var buckets = listBuckets.Value.ToList();
            buckets.ShouldContain(bucket1);
            buckets.ShouldContain(bucket2);
        }

        [Test]
        public void Test2iOperations()
        {
            const string indexName = "num";
            const string key1 = "key1";
            const string key2 = "key2";

            var obj1 = new RiakObject(new RiakObjectId(TestBucketType, TestBucket, key1), Value);
            obj1.IntIndex(indexName).Add(1);
            
            var obj2 = new RiakObject(new RiakObjectId(TestBucketType, TestBucket, key2), Value);
            obj2.IntIndex(indexName).Add(2);

            Client.Put(obj1, new RiakPutOptions().SetDw(3));
            Client.Put(obj2, new RiakPutOptions().SetDw(3));

            // fetch 2i
            var indexId = new RiakIndexId(TestBucketType, TestBucket, indexName);
            var indexResult = Client.GetSecondaryIndex(indexId, 1, 2, new RiakIndexGetOptions().SetReturnTerms(true));

            indexResult.IsSuccess.ShouldBeTrue();
            var keyTerms = indexResult.Value.IndexKeyTerms.ToList();
            var keys = keyTerms.Select(t => t.Key).ToList();
            var terms = keyTerms.Select(t => t.Term).ToList();
            
            keys.ShouldContain(key1);
            keys.ShouldContain(key2);
            
            terms.ShouldContain("1");
            terms.ShouldContain("2");
        }

        [Test]
        public void TestBucketPropertyOperations()
        {
            const string bucketType = "plain";
            const string bucket = "BucketPropsTestBucket";

            // get
            var getPropsResult = Client.GetBucketProperties(bucketType, bucket);
            getPropsResult.IsSuccess.ShouldBeTrue();
            getPropsResult.Value.LastWriteWins.Value.ShouldBeFalse();

            // set
            var props = getPropsResult.Value;
            props.SetLastWriteWins(true);
            
            var setPropsResult = Client.SetBucketProperties(bucketType, bucket, props);
            setPropsResult.IsSuccess.ShouldBeTrue();
            getPropsResult = Client.GetBucketProperties(bucketType, bucket);
            getPropsResult.Value.LastWriteWins.Value.ShouldBeTrue();
            
            // reset
            var resetPropsResult = Client.ResetBucketProperties(bucketType, bucket);
            resetPropsResult.IsSuccess.ShouldBeTrue();
            getPropsResult = Client.GetBucketProperties(bucketType, bucket);
            getPropsResult.Value.LastWriteWins.Value.ShouldBeFalse();
        }
    }
}
