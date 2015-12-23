namespace RiakClientTests.Auth
{
    using System.Security.Cryptography.X509Certificates;
    using NUnit.Framework;
    using Riak.Config;

    [TestFixture, UnitTest]
    public class RiakSecurityManagerTests : AuthTestBase
    {
        private readonly IClusterConfiguration clusterConfig;
        private readonly IClusterConfiguration noAuthClusterConfig;
        private readonly IClusterConfiguration certSubjectOnlyClusterConfig;

        public RiakSecurityManagerTests()
        {
            clusterConfig = ClusterConfiguration.LoadFromConfig("riakConfiguration");
            Assert.IsNotNull(clusterConfig, "riakConfiguration is not present?");
            Assert.IsNotNull(clusterConfig.Authentication, "Authentication should NOT be null");

            noAuthClusterConfig = ClusterConfiguration.LoadFromConfig("riakNoAuthConfiguration");
            Assert.IsNotNull(noAuthClusterConfig, "riakNoAuthConfiguration is not present?");
            Assert.IsNotNull(noAuthClusterConfig.Authentication, "Authentication should NOT be null");

            certSubjectOnlyClusterConfig = ClusterConfiguration.LoadFromConfig("riakCertSubjectOnlyConfiguration");
            Assert.IsNotNull(certSubjectOnlyClusterConfig, "riakCertSubjectOnlyConfiguration is not present?");
            Assert.IsNotNull(certSubjectOnlyClusterConfig.Authentication, "Authentication should NOT be null");
        }

        [Test]
        public void WhenSecurityNotConfiguredInAppConfig_SecurityManagerIndicatesIt()
        {
            var authConfig = noAuthClusterConfig.Authentication;
            var securityManager = new Riak.Core.SecurityManager("riak-test", authConfig);
            Assert.IsFalse(securityManager.IsSecurityEnabled);
        }

        [Test]
        public void WhenSecurityConfiguredInAppConfig_SecurityManagerIndicatesIt()
        {
            var authConfig = clusterConfig.Authentication;
            var securityManager = new Riak.Core.SecurityManager("riak-test", authConfig);
            Assert.IsTrue(securityManager.IsSecurityEnabled);
        }

        [Test]
        public void WhenClientCertificateFileIsConfigured_ItIsPartOfCertificatesCollection()
        {
            var authConfig = clusterConfig.Authentication;
            var securityManager = new Riak.Core.SecurityManager("riak-test", authConfig);
            Assert.True(securityManager.ClientCertificatesConfigured);

            var certFromFile = new X509Certificate2(authConfig.ClientCertificateFile);
            Assert.Contains(certFromFile, securityManager.ClientCertificates);
        }

        [Test]
        public void WhenClientCertificateSubjectIsConfigured_ItIsPartOfCertificatesCollection()
        {
            /*
             * Note: the AuthTestBase class ensures that the test client cert is installed
             * to the current user's "My" store
             */
            var authConfig = certSubjectOnlyClusterConfig.Authentication;
            var securityManager = new Riak.Core.SecurityManager("riak-test", authConfig);
            Assert.True(securityManager.ClientCertificatesConfigured);
            Assert.False(string.IsNullOrWhiteSpace(authConfig.ClientCertificateSubject));

            X509Store x509Store = null;
            try
            {
                x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                x509Store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                bool found = false;
                foreach (var cert in x509Store.Certificates)
                {
                    if (cert.Subject == authConfig.ClientCertificateSubject &&
                        securityManager.ClientCertificates.Contains(cert))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found,
                    "Could not find cert with subject '{0}' in CurrentUser/My store!",
                    authConfig.ClientCertificateSubject);
            }
            finally
            {
                x509Store.Close();
            }
        }
    }
}
