// <copyright file="MockCluster.cs" company="Basho Technologies, Inc.">
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
    using System;
    using System.Collections.Generic;
    using Moq;
    using RiakClient;
    using RiakClient.Comms;

    public sealed class MockCluster : IRiakEndPoint
    {
        public Mock<IRiakConnection> ConnectionMock = new Mock<IRiakConnection>();

        public MockCluster()
        {
            RetryWaitTime = TimeSpan.FromMilliseconds(200);
        }

        public void Dispose()
        {
        }

        public IRiakClient CreateClient()
        {
            return new Mock<IRiakClient>().Object;
        }

        public IRiakClient CreateClient(string seed)
        {
            return new Mock<IRiakClient>().Object;
        }

        public TimeSpan RetryWaitTime { get; set; }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return useFun(ConnectionMock.Object);
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult
        {
            throw new NotImplementedException();
        }
    }
}
