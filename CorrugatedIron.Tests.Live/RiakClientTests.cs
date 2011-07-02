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

using System.Threading;
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
    }
}
