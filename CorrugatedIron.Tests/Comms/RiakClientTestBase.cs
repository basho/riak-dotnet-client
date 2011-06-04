using System;
using CorrugatedIron.Comms;
using Moq;

namespace CorrugatedIron.Tests.Comms
{
    public abstract class RiakClientTestBase
    {
        protected Mock<IRiakCluster> Cluster;
        protected Mock<IRiakConnection> Conn;
        protected RiakClient Client;

        protected RiakClientTestBase(RiakResult result, byte[] clientId = null)
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(clientId != null)
            {
                Cluster.Setup(m => m.UseConnection(clientId, It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(result);
                Client = new RiakClient(Cluster.Object, clientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }

    public abstract class RiakClientTestBase<TResult>
    {
        protected Mock<IRiakCluster> Cluster;
        protected Mock<IRiakConnection> Conn;
        protected RiakClient Client;

        protected RiakClientTestBase(RiakResult<TResult> result, byte[] clientId = null)
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(clientId != null)
            {
                Cluster.Setup(m => m.UseConnection(clientId, It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(result);
                Client = new RiakClient(Cluster.Object, clientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }
}
