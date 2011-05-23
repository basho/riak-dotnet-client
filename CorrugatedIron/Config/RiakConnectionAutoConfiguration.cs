using System.Configuration;

namespace CorrugatedIron.Config
{
    public class RiakConnectionAutoConfiguration : ConfigurationSection, IRiakConnectionConfiguration
    {
        public static IRiakConnectionConfiguration LoadFromConfig(string sectionName)
        {
            return (IRiakConnectionConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        [ConfigurationProperty("hostAddress", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string HostAddress
        {
            get { return (string)this["hostAddress"]; }
            set { this["hostAddress"] = value; }
        }

        [ConfigurationProperty("hostPort", DefaultValue = 8088, IsRequired = false)]
        public int HostPort
        {
            get { return (int)this["hostPort"]; }
            set { this["hostPort"] = value; }
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
