using System.Configuration;

namespace CorrugatedIron.Config
{
    public interface IRiakNodeConfiguration
    {
        string Name { get; }
        string HostAddress { get; }
        int PbcPort { get; }
        int HttpPort { get; }
        int PoolSize { get; }
        int AcquireTimeout { get; }
        int IdleTimeout { get; }
    }

    public class RiakNodeConfiguration : ConfigurationElement, IRiakNodeConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("hostAddress", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string HostAddress
        {
            get { return (string)this["hostAddress"]; }
            set { this["hostAddress"] = value; }
        }

        [ConfigurationProperty("pbcPort", DefaultValue = 8088, IsRequired = false)]
        public int PbcPort
        {
            get { return (int)this["pbcPort"]; }
            set { this["pbcPort"] = value; }
        }

        [ConfigurationProperty("httpPort", DefaultValue = 8098, IsRequired = false)]
        public int HttpPort
        {
            get { return (int)this["httpPort"]; }
            set { this["httpPort"] = value; }
        }

        [ConfigurationProperty("poolSize", DefaultValue = 30, IsRequired = false)]
        public int PoolSize
        {
            get { return (int)this["poolSize"]; }
            set { this["poolSize"] = value; }
        }

        [ConfigurationProperty("acquireTimeout", DefaultValue = 5000, IsRequired = false)]
        public int AcquireTimeout
        {
            get { return (int)this["acquireTimeout"]; }
            set { this["acquireTimeout"] = value; }
        }

        [ConfigurationProperty("idleTimeout", DefaultValue = 15000, IsRequired = false)]
        public int IdleTimeout
        {
            get { return (int)this["idleTimeout"]; }
            set { this["idleTimeout"] = value; }
        }
    }
}
