// <copyright file="RiakAuthenticationConfiguration.cs" company="Basho Technologies, Inc.">
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
    using System.Configuration;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a configuration file element for configuring Riak Authentication.
    /// </summary>
    public class RiakAuthenticationConfiguration : ConfigurationElement, IRiakAuthenticationConfiguration
    {
        /// <inheritdoc/>
        [ConfigurationProperty("username", DefaultValue = "", IsRequired = true)]
        public string Username
        {
            get { return (string)this["username"]; }
            set { this["username"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("clientCertificateFile", DefaultValue = "", IsRequired = false)]
        public string ClientCertificateFile
        {
            get { return (string)this["clientCertificateFile"]; }
            set { this["clientCertificateFile"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("clientCertificateSubject", DefaultValue = "", IsRequired = false)]
        public string ClientCertificateSubject
        {
            get { return (string)this["clientCertificateSubject"]; }
            set { this["clientCertificateSubject"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("certificateAuthorityFile", DefaultValue = "", IsRequired = false)]
        public string CertificateAuthorityFile
        {
            get { return (string)this["certificateAuthorityFile"]; }
            set { this["certificateAuthorityFile"] = value; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("checkCertificateRevocation", DefaultValue = false, IsRequired = false)]
        public bool CheckCertificateRevocation
        {
            get { return (bool)this["checkCertificateRevocation"]; }
            set { this["checkCertificateRevocation"] = value; }
        }
    }
}
