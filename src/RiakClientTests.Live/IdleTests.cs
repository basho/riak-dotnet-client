namespace RiakClientTests.Live.IdleTests
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Comms;

    [TestFixture, IntegrationTest]
    [Ignore("Idling is undergoing rework, so these tests are currently invalid")]
    public class WhenConnectionGoesIdle : LiveRiakConnectionTestBase
    {
        private IRiakConnection GetIdleConnection()
        {
            var result = Cluster.UseConnection(RiakResult<IRiakConnection>.Success, 1);
            //System.Threading.Thread.Sleep(ClusterConfig.RiakNodes[0].IdleTimeout + 1000);
            return result.Value;
        }

        [Test]
        public void ConnectionIsRestoredOnNextUse()
        {
            GetIdleConnection();
            var result = Client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }
    }
}
