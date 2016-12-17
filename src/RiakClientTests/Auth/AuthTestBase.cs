// <copyright file="AuthTestBase.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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
// </copyright>

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using RiakClient.Util;

namespace RiakClientTests.Auth
{
    public abstract class AuthTestBase
    {
        private static readonly string testCertsDir;
        private static readonly string rootCaCertFile;
        private static readonly string riakUserClientCertFile;

        protected static readonly string rootCaCertFileRelativePath;
        protected static readonly string riakUserClientCertFileRelativePath;
        protected const string riakUserClientCertSubject =
            @"E=riakuser@myorg.com, CN=riakuser, OU=Development, O=Basho Technologies, S=WA, C=US";

        static AuthTestBase()
        {
            var currentDir = Environment.CurrentDirectory;
            string[] testCertsDirRelativePathAry = new string[] { "tools", "test-ca", "certs" };
            string testCertsDirRelativePath = Path.Combine(testCertsDirRelativePathAry);
            testCertsDir = Path.GetFullPath(Path.Combine(currentDir, testCertsDirRelativePath));

            rootCaCertFile = Path.GetFullPath(Path.Combine(testCertsDir, "cacert.pem"));
            rootCaCertFileRelativePath = Path.Combine(testCertsDirRelativePath, "cacert.pem");

            riakUserClientCertFileRelativePath = Path.Combine(testCertsDirRelativePath, "riakuser-client-cert.pfx");
            riakUserClientCertFile = Path.GetFullPath(Path.Combine(currentDir, riakUserClientCertFileRelativePath));
        }

        [OneTimeSetUp]
        public void ImportTestCertificates()
        {
            if (MonoUtil.IsRunningOnMono)
            {
                Assert.Ignore("Running on Mono, X509 Certificate tests will be skipped.");
            }

            Assert.True(Directory.Exists(testCertsDir));

            /*
             * NB: the first time this is run, the user WILL get a popup asking if they should import the root
             *     cert. There is no way around this if the user is running as a normal user.
             */
            Assert.True(File.Exists(rootCaCertFile));
            var rootCaCert = new X509Certificate2(rootCaCertFile);

            Assert.True(File.Exists(riakUserClientCertFile));
            var riakUserClientCert = new X509Certificate2(riakUserClientCertFile);

            SaveToStore(rootCaCert, StoreName.Root);
            SaveToStore(riakUserClientCert, StoreName.My);
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
