// <copyright file="CertificateTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

using NUnit.Framework;
using RiakClient.Config;

namespace RiakClientTests.Auth
{
    [TestFixture]
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
