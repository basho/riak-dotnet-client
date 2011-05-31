using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CorrugatedIron.Config
{
    public interface IRiakClusterConfiguration
    {
        IList<IRiakNodeConfiguration> RiakNodes { get; }
    }

    public class RiakClusterConfiguration : ConfigurationSection, IRiakClusterConfiguration
    {
        public static IRiakClusterConfiguration LoadFromConfig(string sectionName)
        {
            return (IRiakClusterConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        [ConfigurationProperty("nodes", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(RiakNodeConfigurationCollection), AddItemName = "node")]
        public RiakNodeConfigurationCollection Nodes
        {
            get { return (RiakNodeConfigurationCollection)this["nodes"]; }
            set { this["nodes"] = value; }
        }

        IList<IRiakNodeConfiguration> IRiakClusterConfiguration.RiakNodes
        {
            get { return Nodes.Cast<IRiakNodeConfiguration>().ToList(); }
        }
    }
}
