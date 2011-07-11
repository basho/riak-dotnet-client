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
    public interface IRiakNodeConfiguration
    {
        string Name { get; }
        string HostAddress { get; }
        int PbcPort { get; }
        string RestScheme { get; }
        int RestPort { get; }
        int PoolSize { get; }
        int AcquireTimeout { get; }
        int IdleTimeout { get; }
    }

    public class RiakNodeConfiguration : ConfigurationElement, IRiakNodeConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("hostAddress", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string HostAddress
        {
            get { return (string)this["hostAddress"]; }
            set { this["hostAddress"] = value; }
        }

        [ConfigurationProperty("pbcPort", DefaultValue = 8088, IsRequired = false)]
        public int PbcPort
        {
            get { return (int)this["pbcPort"]; }
            set { this["pbcPort"] = value; }
        }

        [ConfigurationProperty("restScheme", DefaultValue = "http", IsRequired = false)]
        public string RestScheme
        {
            get { return (string)this["restScheme"]; }
            set { this["restScheme"] = value; }
        }

        [ConfigurationProperty("restPort", DefaultValue = 8098, IsRequired = false)]
        public int RestPort
        {
            get { return (int)this["restPort"]; }
            set { this["restPort"] = value; }
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
