// <copyright file="RiakClientTestBase.cs" company="Basho Technologies, Inc.">
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

namespace RiakClientTests.RiakClientTests
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using RiakClient;
    using RiakClient.Comms;
    using RiakClient.Config;

    internal abstract class RiakClientTestBase<TRequest, TResult> : IDisposable
        where TRequest : class, ProtoBuf.IExtensible
        where TResult : class, ProtoBuf.IExtensible, new()
    {
        protected RiakResult<TResult> Result = null;
        protected Mock<IRiakConnection> ConnMock;
        protected Mock<IRiakNodeConfiguration> NodeConfigMock;
        protected Mock<IRiakClusterConfiguration> ClusterConfigMock;
        protected Mock<IRiakConnectionFactory> ConnFactoryMock;
        protected RiakCluster Cluster;
        protected IRiakClient Client;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Cluster != null)
            {
                Cluster.Dispose();
            }
        }

        protected void SetUpInternal()
        {
            ConnMock = new Mock<IRiakConnection>();
            ClusterConfigMock = new Mock<IRiakClusterConfiguration>();
            ConnFactoryMock = new Mock<IRiakConnectionFactory>();
            NodeConfigMock = new Mock<IRiakNodeConfiguration>();

            ConnMock.Setup(m => m.PbcWriteRead<TRequest, TResult>(It.IsAny<TRequest>())).Returns(() => Result);
            ConnFactoryMock.Setup(m =>
                m.CreateConnection(
                    It.IsAny<IRiakNodeConfiguration>(),
                    It.IsAny<IRiakAuthenticationConfiguration>())
                ).Returns(ConnMock.Object);
            NodeConfigMock.SetupGet(m => m.PoolSize).Returns(1);
            ClusterConfigMock.SetupGet(m => m.RiakNodes).Returns(new List<IRiakNodeConfiguration> { NodeConfigMock.Object });
            ClusterConfigMock.SetupGet(m => m.DefaultRetryCount).Returns(100);
            ClusterConfigMock.SetupGet(m => m.DefaultRetryWaitTime).Returns(new Timeout(100));

            Cluster = new RiakCluster(ClusterConfigMock.Object, ConnFactoryMock.Object);
            Client = Cluster.CreateClient();
        }
    }
}
