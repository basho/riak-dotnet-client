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

using System;
using System.Configuration;

namespace RiakClient.Config
{
    public class RiakNodeConfiguration : ConfigurationElement, IRiakNodeConfiguration
    {
        private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromMilliseconds(4000);

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("hostAddress", IsRequired = true)]
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

        // TODO: put this back in when we've got the idling stuff figured out
        // [ConfigurationProperty("idleTimeout", DefaultValue = 15000, IsRequired = false)]
        // public int IdleTimeout
        // {
        //     get { return (int)this["idleTimeout"]; }
        //     set { this["idleTimeout"] = value; }
        // }

        [ConfigurationProperty("networkReadTimeout", IsRequired = false)]
        public TimeSpan NetworkReadTimeout
        {
            get
            {
                if (this["networkReadTimeout"] == null)
                {
                    return s_defaultTimeout;
                }

                int networkReadTimeoutMilliseconds;
                if (int.TryParse(this["networkReadTimeout"].ToString(), out networkReadTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkReadTimeoutMilliseconds);
                }
                else
                {
                    return s_defaultTimeout;
                }
            }
            set { this["networkReadTimeout"] = value.TotalMilliseconds.ToString(); }
        }

        [ConfigurationProperty("networkWriteTimeout", IsRequired = false)]
        public TimeSpan NetworkWriteTimeout
        {
            get
            {
                if (this["networkWriteTimeout"] == null)
                {
                    return s_defaultTimeout;
                }

                int networkWriteTimeoutMilliseconds;
                if (int.TryParse(this["networkWriteTimeout"].ToString(), out networkWriteTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkWriteTimeoutMilliseconds);
                }
                else
                {
                    return s_defaultTimeout;
                }
            }
            set { this["networkWriteTimeout"] = value.TotalMilliseconds.ToString(); }
        }

        [ConfigurationProperty("networkConnectTimeout", IsRequired = false)]
        public TimeSpan NetworkConnectTimeout
        {
            get
            {
                if (this["networkConnectTimeout"] == null)
                {
                    return s_defaultTimeout;
                }

                int networkConnectTimeoutMilliseconds;
                if (int.TryParse(this["networkConnectTimeout"].ToString(), out networkConnectTimeoutMilliseconds))
                {
                    return TimeSpan.FromMilliseconds(networkConnectTimeoutMilliseconds);
                }
                else
                {
                    return s_defaultTimeout;
                }
            }
            set { this["networkConnectTimeout"] = value.TotalMilliseconds.ToString(); }
        }
    }
}
