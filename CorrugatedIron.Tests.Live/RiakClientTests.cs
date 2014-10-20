// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System.Reactive.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class RiakClientTests : LiveRiakConnectionTestBase
    {
        [Test]
        public void WritingLargeObjectIsSuccessful()
        {
            var text = Enumerable.Range(0, 100000).Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString())).ToString();
            var riakObject = new RiakObject(TestBucket, "large", text, RiakConstants.ContentTypes.TextPlain);
            var result = Client.Put(riakObject);
            result.ShouldNotBeNull();
        }

        [Test]
        public void DeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.ShouldNotBeNull();

            Client.Delete(riakObjectId).ShouldNotBeNull();

            var getResult = Client.Get(riakObjectId);
            getResult.ShouldBeNull();
        }

        [Test]
        public void DeleteIsSuccessfulInBatch()
        {
            Client.Batch(batch =>
            {
                var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
                var riakObjectId = riakObject.ToRiakObjectId();

                var putResult = batch.Put(riakObject);
                putResult.ShouldNotBeNull();

                var delResult = batch.Delete(riakObjectId);
                delResult.ShouldNotBeNull();

                var getResult = batch.Get(riakObjectId);
                getResult.ShouldBeNull();
            });
        }

        [Test]
        public void AsyncDeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.ShouldNotBeNull();

            var deletedObjectId = Client.Async.Delete(riakObjectId).ConfigureAwait(false).GetAwaiter().GetResult();

            var getResult = Client.Get(riakObjectId);
            getResult.ShouldBeNull();
        }

        [Test]
        public void AsyncDeleteMultipleIsSuccessful()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            Client.Put(one).ShouldNotBeNull();
            Client.Put(two).ShouldNotBeNull();

            var oneObjectId = one.ToRiakObjectId();
            var twoObjectId = two.ToRiakObjectId();

            var list = new List<RiakObjectId> { oneObjectId, twoObjectId };

            var deletedObjectIds = Client.Async.Delete(list)
                .ToEnumerable()
                .ToList();
            deletedObjectIds.Count().ShouldEqual(2);

            var oneResult = Client.Get(oneObjectId);
            oneResult.ShouldBeNull();

            var twoResult = Client.Get(twoObjectId);
            twoResult.ShouldBeNull();
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

            var results = Client.Async.Get(list)
                .ToEnumerable()
                .ToList();

            results.Count().ShouldEqual(2);
        }

        [Test]
        public void AsyncGetWithRiakObjectIdReturnsData()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            Client.Put(riakObject);

            var result = Client.Async.Get(riakObjectId).ConfigureAwait(false).GetAwaiter().GetResult();
            result.IsLeft.ShouldBeFalse();

            var updatedRiakObject = result.Right;
            updatedRiakObject.Bucket.ShouldEqual(TestBucket);
            updatedRiakObject.Key.ShouldEqual(TestKey);
            updatedRiakObject.Value.FromRiakString().ShouldEqual(TestJson);
        }

        [Test]
        public void AsyncPutIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);

            var result = Client.Async.Put(riakObject).ConfigureAwait(false).GetAwaiter().GetResult();
            result.ShouldNotBeNull();
        }

        [Test]
        public void AsyncPutMultipleIsSuccessful()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            var results = Client.Async.Put(new List<RiakObject> { one, two })
                .ToEnumerable()
                .ToList();

            results.Count.ShouldEqual(2);
        }

        [Test]
        public void ListKeysFromIndexReturnsAllKeys()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());
            var originalKeyList = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");
                originalKeyList.Add(i.ToString());

                Client.Put(o);
            }

            var keys = ((RiakClient)Client).ListKeysFromIndex(bucket).ToList();

            keys.Count().ShouldEqual(10);

            foreach (var key in keys)
            {
                originalKeyList.ShouldContain(key);
            }

            Client.DeleteBucket(bucket);
        }

        [Test]
        public void UpdatingCounterOnBucketWithoutAllowMultiFails()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());
            var counter = "counter";
            Exception expectedException = null;
            try
            {
                var result = Client.IncrementCounter(bucket, counter, 1);
            }
            catch (Exception exception)
            {
                expectedException = exception;
            }

            expectedException.ShouldNotBeNull();
        }

        [Test]
        public void UpdatingCounterOnBucketWithAllowMultiIsSuccessful()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());
            var counter = "counter";

            var props = Client.GetBucketProperties(bucket);
            props.SetAllowMultiple(true);

            Client.SetBucketProperties(bucket, props).ShouldBeTrue();

            var result = Client.IncrementCounter(bucket, counter, 1);
            result.ShouldNotBeNull();
        }

        [Test]
        public void UpdatingCounterOnBucketWithReturnValueShouldReturnIncrementedCounterValue()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());
            var counter = "counter";

            var props = Client.GetBucketProperties(bucket) ?? new RiakBucketProperties();
            props.SetAllowMultiple(true);

            Client.SetBucketProperties(bucket, props).ShouldBeTrue();

            Client.IncrementCounter(bucket, counter, 1, new RiakCounterUpdateOptions().SetReturnValue(true));

            var readResult = Client.GetCounter(bucket, counter);
            var currentCounter = readResult.Value;

            var result = Client.IncrementCounter(bucket, counter, 1, new RiakCounterUpdateOptions().SetReturnValue(true));
            result.Value.ShouldBeGreaterThan(currentCounter);
        }

        [Test, Ignore]
        public void ReadingWithTimeoutSetToZeroShouldImmediatelyReturn()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");

                Client.Put(o);
            }

            var result = Client.Get(bucket, "2", new RiakGetOptions().SetTimeout(0).SetPr(RiakConstants.QuorumOptions.All));
            result.ShouldBeNull();
            //maybe exception should thrown?
        }
    }
}
