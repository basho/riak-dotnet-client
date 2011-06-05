// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using CorrugatedIron.Comms;
using Moq;

namespace CorrugatedIron.Tests.Comms
{
    public abstract class RiakClientTestBase
    {
        protected Mock<IRiakCluster> Cluster;
        protected Mock<IRiakConnection> Conn;
        protected RiakClient Client;
        protected RiakResult Result;
        protected byte[] ClientId;

        protected void SetUpInternal()
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(ClientId != null)
            {
                Cluster.Setup(m => m.UseConnection(ClientId, It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object, ClientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }

    public abstract class RiakClientTestBase<TResult>
    {
        protected Mock<IRiakCluster> Cluster;
        protected Mock<IRiakConnection> Conn;
        protected RiakClient Client;
        protected RiakResult<TResult> Result;
        protected byte[] ClientId;

        protected void SetUpInternal()
        {
            Conn = new Mock<IRiakConnection>();
            Cluster = new Mock<IRiakCluster>();

            if(ClientId != null)
            {
                Cluster.Setup(m => m.UseConnection(ClientId, It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object, ClientId);
            }
            else
            {
                Cluster.Setup(m => m.UseConnection(It.IsAny<byte[]>(), It.IsAny<Func<IRiakConnection, RiakResult<TResult>>>())).Returns(Result);
                Client = new RiakClient(Cluster.Object);
            }
        }
    }
}
