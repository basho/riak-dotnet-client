// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2015 - Basho Technologies, Inc.
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

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CorrugatedIron.Config;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Auth
{
    internal class RiakSecurityManager
    {
        private static readonly StoreLocation[] storeLocations =
            new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };
        private readonly IRiakAuthenticationConfiguration authConfig;
        private readonly X509CertificateCollection clientCertificates;

        // Interesting discussion:
        // http://stackoverflow.com/questions/3780801/whats-the-difference-between-a-public-constructor-in-an-internal-class-and-an-i
        // http://stackoverflow.com/questions/9302236/why-use-a-public-method-in-an-internal-class
        internal RiakSecurityManager(IRiakAuthenticationConfiguration authConfig)
        {
            this.authConfig = authConfig;
            this.clientCertificates = GetClientCertificates();
        }

        public bool IsSecurityEnabled
        {
            get
            {
                return (authConfig != null && (false == authConfig.Username.IsNullOrEmpty()));
            }
        }

        public bool ClientCertificatesConfigured
        {
            get { return clientCertificates.Count > 0; }
        }

        public X509CertificateCollection ClientCertificates
        {
            get { return clientCertificates; }
        }

        public bool ServerCertificateValidationCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO
            bool rv = sslPolicyErrors == SslPolicyErrors.None;
            Debug.WriteLine("SslPolicyErrors: {0}", sslPolicyErrors);
            return rv;
        }

        public X509Certificate ClientCertificateSelectionCallback(object sender, string targetHost,
            X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            X509Certificate clientCertToPresent = null;

            /*
             * NB:
             * 1st time in here, targetHost == "riak-test" and acceptableIssuers is empty
             * 2nd time in here, targetHost == "riak-test" and acceptableIssues is one element in array:
             *     OU=Development, O=Basho Technologies, L=Bellevue, S=WA, C=US
             */
            if (false == localCertificates.IsNullOrEmpty())
            {
                if (false == acceptableIssuers.IsNullOrEmpty())
                {
                    foreach (X509Certificate cert in localCertificates)
                    {
                        if (acceptableIssuers.Any(issuer => cert.Issuer.Contains(issuer)))
                        {
                            clientCertToPresent = cert;
                            break;
                        }
                    }
                }

                if (clientCertToPresent == null)
                {
                    // Hope that this cert is the right one
                    clientCertToPresent = localCertificates[0];
                }
            }

            return clientCertToPresent;
        }

        public RpbAuthReq GetAuthRequest()
        {
            var userBytes = Encoding.ASCII.GetBytes(authConfig.Username);
            var passBytes = Encoding.ASCII.GetBytes(authConfig.Password);
            return new RpbAuthReq
                {
                    user = userBytes,
                    password = passBytes
                };
        }

        private X509CertificateCollection GetClientCertificates()
        {
            var clientCertificates = new X509CertificateCollection();

            // TODO private key file vs pfx? Check for .pfx extension?
            // http://stackoverflow.com/questions/18462064/associate-a-private-key-with-the-x509certificate2-class-in-net
            if ((false == authConfig.ClientCertificateFile.IsNullOrEmpty()) &&
                (File.Exists(authConfig.ClientCertificateFile)))
            {
                var cert = new X509Certificate2(authConfig.ClientCertificateFile);
                clientCertificates.Add(cert);
            }

            if (false == authConfig.ClientCertificateSubject.IsNullOrEmpty())
            {
                foreach (var storeLocation in storeLocations)
                {
                    X509Store x509Store = null;
                    try
                    {
                        x509Store = new X509Store(StoreName.My, storeLocation);
                        x509Store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                        foreach (var cert in x509Store.Certificates)
                        {
                            if (cert.Subject == authConfig.ClientCertificateSubject)
                            {
                                clientCertificates.Add(cert);
                            }
                        }
                    }
                    finally
                    {
                        x509Store.Close();
                    }
                }
            }

            return clientCertificates;
        }
    }
}
