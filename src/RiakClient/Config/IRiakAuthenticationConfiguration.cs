namespace RiakClient.Config
{
    /// <summary>
    /// Represents a configuration element interface for Riak authentication.
    /// </summary>
    public interface IRiakAuthenticationConfiguration
    {
        /// <summary>
        /// The username to authenticate with.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The password to authenticate with.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// A client certificate file to load and use.
        /// Must be a valid file path.
        /// </summary>
        string ClientCertificateFile { get; set; }

        /// <summary>
        /// A client certificate subject, used to find and use a certificate from the local store.
        /// </summary>
        string ClientCertificateSubject { get; set; }

        /// <summary>
        /// A client certificate authority certificate to load and use. 
        /// Must be a valid file path.
        /// </summary>
        string CertificateAuthorityFile { get; set; }

        /// <summary>
        /// The option to check the certificate revocation list during authentication.
        /// </summary>
        bool CheckCertificateRevocation { get; set; }
    }
}
