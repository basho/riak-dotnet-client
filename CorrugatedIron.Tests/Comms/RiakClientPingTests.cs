using System;
using CorrugatedIron.Comms;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Messages;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Comms.RiakClientTests
{
    public abstract class RiakClientPingTestBase : RiakClientTestBase<RpbPingResp>
    {
        protected RiakResult Result;

        protected RiakClientPingTestBase(RiakResult<RpbPingResp> response)
            : base(response)
        {
        }

        [SetUp]
        public void SetUp()
        {
            Result = Client.Ping();
        }

        [Test]
        public void PbcClientIsInvoked()
        {
            Cluster.Verify(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult<RpbPingResp>>>()), Times.Once());
        }
    }

    [TestFixture]
    public class WhenCallingPingWithError : RiakClientPingTestBase
    {
        public WhenCallingPingWithError()
            : base(RiakResult<RpbPingResp>.Error(ResultCode.CommunicationError))
        {
        }

        [Test]
        public void SuccessResultIsReturned()
        {
            Result.IsSuccess.ShouldBeFalse();
            Result.ResultCode.ShouldEqual(ResultCode.CommunicationError);
        }
    }

    [TestFixture]
    public class WhenCallingPingSuccessfully : RiakClientPingTestBase
    {
        public WhenCallingPingSuccessfully()
            : base(RiakResult<RpbPingResp>.Success(new RpbPingResp()))
        {
        }

        [Test]
        public void SuccessResultIsReturned()
        {
            Result.IsSuccess.ShouldBeTrue();
        }
    }
}
