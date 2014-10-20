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

using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using Moq;
using System.Collections.Generic;

namespace CorrugatedIron.Tests.RiakClientTests
{
    internal abstract class RiakClientTestBase<TRequest, TResult>
        where TRequest : class
        where TResult : class, new()
    {
        protected TResult Result = null;
        protected Mock<IRiakConnection> ConnMock;
        protected Mock<IRiakNodeConfiguration> NodeConfigMock;
        protected Mock<IRiakClusterConfiguration> ClusterConfigMock;
        protected RiakCluster Cluster;
        protected IRiakClient Client;

        protected void SetUpInternal()
        {
            ConnMock = new Mock<IRiakConnection>();
            ClusterConfigMock = new Mock<IRiakClusterConfiguration>();
            NodeConfigMock = new Mock<IRiakNodeConfiguration>();

            ConnMock.Setup(m => m.PbcWriteRead<TRequest, TResult>(It.IsAny<IRiakEndPoint>(), It.IsAny<TRequest>()).ConfigureAwait(false).GetAwaiter().GetResult()).Returns(() => Result);
            NodeConfigMock.SetupGet(m => m.PoolSize).Returns(1);
            NodeConfigMock.SetupGet(m => m.BufferSize).Returns(2097152);
            ClusterConfigMock.SetupGet(m => m.RiakNodes).Returns(new List<IRiakNodeConfiguration> { NodeConfigMock.Object });
            ClusterConfigMock.SetupGet(m => m.DefaultRetryCount).Returns(100);
            ClusterConfigMock.SetupGet(m => m.DefaultRetryWaitTime).Returns(100);

            Cluster = new RiakCluster(ClusterConfigMock.Object);
            Client = Cluster.CreateClient();
        }
    }
}
