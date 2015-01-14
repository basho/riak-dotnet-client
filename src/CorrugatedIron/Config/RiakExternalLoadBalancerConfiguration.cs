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

using System.Configuration;

namespace CorrugatedIron.Config
{
    public class RiakExternalLoadBalancerConfiguration : ConfigurationSection, IRiakExternalLoadBalancerConfiguration
    {
        public static IRiakExternalLoadBalancerConfiguration LoadFromConfig(string sectionName)
        {
            return (IRiakExternalLoadBalancerConfiguration)ConfigurationManager.GetSection(sectionName);
        }

        public static IRiakExternalLoadBalancerConfiguration LoadFromConfig(string sectionName, string fileName)
        {
            var map = new ConfigurationFileMap(fileName);
            var config = ConfigurationManager.OpenMappedMachineConfiguration(map);
            return (IRiakExternalLoadBalancerConfiguration)config.GetSection(sectionName);
        }

        public IRiakNodeConfiguration Target
        {
            get { return TargetNode; }
        }

        [ConfigurationProperty("target", IsRequired = true)]
        public RiakNodeConfiguration TargetNode
        {
            get { return (RiakNodeConfiguration)this["target"]; }
            set { this["target"] = value; }
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

        // TODO TODO TODO TEST THIS
        [ConfigurationProperty("authentication", IsRequired = false)]
        public RiakAuthenticationConfiguration Authentication
        {
            get { return (RiakAuthenticationConfiguration)this["authentication"]; }
        }

        IRiakAuthenticationConfiguration IRiakExternalLoadBalancerConfiguration.Authentication
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
