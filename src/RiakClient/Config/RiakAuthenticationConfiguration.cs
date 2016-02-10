namespace RiakClient.Config
{
    using System.Configuration;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a configuration file element for configuring Riak Authentication.
    /// </summary>
    public class RiakAuthenticationConfiguration : ConfigurationElement, IRiakAuthenticationConfiguration
    {
        /// <inheritdoc/>
        [ConfigurationProperty("username", DefaultValue = "", IsRequired = true)]
        public string Username
        {
            get { return (string)this["username"]; }
            set { this["username"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("clientCertificateFile", DefaultValue = "", IsRequired = false)]
        public string ClientCertificateFile
        {
            get { return (string)this["clientCertificateFile"]; }
            set { this["clientCertificateFile"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("clientCertificateSubject", DefaultValue = "", IsRequired = false)]
        public string ClientCertificateSubject
        {
            get { return (string)this["clientCertificateSubject"]; }
            set { this["clientCertificateSubject"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("certificateAuthorityFile", DefaultValue = "", IsRequired = false)]
        public string CertificateAuthorityFile
        {
            get { return (string)this["certificateAuthorityFile"]; }
            set { this["certificateAuthorityFile"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("checkCertificateRevocation", DefaultValue = false, IsRequired = false)]
        public bool CheckCertificateRevocation
        {
            get { return (bool)this["checkCertificateRevocation"]; }
            set { this["checkCertificateRevocation"] = value; }
        }
    }
}
