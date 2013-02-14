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
            var text = Enumerable.Range(0, 2000000).Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString())).ToString();
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
                    var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
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

            var list = new List<RiakObjectId> {oneObjectId, twoObjectId};

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

            var result = Client.Async.Get(riakObjectId).Result;

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.ShouldNotBeNull();
            result.Value.Bucket.ShouldEqual(TestBucket);
            result.Value.Key.ShouldEqual(TestKey);
            result.Value.Value.FromRiakString().ShouldEqual(TestJson);
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

            var results = Client.Async.Put(new List<RiakObject> {one, two}).Result;

            foreach (var riakResult in results)
            {
                riakResult.IsSuccess.ShouldBeTrue(riakResult.ErrorMessage);
                riakResult.Value.ShouldNotBeNull();
            }
        }

        [Test]
        public void ListKeysFromIndexReturnsAllKeys()
        {
            var bucket = TestBucket + "_" + Guid.NewGuid().ToString();
            var originalKeyList = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");
                originalKeyList.Add(i.ToString());

                Client.Put(o);
            }

            var result = ((RiakClient)Client).ListKeysFromIndex(bucket);
            var keys = result.Value;

            keys.Count.ShouldEqual(10);

            foreach (var key in keys)
            {
                originalKeyList.ShouldContain(key);
            }

            Client.DeleteBucket(bucket);
        }
    }
}
