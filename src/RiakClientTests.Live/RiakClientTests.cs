namespace RiakClientTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;

    [TestFixture, IntegrationTest, SkipMono]
    public class RiakClientTests : LiveRiakConnectionTestBase
    {
        [Test]
        [Ignore("Nondeterministic or failing")]
        public void WritingLargeObjectIsSuccessful()
        {
            var text = Enumerable.Range(0, 2000000)
                                 .Aggregate(new StringBuilder(),
                                            (sb, i) => sb.Append(i.ToString()))
                                 .ToString();
            var riakObject = new RiakObject(TestBucket, "large", text, RiakConstants.ContentTypes.TextPlain);
            var result = Client.Put(riakObject);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
        }

        [Test]
        public void DeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.IsSuccess.ShouldBeTrue(putResult.ErrorMessage);

            var delResult = Client.Delete(riakObjectId);
            delResult.IsSuccess.ShouldBeTrue(delResult.ErrorMessage);

            var getResult = Client.Get(riakObjectId);
            getResult.IsSuccess.ShouldBeFalse(getResult.ErrorMessage);
            getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
            getResult.Value.ShouldBeNull();
        }

        [Test]
        public void DeleteIsSuccessfulInBatch()
        {
            Client.Batch(batch =>
            {
                var riakObject = new RiakObject(TestBucket, TestKey, TestJson,
                                                RiakConstants.ContentTypes.ApplicationJson);
                var riakObjectId = riakObject.ToRiakObjectId();

                var putResult = batch.Put(riakObject);
                putResult.IsSuccess.ShouldBeTrue(putResult.ErrorMessage);

                var delResult = batch.Delete(riakObjectId);
                delResult.IsSuccess.ShouldBeTrue(delResult.ErrorMessage);

                var getResult = batch.Get(riakObjectId);
                getResult.IsSuccess.ShouldBeFalse();
                getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
                getResult.Value.ShouldBeNull();
            });
        }

        [Test]
        public void AsyncDeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.IsSuccess.ShouldBeTrue(putResult.ErrorMessage);

            var result = Client.Async.Delete(riakObjectId).Result;

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var getResult = Client.Get(riakObjectId);
            getResult.IsSuccess.ShouldBeFalse();
            getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
            getResult.Value.ShouldBeNull();
        }

        [Test]
        public void AsyncDeleteMultipleIsSuccessful()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            Client.Put(one);
            Client.Put(two);

            var oneObjectId = one.ToRiakObjectId();
            var twoObjectId = two.ToRiakObjectId();

            var list = new List<RiakObjectId> { oneObjectId, twoObjectId };

            var results = Client.Async.Delete(list).Result;

            foreach (var riakResult in results)
            {
                riakResult.IsSuccess.ShouldBeTrue(riakResult.ErrorMessage);
            }

            var oneResult = Client.Get(oneObjectId);
            oneResult.IsSuccess.ShouldBeFalse();
            oneResult.ResultCode.ShouldEqual(ResultCode.NotFound);
            oneResult.Value.ShouldBeNull();

            var twoResult = Client.Get(twoObjectId);
            twoResult.IsSuccess.ShouldBeFalse();
            twoResult.ResultCode.ShouldEqual(ResultCode.NotFound);
            twoResult.Value.ShouldBeNull();
        }

        [Test]
        public void AsyncGetMultipleReturnsAllObjects()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            Client.Put(one);
            Client.Put(two);

            var oneObjectId = one.ToRiakObjectId();
            var twoObjectId = two.ToRiakObjectId();

            var list = new List<RiakObjectId> { oneObjectId, twoObjectId };

            var results = Client.Async.Get(list).Result;

            foreach (var result in results)
            {
                result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
                result.Value.ShouldNotBeNull();
            }
        }

        [Test]
        public void AsyncGetWithRiakObjectIdReturnsData()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            Client.Put(riakObject);

            Func<RiakResult<RiakObject>> asyncGet = () => Client.Async.Get(riakObjectId).Result;

            var asyncGetResult = asyncGet.WaitUntil();

            asyncGetResult.IsSuccess.ShouldBeTrue(asyncGetResult.ErrorMessage);
            asyncGetResult.Value.ShouldNotBeNull();
            asyncGetResult.Value.Bucket.ShouldEqual(TestBucket);
            asyncGetResult.Value.Key.ShouldEqual(TestKey);
            asyncGetResult.Value.Value.FromRiakString().ShouldEqual(TestJson);
        }

        [Test]
        public void AsyncPutIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);

            var result = Client.Async.Put(riakObject).Result;

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.ShouldNotBeNull();
        }

        [Test]
        public void AsyncPutMultipleIsSuccessful()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            var results = Client.Async.Put(new List<RiakObject> { one, two }).Result;

            foreach (var riakResult in results)
            {
                riakResult.IsSuccess.ShouldBeTrue(riakResult.ErrorMessage);
                riakResult.Value.ShouldNotBeNull();
            }
        }

        [Test]
        public void ListKeysFromIndexReturnsAllKeys()
        {
            const int keyCount = 10;
            const string listKeysBucket = "listKeysBucket";
            var originalKeyList = new List<string>();

            for (var i = 0; i < keyCount; i++)
            {
                string idx = i.ToString();
                var id = new RiakObjectId(TestBucketType, listKeysBucket, idx);
                var o = new RiakObject(id, "{ value: \"this is an object\" }");
                originalKeyList.Add(idx);
                Client.Put(o);
            }

            var result = Client.ListKeysFromIndex(TestBucketType, listKeysBucket);
            var keys = result.Value;

            keys.Count.ShouldEqual(keyCount);

            foreach (var key in keys)
            {
                originalKeyList.ShouldContain(key);
            }

            Client.DeleteBucket(TestBucketType, listKeysBucket);
        }

        [Test]
        public void UpdatingCounterOnBucketWithoutAllowMultFails()
        {
            const string counter = "counter";
            var bucket = TestBucket + "_" + Guid.NewGuid();

            var result = Client.IncrementCounter(bucket, counter, 1);

            result.Result.IsSuccess.ShouldBeFalse();
        }

        [Test]
        public void UpdatingCounterOnBucketWithAllowMultIsSuccessful()
        {
            const string counter = "counter";
            var bucket = TestBucket + "_" + Guid.NewGuid();

            var props = Client.GetBucketProperties(bucket).Value;
            props.SetAllowMultiple(true);

            Client.SetBucketProperties(bucket, props);

            var result = Client.IncrementCounter(bucket, counter, 1);

            result.Result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        [Ignore("Nondeterministic or failing")]
        public void UpdatingCounterOnBucketWithReturnValueShouldReturnIncrementedCounterValue()
        {
            const string counter = "counter";
            var bucket = TestBucket + "_" + Guid.NewGuid();

            var props = Client.GetBucketProperties(bucket).Value ?? new RiakBucketProperties();
            props.SetAllowMultiple(true);

            Client.SetBucketProperties(bucket, props);

            Client.IncrementCounter(bucket, counter, 1, new RiakCounterUpdateOptions().SetReturnValue(true));

            var readResult = Client.GetCounter(bucket, counter);
            var currentCounter = readResult.Value;

            var result = Client.IncrementCounter(bucket, counter, 1, new RiakCounterUpdateOptions().SetReturnValue(true));

            result.Result.IsSuccess.ShouldBeTrue();
            result.Result.ShouldNotBeNull();
            result.Value.ShouldBeGreaterThan(currentCounter);
        }

        [Test]
        [Ignore("Nondeterministic or failing")]
        public void ReadingWithTimeoutSetToZeroShouldImmediatelyReturn()
        {
            var bucket = TestBucket + "_" + Guid.NewGuid();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");

                Client.Put(o);
            }

            var result = Client.Get(bucket, "2",
                new RiakGetOptions().SetTimeout(new TimeSpan(0)).SetPr(Quorum.WellKnown.All));

            result.IsSuccess.ShouldBeFalse();
        }
    }
}
