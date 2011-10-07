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
using System.Collections.Generic;
using CorrugatedIron.Config;

namespace CorrugatedIron.Comms
{
    public interface IRiakNode : IDisposable
    {
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, byte[] clientId);
        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun, byte[] clientId);
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, byte[] clientId)
            where TResult : RiakResult;
    }

    public class RiakNode : IRiakNode
    {
        private readonly RiakConnectionPool _connections;
        private bool _disposing;

        public RiakNode(IRiakNodeConfiguration nodeConfiguration, IRiakConnectionFactory connectionFactory)
        {
            _connections = new RiakConnectionPool(nodeConfiguration, connectionFactory);
        }

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun, byte[] clientId = default(byte[]))
        {
            return UseConnection(useFun, RiakResult.Error, clientId);
        }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun,byte[] clientId = default(byte[]))
        {
            return UseConnection(useFun, RiakResult<TResult>.Error, clientId);
        }

        private TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, TRiakResult> onError, byte[] clientId = default(byte[]))
            where TRiakResult : RiakResult
        {
            if (_disposing) return onError(ResultCode.ShuttingDown, "Connection is shutting down");

            Func<IRiakConnection, TRiakResult> wrapper = conn =>
                {
                    conn.SetClientId(clientId);
                    return useFun(conn);
                };

            var response = _connections.Consume(wrapper);
            if (response.Item1)
            {
                return response.Item2;
            }
            return onError(ResultCode.NoConnections, "Unable to acquire connection");
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, byte[] clientId = default(byte[]))
            where TResult : RiakResult
        {
            if (_disposing) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "Connection is shutting down");

            Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> wrapper = (conn, onFinish) =>
                {
                    conn.SetClientId(clientId);
                    return useFun(conn, onFinish);
                };

            var response = _connections.DelayedConsume(wrapper);
            if (response.Item1)
            {
                return response.Item2;
            }
            return RiakResult<IEnumerable<TResult>>.Error(ResultCode.NoConnections, "Unable to acquire connection");
        }

        public void Dispose()
        {
            _disposing = true;
            _connections.Dispose();
        }
    }
}
