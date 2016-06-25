namespace RiakClientTests.Client
{
    using System;
    using System.Linq;
    using RiakClient;

    public abstract class ClientTestBase : IDisposable
    {
        protected MockCluster Cluster;
        protected RiakClient Client;
        protected byte[] ClientId;

        public ClientTestBase()
        {
            Cluster = new MockCluster();
            ClientId = System.Text.Encoding.Default.GetBytes("fadjskl").Take(4).ToArray();
            Client = new RiakClient(Cluster);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Cluster != null)
            {
                Cluster.Dispose();
            }
        }
    }
}
