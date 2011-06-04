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
        protected RiakResult Response;

        public virtual void SetUp()
        {
            SetUpInternal();
            Response = Client.Ping();
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
    public class WhenCallingPingSuccessfully : RiakClientPingTestBase
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
}
