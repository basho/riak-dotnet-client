namespace RiakClientTests.Auth
{
    using NUnit.Framework;
    using RiakClient.Config;

    [TestFixture, UnitTest]
    public class CertificateTests : AuthTestBase
    {
        [Test]
        public void Configuration_CanSpecifyX509Certificates()
        {
            var config = RiakClusterConfiguration.LoadFromConfig("riakConfiguration");
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
            var config = RiakClusterConfiguration.LoadFromConfig("riakCAConfiguration");
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
