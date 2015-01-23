// Copyright (c) 2015 Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Security.Cryptography.X509Certificates;
using CorrugatedIron.Auth;
using CorrugatedIron.Config;
using CorrugatedIron.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Auth
{
    [TestFixture]
    public class RiakSecurityManagerTests : AuthTestBase
    {
        private readonly IRiakClusterConfiguration clusterConfig;
        private readonly IRiakClusterConfiguration noAuthClusterConfig;
        private readonly IRiakClusterConfiguration certSubjectOnlyClusterConfig;

        public RiakSecurityManagerTests()
        {
            clusterConfig = RiakClusterConfiguration.LoadFromConfig("riakConfiguration");
            Assert.IsNotNull(clusterConfig, "riakConfiguration is not present?");
            Assert.IsNotNull(clusterConfig.Authentication, "Authentication should NOT be null");

            noAuthClusterConfig = RiakClusterConfiguration.LoadFromConfig("riakNoAuthConfiguration");
            Assert.IsNotNull(noAuthClusterConfig, "riakNoAuthConfiguration is not present?");
            Assert.IsNotNull(noAuthClusterConfig.Authentication, "Authentication should NOT be null");

            certSubjectOnlyClusterConfig = RiakClusterConfiguration.LoadFromConfig("riakCertSubjectOnlyConfiguration");
            Assert.IsNotNull(certSubjectOnlyClusterConfig, "riakCertSubjectOnlyConfiguration is not present?");
            Assert.IsNotNull(certSubjectOnlyClusterConfig.Authentication, "Authentication should NOT be null");
        }


        [Test]
        public void WhenSecurityNotConfiguredInAppConfig_SecurityManagerIndicatesIt()
        {
            var authConfig = noAuthClusterConfig.Authentication;
            var securityManager = new RiakSecurityManager("riak-test", authConfig);
            Assert.IsFalse(securityManager.IsSecurityEnabled);
        }

        [Test]
        public void WhenSecurityConfiguredInAppConfig_SecurityManagerIndicatesIt()
        {
            var authConfig = clusterConfig.Authentication;
            var securityManager = new RiakSecurityManager("riak-test", authConfig);
            Assert.IsTrue(securityManager.IsSecurityEnabled);
        }

        [Test]
        public void WhenClientCertificateFileIsConfigured_ItIsPartOfCertificatesCollection()
        {
            var authConfig = clusterConfig.Authentication;
            var securityManager = new RiakSecurityManager("riak-test", authConfig);
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
            var securityManager = new RiakSecurityManager("riak-test", authConfig);
            Assert.True(securityManager.ClientCertificatesConfigured);
            Assert.False(authConfig.ClientCertificateSubject.IsNullOrEmpty());

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
