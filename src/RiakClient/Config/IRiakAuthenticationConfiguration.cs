// <copyright file="IRiakAuthenticationConfiguration.cs" company="Basho Technologies, Inc.">
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
    /// <summary>
    /// Represents a configuration element interface for Riak authentication.
    /// </summary>
    public interface IRiakAuthenticationConfiguration
    {
        /// <summary>
        /// The username to authenticate with.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The password to authenticate with.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// A client certificate file to load and use.
        /// Must be a valid file path.
        /// </summary>
        string ClientCertificateFile { get; set; }

        /// <summary>
        /// A client certificate subject, used to find and use a certificate from the local store.
        /// </summary>
        string ClientCertificateSubject { get; set; }

        /// <summary>
        /// A client certificate authority certificate to load and use. 
        /// Must be a valid file path.
        /// </summary>
        string CertificateAuthorityFile { get; set; }

        /// <summary>
        /// The option to check the certificate revocation list during authentication.
        /// </summary>
        bool CheckCertificateRevocation { get; set; }
    }
}