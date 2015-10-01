namespace RiakClient.Config
{
    using System.Configuration;

    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="RiakNodeConfiguration"/>s.
    /// </summary>
    public sealed class RiakNodeConfigurationCollection : ConfigurationElementCollection
    {
        public void Add(RiakNodeConfiguration nodeConfig)
        {
            this.BaseAdd(nodeConfig);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RiakNodeConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RiakNodeConfiguration)element).Name;
        }
    }
}
