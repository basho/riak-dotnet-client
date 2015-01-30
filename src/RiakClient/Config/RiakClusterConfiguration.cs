// <copyright file="RiakClusterConfiguration.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RiakClient.Config
{
    public class RiakClusterConfiguration : ConfigurationSection, IRiakClusterConfiguration
    {
        private static TimeSpan s_defaultNodePollTime = TimeSpan.FromSeconds(5);
        private static TimeSpan s_defaultRetryWaitTime = TimeSpan.FromMilliseconds(200);

        public static IRiakClusterConfiguration LoadFromConfig(string sectionName)
        {
            return (IRiakClusterConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        public static IRiakClusterConfiguration LoadFromConfig(string sectionName, string fileName)
        {
            var map = new ConfigurationFileMap(fileName);
            var config = ConfigurationManager.OpenMappedMachineConfiguration(map);
            return (IRiakClusterConfiguration)config.GetSection(sectionName);
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
            get { return this.Nodes.Cast<IRiakNodeConfiguration>().ToList(); }
        }

        [ConfigurationProperty("nodePollTime", DefaultValue = "5000", IsRequired = false)]
        private string NodePollTimeProperty
        {
            get { return (string)this["nodePollTime"]; }
            set { this["nodePollTime"] = value; }
        }

        public TimeSpan NodePollTime
        {
            get
            {
                int nodePollTimeMilliseconds;
                if (int.TryParse(NodePollTimeProperty, out nodePollTimeMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(nodePollTimeMilliseconds);
                }
                else
                {
                    return s_defaultNodePollTime;
                }
            }
        }

        [ConfigurationProperty("defaultRetryWaitTime", DefaultValue = "200", IsRequired = false)]
        private string DefaultRetryWaitTimeProperty
        {
            get { return (string)this["defaultRetryWaitTime"]; }
            set { this["defaultRetryWaitTime"] = value; }
        }

        public TimeSpan DefaultRetryWaitTime
        {
            get
            {
                int defaultRetryWaitTimeMilliseconds;
                if (int.TryParse(DefaultRetryWaitTimeProperty, out defaultRetryWaitTimeMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(defaultRetryWaitTimeMilliseconds);
                }
                else
                {
                    return s_defaultRetryWaitTime;
                }
            }
        }

        [ConfigurationProperty("defaultRetryCount", DefaultValue = 3, IsRequired = false)]
        public int DefaultRetryCount
        {
            get { return (int)this["defaultRetryCount"]; }
            set { this["defaultRetryCount"] = value; }
        }

        [ConfigurationProperty("authentication", IsRequired = false)]
        public RiakAuthenticationConfiguration Authentication
        {
            get { return (RiakAuthenticationConfiguration)this["authentication"]; }
        }

        IRiakAuthenticationConfiguration IRiakClusterConfiguration.Authentication
        {
            get { return this.Authentication; }
        }
    }
}
