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

using System.Threading;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Tests.RiakClientTests;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.RiakClientPingTests
{
    internal abstract class RiakClientPingTestBase : RiakClientTestBase<RpbPingReq, RpbPingResp>
    {
        protected RiakResult Response;

        public virtual void SetUp()
        {
            SetUpInternal();
            Response = Client.Ping();
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
        public void CorrectResultIsReturned()
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

        [Test]
        public void PbcClientIsInvoked()
        {
            ConnMock.Verify(m => m.PbcWriteRead<RpbPingReq, RpbPingResp>(It.IsAny<RpbPingReq>()), Times.Once());
        }
    }

    [TestFixture]
    internal class WhenCallingPingAsynchronously : RiakClientTestBase<RpbPingReq, RpbPingResp>
    {
        [SetUp]
        public void SetUp()
        {
            Result = RiakResult<RpbPingResp>.Success(new RpbPingResp());
            SetUpInternal();
        }

        [Test]
        public void CallbackIsInvokedCorrectly()
        {
            var result = Client.Async.Ping().Result;
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
        }
    }
}
