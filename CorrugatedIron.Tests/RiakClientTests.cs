using System.Collections.Generic;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using CorrugatedIron.Tests.Extensions;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.RiakClientTests
{
    [TestFixture]
    internal class RiakClientTests
    {
        #region Setup/Teardown

        [SetUp]
        protected void SetUp()
        {
            ConnMock = new Mock<IRiakConnection>();
            ClusterConfigMock = new Mock<IRiakClusterConfiguration>();
            ConnFactoryMock = new Mock<IRiakConnectionFactory>();
            NodeConfigMock = new Mock<IRiakNodeConfiguration>();

            ConnFactoryMock.Setup(m => m.CreateConnection(It.IsAny<IRiakNodeConfiguration>())).Returns(ConnMock.Object);
            NodeConfigMock.SetupGet(m => m.PoolSize).Returns(1);
            ClusterConfigMock.SetupGet(m => m.RiakNodes).Returns(new List<IRiakNodeConfiguration>
                                                                     {NodeConfigMock.Object});

            Cluster = new RiakCluster(ClusterConfigMock.Object, ConnFactoryMock.Object);
            Client = new RiakClient(Cluster);
        }

        #endregion

        protected Mock<IRiakConnection> ConnMock;
        protected Mock<IRiakNodeConfiguration> NodeConfigMock;
        protected Mock<IRiakClusterConfiguration> ClusterConfigMock;
        protected Mock<IRiakConnectionFactory> ConnFactoryMock;
        protected RiakCluster Cluster;
        protected RiakClient Client;

        [Test]
        public void ClientIdIsNotNull()
        {
            Client.ClientId.ShouldNotBeNull();
            Client.ClientId.ShouldNotEqual(new byte[] {0, 0, 0, 0});
        }
    }
}