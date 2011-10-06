// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CorrugatedIron.Config
{
    public interface IRiakClusterConfiguration
    {
        IList<IRiakNodeConfiguration> RiakNodes { get; }
        int NodePollTime { get; }
        int DefaultRetryWaitTime { get; }
        int DefaultRetryCount { get; }
        bool VnodeVclocks { get; }
    }

    public class RiakClusterConfiguration : ConfigurationSection, IRiakClusterConfiguration
    {
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
            get { return Nodes.Cast<IRiakNodeConfiguration>().ToList(); }
        }

        [ConfigurationProperty("nodePollTime", DefaultValue = 5000, IsRequired = false)]
        public int NodePollTime
        {
            get { return (int)this["nodePollTime"]; }
            set { this["nodePollTime"] = value; }
        }

        [ConfigurationProperty("defaultRetryWaitTime", DefaultValue = 200, IsRequired = false)]
        public int DefaultRetryWaitTime
        {
            get { return (int)this["defaultRetryWaitTime"]; }
            set { this["defaultRetryWaitTime"] = value; }
        }

        [ConfigurationProperty("defaultRetryCount", DefaultValue = 3, IsRequired = false)]
        public int DefaultRetryCount
        {
            get { return (int)this["defaultRetryCount"]; }
            set { this["defaultRetryCount"] = value; }
        }
        
        [ConfigurationProperty("vnodeVclocks", DefaultValue = true, IsRequired = false)]
        public bool VnodeVclocks
        {
            get { return (bool)this["vnodeVclocks"]; }
            set
            {
                this["vnodeVclocks"] = value;

                foreach (RiakNodeConfiguration node in Nodes)
                {
                    node.VnodeVclocks = true;
                }
            }
        }
    }
}
