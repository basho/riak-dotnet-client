namespace RiakClientTests.Auth
{
    using NUnit.Framework;
    using Riak.Config;

    [TestFixture, UnitTest]
    public class CertificateTests : AuthTestBase
    {
        [Test]
        public void Configuration_CanSpecifyX509Certificates()
        {
            var config = ClusterConfiguration.LoadFromConfig("riakConfiguration");
            Assert.IsNotNull(config);

            var authConfig = config.Authentication;
            Assert.IsNotNull(authConfig);

            Assert.AreEqual("riakuser", authConfig.Username);
            Assert.IsNullOrEmpty(authConfig.Password);
            Assert.AreEqual(riakUserClientCertFileRelativePath, authConfig.ClientCertificateFile);
            Assert.AreEqual(riakUserClientCertSubject, authConfig.ClientCertificateSubject);
        }

        [Test]
        public void Configuration_CanSpecifyX509CertificateAndRootCA()
        {
            var config = ClusterConfiguration.LoadFromConfig("riakCAConfiguration");
            Assert.IsNotNull(config);

            var authConfig = config.Authentication;
            Assert.IsNotNull(authConfig);

            Assert.AreEqual("riakuser", authConfig.Username);
            Assert.IsNullOrEmpty(authConfig.Password);
            Assert.AreEqual(riakUserClientCertSubject, authConfig.ClientCertificateSubject);
            Assert.AreEqual(rootCaCertFileRelativePath, authConfig.CertificateAuthorityFile);
        }
    }
}
