using System;
using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Util;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Comms.RiakClientSetBucketPropertiesTests
{
    public class MockCluster : IRiakCluster
    {
        public Mock<IRiakConnection> ConnectionMock = new Mock<IRiakConnection>();

        public void Dispose()
        {
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun)
        {
            return useFun(ConnectionMock.Object);
        }
    }

    public abstract class RiakClientSetBucketPropertiesTestBase
    {
        protected MockCluster Cluster;
        protected RiakClient Client;
        protected byte[] ClientId;

        protected RiakClientSetBucketPropertiesTestBase()
        {
            Cluster = new MockCluster();
            ClientId = System.Text.Encoding.Default.GetBytes("fadjskl").Take(4).ToArray();
            Client = new RiakClient(Cluster, ClientId);
        }
    }

    [TestFixture]
    public class WhenSettingBucketPropertiesWithExtendedProperties : RiakClientSetBucketPropertiesTestBase
    {
        protected RiakResult Response;
        [SetUp]
        public void SetUp()
        {
            var result = RiakResult<RiakRestResponse>.Success(new RiakRestResponse { StatusCode = System.Net.HttpStatusCode.NoContent });
            Cluster.ConnectionMock.Setup(m => m.RestRequest(It.IsAny<RiakRestRequest>())).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true).SetRVal("one"));
        }

        [Test]
        public void RestInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.RestRequest(It.Is<RiakRestRequest>(r => r.ContentType == Constants.ContentTypes.ApplicationJson
                && r.Method == Constants.Rest.HttpMethod.Put)), Times.Once());
        }
    }

    [TestFixture]
    public class WhenSettingBucketPropertiesWithoutExtendedProperties : RiakClientSetBucketPropertiesTestBase
    {
        protected RiakResult Response;
        [SetUp]
        public void SetUp()
        {
            var result = RiakResult<RpbSetBucketResp>.Success(new RpbSetBucketResp());
            Cluster.ConnectionMock.Setup(m => m.PbcWriteRead<RpbSetBucketReq, RpbSetBucketResp>(It.IsAny<RpbSetBucketReq>())).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true));
        }

        [Test]
        public void PbcInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.PbcWriteRead<RpbSetBucketReq, RpbSetBucketResp>(It.Is<RpbSetBucketReq>(r => r.Props.AllowMultiple)), Times.Once());
        }
    }
}
