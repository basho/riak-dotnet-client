namespace Riak.Core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using RiakClient.Config;
    using RiakClient.Extensions;
    using RiakClient.Messages;
    using RiakClient.Util;

    internal class SecurityManager
    {
        private static readonly StoreLocation[] StoreLocations =
            new StoreLocation[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };

        private static readonly string[] SubjectSplit = new[] { ", " };

        private readonly string targetHostCommonName;
        private readonly IRiakAuthenticationConfiguration authConfig;
        private readonly X509CertificateCollection clientCertificates;
        private readonly X509Certificate2 certificateAuthorityCert;

        // Interesting discussion:
        // http://stackoverflow.com/questions/3780801/whats-the-difference-between-a-public-constructor-in-an-internal-class-and-an-i
        // http://stackoverflow.com/questions/9302236/why-use-a-public-method-in-an-internal-class
        internal SecurityManager(string targetHost, IRiakAuthenticationConfiguration authConfig)
        {
            if (string.IsNullOrWhiteSpace(targetHost))
            {
                throw new ArgumentNullException("targetHost");
            }

            targetHostCommonName = string.Format("CN={0}", targetHost);
            this.authConfig = authConfig;

            if (IsSecurityEnabled)
            {
                clientCertificates = GetClientCertificates();
                certificateAuthorityCert = GetCertificateAuthorityCert();
            }
        }

        /// <summary>
        /// Gets a value indicating whether security is enabled
        /// </summary>
        public bool IsSecurityEnabled
        {
            get
            {
                return (false == MonoUtil.IsRunningOnMono) &&
                       (authConfig != null && (!string.IsNullOrWhiteSpace(authConfig.Username)));
            }
        }

        /// <summary>
        /// Gets a value indicating whether client certs are configured and at least one available
        /// </summary>
        public bool ClientCertificatesConfigured
        {
            get { return clientCertificates.Count > 0; }
        }

        /// <summary>
        /// Gets the client certs collection
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get { return clientCertificates; }
        }

        /// <summary>
        /// Method used to validate a server certificate
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="certificate">The server certificate</param>
        /// <param name="chain">The X509 certificate chain</param>
        /// <param name="sslPolicyErrors">The set of errors according to SSL policy</param>
        /// <returns>boolean indicating validity of server certificate</returns>
        public bool ServerCertificateValidationCallback(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            /*
             * Inspired by the following:
             * http://msdn.microsoft.com/en-us/library/office/dd633677%28v=exchg.80%29.aspx
             * http://stackoverflow.com/questions/22076184/how-to-validate-a-certificate
             * 
             * First, ensure we've got a cert authority file
             */
            if (certificateAuthorityCert != null)
            {
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                {
                    // This ensures the presented cert is for the current host
                    if (EnsureServerCertificateSubject(certificate.Subject))
                    {
                        if (chain != null && chain.ChainStatus != null)
                        {
                            foreach (X509ChainStatus status in chain.ChainStatus)
                            {
                                if (status.Status == X509ChainStatusFlags.UntrustedRoot &&
                                    EnumerableUtil.NotNullOrEmpty(chain.ChainElements))
                                {
                                    // The root cert must not be installed but we provided a file
                                    // See if anything in the chain matches our root cert
                                    foreach (X509ChainElement chainElement in chain.ChainElements)
                                    {
                                        if (chainElement.Certificate.Equals(certificateAuthorityCert))
                                        {
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (status.Status != X509ChainStatusFlags.NoError)
                                    {
                                        // If there are any other errors in the certificate chain, the certificate is invalid,
                                        // so immediately returns false.
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Callback to select a client certificate for authentication
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="targetHost">The host requesting authentication</param>
        /// <param name="localCertificates">The collection of local certificates</param>
        /// <param name="remoteCertificate">The remote certificate</param>
        /// <param name="acceptableIssuers">The collection of acceptable issuers</param>
        /// <returns>A matching certificate for authentication</returns>
        public X509Certificate ClientCertificateSelectionCallback(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            X509Certificate clientCertToPresent = null;

            /*
             * NB:
             * 1st time in here, targetHost == "riak-test" and acceptableIssuers is empty
             * 2nd time in here, targetHost == "riak-test" and acceptableIssues is one element in array:
             *     OU=Development, O=Basho Technologies, L=Bellevue, S=WA, C=US
             */
            if (EnumerableUtil.NotNullOrEmpty(localCertificates))
            {
                if (EnumerableUtil.NotNullOrEmpty(acceptableIssuers))
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

        /// <summary>
        /// Gets the protobuf object for an authentication request
        /// </summary>
        /// <returns>A correctly constructed protobuf object</returns>
        public RpbAuthReq GetAuthRequest()
        {
            return new RpbAuthReq
                {
                    user = authConfig.Username.ToRiakString(),
                    password = authConfig.Password.ToRiakString()
                };
        }

        /// <summary>
        /// Ensures that the server certificate is for the target host
        /// </summary>
        /// <param name="serverCertificateSubject">The presented subject</param>
        /// <returns>boolean indicating validity</returns>
        private bool EnsureServerCertificateSubject(string serverCertificateSubject)
        {
            string serverCommonName =
                serverCertificateSubject.Split(SubjectSplit, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(s => s.StartsWith("CN="));
            return targetHostCommonName.Equals(serverCommonName);
        }

        /// <summary>
        /// Gets a file containing the certificate authority certificate
        /// </summary>
        /// <returns>An <see cref="X509Certificate2"/> object</returns>
        private X509Certificate2 GetCertificateAuthorityCert()
        {
            X509Certificate2 certificateAuthorityCert = null;

            if (!string.IsNullOrWhiteSpace(authConfig.CertificateAuthorityFile) && File.Exists(authConfig.CertificateAuthorityFile))
            {
                certificateAuthorityCert = new X509Certificate2(authConfig.CertificateAuthorityFile);
            }

            return certificateAuthorityCert;
        }

        /// <summary>
        /// Returns a collection of client certificates from the configuration setting and local stores
        /// </summary>
        /// <returns>Returns <see cref="X509CertificateCollection"/> representing available client certificates</returns>
        private X509CertificateCollection GetClientCertificates()
        {
            var clientCertificates = new X509CertificateCollection();

            // http://stackoverflow.com/questions/18462064/associate-a-private-key-with-the-x509certificate2-class-in-net
            if (!string.IsNullOrWhiteSpace(authConfig.ClientCertificateFile))
            {
                if (File.Exists(authConfig.ClientCertificateFile))
                {
                    var cert = new X509Certificate2(authConfig.ClientCertificateFile);
                    clientCertificates.Add(cert);
                }
                else
                {
                    throw new FileNotFoundException(Riak.Properties.Resources.Riak_Core_Auth_SecurityManager_ClientCertificateFileNotFound, authConfig.ClientCertificateFile);
                }
            }

            if (!string.IsNullOrWhiteSpace(authConfig.ClientCertificateSubject))
            {
                foreach (var storeLocation in StoreLocations)
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

            // TODO 3.0 FUTURE exception if expected to get certs but count is 0 here
            return clientCertificates;
        }
    }
}
