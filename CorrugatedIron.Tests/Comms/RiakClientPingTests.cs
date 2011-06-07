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

using System;
using CorrugatedIron.Comms;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Messages;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Comms.RiakClientPingTests
{
    public abstract class RiakClientPingTestBase : RiakClientTestBase
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
            Cluster.Verify(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult>>()), Times.Once());
        }
    }

    [TestFixture]
    public class WhenCallingPingWithError : RiakClientPingTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            Result = RiakResult.Error(ResultCode.CommunicationError);
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
    public class WhenCallingPingSuccessfully : RiakClientPingTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            Result = RiakResult.Success();
            base.SetUp();
        }

        [Test]
        public void SuccessResultIsReturned()
        {
            Response.IsSuccess.ShouldBeTrue();
        }
    }
}
