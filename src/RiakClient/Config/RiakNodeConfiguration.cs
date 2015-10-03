namespace RiakClient.Config
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a configuration element for a Riak Node.
    /// </summary>
    public sealed class RiakNodeConfiguration : ConfigurationElement, IRiakNodeConfiguration
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(4000);

        /// <inheritdoc/>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("hostAddress", IsRequired = true)]
        public string HostAddress
        {
            get { return (string)this["hostAddress"]; }
            set { this["hostAddress"] = value; }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 8087 if omitted from the configuration file.</remarks>
        [ConfigurationProperty("pbcPort", DefaultValue = 8087, IsRequired = false)]
        public int PbcPort
        {
            get { return (int)this["pbcPort"]; }
            set { this["pbcPort"] = value; }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 30 if omitted from the configuration file.</remarks>
        [ConfigurationProperty("poolSize", DefaultValue = 30, IsRequired = false)]
        public int PoolSize
        {
            get { return (int)this["poolSize"]; }
            set { this["poolSize"] = value; }
        }

        /*
         * TODO: put this back in when we've got the idling stuff figured out
         * [ConfigurationProperty("idleTimeout", DefaultValue = 15000, IsRequired = false)]
         * public int IdleTimeout
         * {
         *     get { return (int)this["idleTimeout"]; }
         *     set { this["idleTimeout"] = value; }
         * }
         */

        /// <inheritdoc/>
        /// <remarks>Defaults to 4000ms if omitted from the configuration file.</remarks>
        public TimeSpan NetworkReadTimeout
        {
            get
            {
                int networkReadTimeoutMilliseconds;
                if (int.TryParse(NetworkReadTimeoutProperty, out networkReadTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkReadTimeoutMilliseconds);
                }
                else
                {
                    return DefaultTimeout;
                }
            }

            set
            {
                this.NetworkReadTimeoutProperty = value.ToString();
            }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 4000ms if omitted from the configuration file.</remarks>
        public TimeSpan NetworkWriteTimeout
        {
            get
            {
                int networkWriteTimeoutMilliseconds;
                if (int.TryParse(this.NetworkWriteTimeoutProperty, out networkWriteTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkWriteTimeoutMilliseconds);
                }
                else
                {
                    return DefaultTimeout;
                }
            }

            set
            {
                this.NetworkWriteTimeoutProperty = value.ToString();
            }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 4000ms if omitted from the configuration file.</remarks>
        public TimeSpan NetworkConnectTimeout
        {
            get
            {
                int networkConnectTimeoutMilliseconds;
                if (int.TryParse(this.NetworkConnectTimeoutProperty, out networkConnectTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkConnectTimeoutMilliseconds);
                }
                else
                {
                    return DefaultTimeout;
                }
            }

            set
            {
                this.NetworkConnectTimeoutProperty = value.ToString();
            }
        }

        [ConfigurationProperty("networkReadTimeout", DefaultValue = "4000", IsRequired = false)]
        private string NetworkReadTimeoutProperty
        {
            get { return (string)this["networkReadTimeout"]; }
            set { this["networkReadTimeout"] = value; }
        }

        [ConfigurationProperty("networkWriteTimeout", DefaultValue = "4000", IsRequired = false)]
        private string NetworkWriteTimeoutProperty
        {
            get { return (string)this["networkWriteTimeout"]; }
            set { this["networkWriteTimeout"] = value; }
        }

        [ConfigurationProperty("networkConnectTimeout", DefaultValue = "4000", IsRequired = false)]
        private string NetworkConnectTimeoutProperty
        {
            get { return (string)this["networkConnectTimeout"]; }
            set { this["networkConnectTimeout"] = value; }
        }
    }
}
