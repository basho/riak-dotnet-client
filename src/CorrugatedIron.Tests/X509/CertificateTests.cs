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

namespace CorrugatedIron.Tests.X509
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;
    using CorrugatedIron.Util;
    using NUnit.Framework;

    [TestFixture]
    public class CertificateTests
    {
        [TestFixtureSetUp]
        public void ImportTestCertificates()
        {
            if (MonoUtil.IsRunningOnMono)
            {
                Assert.Ignore("Running on Mono, X509 Certificate tests will be skipped.");
            }

            var currentDir = Environment.CurrentDirectory;
            string testCertsDir = Path.Combine(currentDir, "..", "..", "..", "..", "tools", "test-ca", "certs");
            Assert.True(Directory.Exists(testCertsDir));

            /*
             * NB: the first time this is run, the user WILL get a popup asking if they should import the root
             *     cert. There is no way around this if the user is running as a normal user.
             */
            string rootCaCertFile = Path.Combine(testCertsDir, "cacert.pem");
            Assert.True(File.Exists(rootCaCertFile));
            var rootCaCert = new X509Certificate2(rootCaCertFile);

            string riakUserClientCertFile = Path.Combine(testCertsDir, "riakuser-client-cert.pfx");
            Assert.True(File.Exists(riakUserClientCertFile));
            var riakUserClientCert = new X509Certificate2(riakUserClientCertFile);

            SaveToStore(rootCaCert, StoreName.Root);
            SaveToStore(riakUserClientCert, StoreName.My);
        }

        [Test]
        public void RiakConfigurationCanSpecifyX509Certificates()
        {
        }

        private static void SaveToStore(X509Certificate2 cert, StoreName storeName)
        {
            X509Store x509Store = null;
            try
            {
                x509Store = new X509Store(storeName, StoreLocation.CurrentUser);
                x509Store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadWrite);
                x509Store.Add(cert);
            }
            finally
            {
                x509Store.Close();
            }
        }
    }
}