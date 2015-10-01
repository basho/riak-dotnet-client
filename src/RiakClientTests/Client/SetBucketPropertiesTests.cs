namespace RiakClientTests.Client
{
    using Moq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture, UnitTest]
    public class WhenSettingBucketPropertiesWithoutExtendedProperties : ClientTestBase
    {
        protected RiakResult Response;

        [SetUp]
        public void SetUp()
        {
            var result = RiakResult.Success();
            Cluster.ConnectionMock.Setup(m => m.PbcWriteRead(It.IsAny<RpbSetBucketReq>(), MessageCode.RpbSetBucketResp)).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true));
        }

        [Test]
        public void PbcInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.PbcWriteRead(It.Is<RpbSetBucketReq>(r => r.props.allow_mult), MessageCode.RpbSetBucketResp), Times.Once());
        }
    }
}
