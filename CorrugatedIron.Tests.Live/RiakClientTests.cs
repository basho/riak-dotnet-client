// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class RiakClientTests : LiveRiakConnectionTestBase
    {
        [Test]
        public void DeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.IsSuccess.ShouldBeTrue();

            var delResult = Client.Delete(riakObjectId);
            delResult.IsSuccess.ShouldBeTrue();

            var getResult = Client.Get(riakObjectId);
            getResult.IsSuccess.ShouldBeFalse();
            getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
            getResult.Value.ShouldBeNull();
        }

        [Test]
        public void AsyncDeleteIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            var putResult = Client.Put(riakObject);
            putResult.IsSuccess.ShouldBeTrue();

            RiakResult theResult = null;
            var resetEvent = new AutoResetEvent(false);

            Client.Delete(riakObjectId, result =>
                                            {
                                                theResult = result;
                                                resetEvent.Set();
                                            });
            resetEvent.WaitOne();

            theResult.IsSuccess.ShouldBeTrue();

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

            IEnumerable<RiakResult> theResults = null;

            var list = new List<RiakObjectId> { oneObjectId, twoObjectId };

            var resetEvent = new AutoResetEvent(false);

            Client.Delete(list, results =>
                                    {
                                        theResults = results;
                                        resetEvent.Set();
                                    });
            resetEvent.WaitOne();

            foreach (var riakResult in theResults)
            {
                riakResult.IsSuccess.ShouldBeTrue();
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

            IEnumerable<RiakResult<RiakObject>> theResults = null;

            var list = new List<RiakObjectId> {oneObjectId, twoObjectId};

            var resetEvent = new AutoResetEvent(false);

            Client.Get(list, results =>
                                 {
                                     theResults = results;
                                     resetEvent.Set();
                                 });
            resetEvent.WaitOne();

            foreach (var result in theResults)
            {
                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldNotBeNull();
            }
        }

        [Test]
        public void AsyncGetWithRiakObjectIdReturnsData()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var riakObjectId = riakObject.ToRiakObjectId();

            Client.Put(riakObject);

            RiakResult<RiakObject> theResult = null;
            var resetEvent = new AutoResetEvent(false);

            Client.Get(riakObjectId, result =>
                                         {
                                             theResult = result;
                                             resetEvent.Set();
                                         });
            resetEvent.WaitOne();

            theResult.IsSuccess.ShouldBeTrue();
            theResult.Value.ShouldNotBeNull();
            theResult.Value.Bucket.ShouldEqual(TestBucket);
            theResult.Value.Key.ShouldEqual(TestKey);
            theResult.Value.Value.FromRiakString().ShouldEqual(TestJson);
        }

        [Test]
        public void AsyncPutIsSuccessful()
        {
            var riakObject = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);

            RiakResult<RiakObject> theResult = null;

            var resetEvent = new AutoResetEvent(false);

            Client.Put(riakObject, result =>
                                       {
                                           theResult = result;
                                           resetEvent.Set();
                                       });
            resetEvent.WaitOne();
            
            theResult.IsSuccess.ShouldBeTrue();
            theResult.Value.ShouldNotBeNull();
        }

        [Test]
        public void AsyncPutMultipleIsSuccessful()
        {
            var one = new RiakObject(TestBucket, "one", TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var two = new RiakObject(TestBucket, "two", TestJson, RiakConstants.ContentTypes.ApplicationJson);

            IEnumerable<RiakResult<RiakObject>> theResults = null;
            var resetEvent = new AutoResetEvent(false);

            Client.Put(new List<RiakObject> {one, two}, result =>
                                                            {
                                                                theResults = result;
                                                                resetEvent.Set();
                                                            });
            resetEvent.WaitOne();

            foreach (var riakResult in theResults)
            {
                riakResult.IsSuccess.ShouldBeTrue();
                riakResult.Value.ShouldNotBeNull();
            }
        }
    }
}
