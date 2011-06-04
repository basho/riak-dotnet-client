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
        protected RiakResult Result;
        protected byte[] ClientId;

        protected void SetUpInternal()
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(ClientId != null)
            {
                Cluster.Setup(m => m.UseConnection(ClientId, It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object, ClientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }

    public abstract class RiakClientTestBase<TResult>
    {
        protected Mock<IRiakCluster> Cluster;
        protected Mock<IRiakConnection> Conn;
        protected RiakClient Client;
        protected RiakResult<TResult> Result;
        protected byte[] ClientId;

        protected void SetUpInternal()
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(ClientId != null)
            {
                Cluster.Setup(m => m.UseConnection(ClientId, It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object, ClientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }
}
