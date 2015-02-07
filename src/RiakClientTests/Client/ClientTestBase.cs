// <copyright file="ClientTestBase.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.Client
{
    using System.Linq;
    using RiakClient;

    public abstract class ClientTestBase
    {
        protected MockCluster Cluster;
        protected RiakClient Client;
        protected byte[] ClientId;

        public ClientTestBase()
        {
            Cluster = new MockCluster();
            ClientId = System.Text.Encoding.Default.GetBytes("fadjskl").Take(4).ToArray();
            Client = new RiakClient(Cluster);
        }
    }
}