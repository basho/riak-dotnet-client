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
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Messages;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Comms.RiakClientPingTests
{
    internal abstract class RiakClientPingTestBase : RiakClientTestBase<RpbPingReq, RpbPingResp>
    {
        protected RiakResult Response;

        public virtual void SetUp()
        {
            SetUpInternal();
            Response = Client.Ping();
        }

        [Test]
        public void PbcClientIsInvoked()
        {
            ConnMock.Verify(m => m.PbcWriteRead<RpbPingReq, RpbPingResp>(It.IsAny<RpbPingReq>()), Times.Once());
        }
    }

    [TestFixture]
    internal class WhenCallingPingWithError : RiakClientPingTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            Result = RiakResult<RpbPingResp>.Error(ResultCode.CommunicationError);
            base.SetUp();
        }

        [Test]
        public void SuccessResultIsReturned()
        {
            Response.IsSuccess.ShouldBeFalse();
            Response.ResultCode.ShouldEqual(ResultCode.CommunicationError);
        }
    }

    [TestFixture]
    internal class WhenCallingPingSuccessfully : RiakClientPingTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            Result = RiakResult<RpbPingResp>.Success(new RpbPingResp());
            base.SetUp();
        }

        [Test]
        public void SuccessResultIsReturned()
        {
            Response.IsSuccess.ShouldBeTrue();
        }
    }

    [TestFixture]
    internal class WhenCallingPingAsynchronously : RiakClientTestBase<RpbPingReq, RpbPingResp>
    {
        private readonly ManualResetEvent _indicator = new ManualResetEvent(false);
        private volatile bool _done = false;

        [SetUp]
        public void SetUp()
        {
            Result = RiakResult<RpbPingResp>.Success(new RpbPingResp());
            SetUpInternal();
        }

        [Test]
        public void CallbackIsInvokedCorrectly()
        {
            Client.Async.Ping(Callback);
            _indicator.WaitOne();
            _done.ShouldBeTrue();
        }

        public void Callback(RiakResult result)
        {
            _done = result.IsSuccess;
            _indicator.Set();
        }
    }
}
