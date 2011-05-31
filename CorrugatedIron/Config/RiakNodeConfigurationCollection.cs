using System.Configuration;

namespace CorrugatedIron.Config
{
    public class RiakNodeConfigurationCollection : ConfigurationElementCollection
    {
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
