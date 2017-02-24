namespace Test.Integration
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Config;
    using RiakClient.Util;

    [TestFixture, IntegrationTest]
    public class SecurityTests
    {
        [Test]
        public void Tls_Configuration()
        {
            if (MonoUtil.IsRunningOnMono)
            {
                Assert.Ignore("Mono");
            }

            var config = RiakClusterConfiguration.LoadFromConfig("riakTlsConfiguration");
            var cluster = new RiakCluster(config);
            var client = cluster.CreateClient();
            var r = client.Ping();
            Assert.True(r.IsSuccess, r.ErrorMessage);
        }

        [Test]
        public void Plaintext_Configuration()
        {
            if (MonoUtil.IsRunningOnMono)
            {
                Assert.Ignore("Mono");
            }

            var config = RiakClusterConfiguration.LoadFromConfig("riakPlaintextConfiguration");
            var cluster = new RiakCluster(config);
            var client = cluster.CreateClient();
            var r = client.Ping();
            Assert.True(r.IsSuccess, r.ErrorMessage);
        }
    }
}
