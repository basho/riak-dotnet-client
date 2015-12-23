namespace Riak.Config
{
    using System.Configuration;

    /// <summary>
    /// Represents a configuration element containing a collection of <see cref="NodeConfiguration"/>s.
    /// </summary>
    public sealed class NodeConfigurationCollection : ConfigurationElementCollection
    {
        public void Add(NodeConfiguration nodeConfig)
        {
            this.BaseAdd(nodeConfig);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new NodeConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NodeConfiguration)element).Name;
        }
    }
}
