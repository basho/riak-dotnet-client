namespace RiakClientTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    [TestFixture, IntegrationTest]
    public class BucketTypeTests : LiveRiakConnectionTestBase
    {
        private const string Value = "value";

        [Test]
        public void TestKVOperations()
        {
            string key = Guid.NewGuid().ToString();
            var id = new RiakObjectId(TestBucketType, TestBucket, key);
            var obj = new RiakObject(id, Value);

            // put
            var putOptions = new RiakPutOptions();
            putOptions.SetReturnBody(true);
            putOptions.SetDw(3);
            putOptions.SetTimeout(new Timeout(TimeSpan.FromSeconds(60)));
            var putResult = Client.Put(obj, putOptions);
            Assert.True(putResult.IsSuccess, putResult.ErrorMessage);
            Assert.AreEqual(TestBucketType, putResult.Value.BucketType);

            // get
            var getResult = Client.Get(id);
            Assert.True(getResult.IsSuccess);
            Assert.AreEqual(TestBucketType, getResult.Value.BucketType);

            // delete
            var deleteOptions = new RiakDeleteOptions();
            deleteOptions.Vclock = getResult.Value.VectorClock;
            deleteOptions.SetDw(3);
            var deleteResult = Client.Delete(id, new RiakDeleteOptions().SetDw(3));
            Assert.True(deleteResult.IsSuccess);

            // multiget
            var ids = new List<RiakObjectId>();
            for (int i = 0; i < 3; i++)
            {
                obj = new RiakObject(new RiakObjectId(TestBucketType, TestBucket, key + i), Value);
                Client.Put(obj, new RiakPutOptions().SetReturnBody(false).SetDw(3));
                ids.Add(obj.ToRiakObjectId());
            }

            var multiGetResult = Client.Get(ids).ToList();
            Assert.True(multiGetResult.All(r => r.IsSuccess));
            Assert.True(multiGetResult.All(r => r.Value.BucketType == TestBucketType));
        }

        [Test]
        public void TestListingOperations()
        {
            string bucket1 = Guid.NewGuid().ToString();
            string bucket2 = Guid.NewGuid().ToString();
            string key = Guid.NewGuid().ToString();

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
            Assert.True(getPropsResult.IsSuccess, getPropsResult.ErrorMessage);
            Assert.True(getPropsResult.Value.AllowMultiple.Value);

            // set
            var props = getPropsResult.Value;
            props.SetAllowMultiple(false);

            var setPropsResult = Client.SetBucketProperties(bucketType, bucket, props);
            Assert.IsTrue(setPropsResult.IsSuccess, setPropsResult.ErrorMessage);

            Func<RiakResult<RiakBucketProperties>> getFunc = () =>
                {
                    var getResult = Client.GetBucketProperties(bucketType, bucket);
                    Assert.IsTrue(getResult.IsSuccess, getResult.ErrorMessage);
                    return getResult;
                };

            Func<RiakResult<RiakBucketProperties>, bool> successFunc =
                (r) => r.Value.AllowMultiple.Value == false;
            getFunc.WaitUntil(successFunc);

            // reset
            var resetPropsResult = Client.ResetBucketProperties(bucketType, bucket);
            Assert.IsTrue(resetPropsResult.IsSuccess, resetPropsResult.ErrorMessage);

            successFunc = (r) => r.Value.AllowMultiple.Value == true;
            getFunc.WaitUntil(successFunc);
        }
    }
}