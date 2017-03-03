// <copyright file="RiakClusterConfiguration.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace RiakClient.Config
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// Represents the configuration file section for a Riak Cluster.
    /// </summary>
    public sealed class RiakClusterConfiguration : ConfigurationSection, IRiakClusterConfiguration
    {
        private static readonly Timeout DefaultNodePollTime = new Timeout(TimeSpan.FromSeconds(5));
        private static readonly Timeout DefaultDefaultRetryWaitTime = new Timeout(200);

        /// <summary>
        /// A collection of <see cref="IRiakNodeConfiguration"/> configurations detailing the Riak nodes that can be connected to.
        /// </summary>
        [ConfigurationProperty("nodes", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(RiakNodeConfigurationCollection), AddItemName = "node")]
        public RiakNodeConfigurationCollection Nodes
        {
            get { return (RiakNodeConfigurationCollection)this["nodes"]; }
            set { this["nodes"] = value; }
        }

        /// <inheritdoc/>
        IEnumerable<IRiakNodeConfiguration> IRiakClusterConfiguration.RiakNodes
        {
            get { return this.Nodes.Cast<IRiakNodeConfiguration>().ToArray(); }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to 5000ms if omitted from the configuration file.</remarks>
        public Timeout NodePollTime
        {
            get
            {
                int nodePollTimeMilliseconds;
                if (int.TryParse(this.NodePollTimeProperty, out nodePollTimeMilliseconds))
                {
                    return new Timeout(nodePollTimeMilliseconds);
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
        public Timeout DefaultRetryWaitTime
        {
            get
            {
                int defaultRetryWaitTimeMilliseconds;
                if (int.TryParse(this.DefaultRetryWaitTimeProperty, out defaultRetryWaitTimeMilliseconds))
                {
                    return new Timeout(defaultRetryWaitTimeMilliseconds);
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

        /// <inheritdoc/>
        /// <remarks>Defaults to <b>true</b> if omitted from the configuration file.</remarks>
        [ConfigurationProperty("useTtbEncoding", DefaultValue = true, IsRequired = false)]
        public bool UseTtbEncoding
        {
            get { return (bool)this["useTtbEncoding"]; }
            set { this["useTtbEncoding"] = value; }
        }

        /// <inheritdoc/>
        /// <remarks>Defaults to <b>false</b> if omitted from the configuration file.</remarks>
        [ConfigurationProperty("disableListExceptions", DefaultValue = false, IsRequired = false)]
        public bool DisableListExceptions
        {
            get { return (bool)this["disableListExceptions"]; }
            set { this["disableListExceptions"] = value; }
        }

        /// <summary>
        /// A <see cref="IRiakAuthenticationConfiguration"/> configuration that details any authentication information.
        /// </summary>
        [ConfigurationProperty("authentication", IsRequired = false)]
        public RiakAuthenticationConfiguration Authentication
        {
            get { return (RiakAuthenticationConfiguration)this["authentication"]; }
            set { this["authentication"] = value; }
        }

        /// <inheritdoc/>
        IRiakAuthenticationConfiguration IRiakClusterConfiguration.Authentication
        {
            get { return this.Authentication; }
            set { this.Authentication = (RiakAuthenticationConfiguration)value; }
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
        /// Load a <see cref="RiakClusterConfiguration"/> from the local configuration file,
        /// and return a new <see cref="IRiakClusterConfiguration"/>. 
        /// </summary>
        /// <param name="sectionName">The section to load the configuration from.</param>
        /// <returns>An initialized and configured <see cref="IRiakClusterConfiguration"/>.</returns>
        public static IRiakClusterConfiguration LoadFromConfig(string sectionName)
        {
            return (IRiakClusterConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        /// <summary>
        /// Load a <see cref="RiakClusterConfiguration"/> from a specified configuration file,
        /// and return a new <see cref="IRiakClusterConfiguration"/>.
        /// </summary>
        /// <param name="sectionName">The section to load the configuration from.</param>
        /// <param name="fileName">The file containing the configuration section.</param>
        /// <returns>An initialized and configured <see cref="IRiakClusterConfiguration"/>.</returns>
        public static IRiakClusterConfiguration LoadFromConfig(string sectionName, string fileName)
        {
            var map = new ConfigurationFileMap(fileName);
            var config = ConfigurationManager.OpenMappedMachineConfiguration(map);
            return (IRiakClusterConfiguration)config.GetSection(sectionName);
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        /// <inheritdoc/>
        void IRiakClusterConfiguration.AddNode(IRiakNodeConfiguration nodeConfiguration)
        {
            Nodes.Add((RiakNodeConfiguration)nodeConfiguration);
        }
    }
}
