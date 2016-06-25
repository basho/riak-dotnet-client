namespace Riak.Config
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents the configuration file section for a Riak Cluster.
    /// </summary>
    public sealed class ClusterConfiguration : ConfigurationSection, IClusterConfiguration
    {
        private static readonly TimeSpan DefaultNodePollTime = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan DefaultDefaultRetryWaitTime = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// A collection of <see cref="INodeConfiguration"/> configurations detailing the Riak nodes that can be connected to.
        /// </summary>
        [ConfigurationProperty("nodes", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(NodeConfigurationCollection), AddItemName = "node")]
        public NodeConfigurationCollection Nodes
        {
            get { return (NodeConfigurationCollection)this["nodes"]; }
            set { this["nodes"] = value; }
        }

        /// <inheritdoc/>
        IEnumerable<INodeConfiguration> IClusterConfiguration.RiakNodes
        {
            get { return this.Nodes.Cast<INodeConfiguration>().ToArray(); }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 5000ms if omitted from the configuration file.</remarks>
        public TimeSpan NodePollTime
        {
            get
            {
                int nodePollTimeMilliseconds;
                if (int.TryParse(this.NodePollTimeProperty, out nodePollTimeMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(nodePollTimeMilliseconds);
                }
                else
                {
                    return DefaultNodePollTime;
                }
            }

            set
            {
                this.NodePollTimeProperty = value.ToString();
            }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 200ms if omitted from the configuration file.</remarks>
        public TimeSpan DefaultRetryWaitTime
        {
            get
            {
                int defaultRetryWaitTimeMilliseconds;
                if (int.TryParse(this.DefaultRetryWaitTimeProperty, out defaultRetryWaitTimeMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(defaultRetryWaitTimeMilliseconds);
                }
                else
                {
                    return DefaultDefaultRetryWaitTime;
                }
            }

            set
            {
                this.DefaultRetryWaitTimeProperty = value.ToString();
            }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 3 if omitted from the configuration file.</remarks>
        [ConfigurationProperty("defaultRetryCount", DefaultValue = 3, IsRequired = false)]
        public int DefaultRetryCount
        {
            get { return (int)this["defaultRetryCount"]; }
            set { this["defaultRetryCount"] = value; }
        }

        /// <summary>
        /// A <see cref="IAuthenticationConfiguration"/> configuration that details any authentication information.
        /// </summary>
        [ConfigurationProperty("authentication", IsRequired = false)]
        public AuthenticationConfiguration Authentication
        {
            get { return (AuthenticationConfiguration)this["authentication"]; }
            set { this["authentication"] = value; }
        }

        /// <inheritdoc/>
        IAuthenticationConfiguration IClusterConfiguration.Authentication
        {
            get { return this.Authentication; }
            set { this.Authentication = (AuthenticationConfiguration)value; }
        }

        [ConfigurationProperty("defaultRetryWaitTime", DefaultValue = "200", IsRequired = false)]
        private string DefaultRetryWaitTimeProperty
        {
            get { return (string)this["defaultRetryWaitTime"]; }
            set { this["defaultRetryWaitTime"] = value; }
        }

        [ConfigurationProperty("nodePollTime", DefaultValue = "5000", IsRequired = false)]
        private string NodePollTimeProperty
        {
            get { return (string)this["nodePollTime"]; }
            set { this["nodePollTime"] = value; }
        }

        /// <summary>
        /// Load a <see cref="ClusterConfiguration"/> from the local configuration file,
        /// and return a new <see cref="IClusterConfiguration"/>. 
        /// </summary>
        /// <param name="sectionName">The section to load the configuration from.</param>
        /// <returns>An initialized and configured <see cref="IClusterConfiguration"/>.</returns>
        public static IClusterConfiguration LoadFromConfig(string sectionName)
        {
            return (IClusterConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        /// <summary>
        /// Load a <see cref="ClusterConfiguration"/> from a specified configuration file,
        /// and return a new <see cref="IClusterConfiguration"/>.
        /// </summary>
        /// <param name="sectionName">The section to load the configuration from.</param>
        /// <param name="fileName">The file containing the configuration section.</param>
        /// <returns>An initialized and configured <see cref="IClusterConfiguration"/>.</returns>
        public static IClusterConfiguration LoadFromConfig(string sectionName, string fileName)
        {
            var map = new ConfigurationFileMap(fileName);
            var config = ConfigurationManager.OpenMappedMachineConfiguration(map);
            return (IClusterConfiguration)config.GetSection(sectionName);
        }

        /// <inheritdoc/>
        void IClusterConfiguration.AddNode(INodeConfiguration nodeConfiguration)
        {
            this.Nodes.Add((NodeConfiguration)nodeConfiguration);
        }
    }
}
