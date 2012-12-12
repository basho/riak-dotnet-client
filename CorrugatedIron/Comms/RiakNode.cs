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

using CorrugatedIron.Config;
using System;
using System.Collections.Generic;

namespace CorrugatedIron.Comms
{
    public interface IRiakNode : IDisposable
    {
        RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun);
        RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun);

        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
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

        public RiakResult UseConnection(Func<IRiakConnection, RiakResult> useFun)
        {
            return UseConnection(useFun, RiakResult.Error);
        }

        public RiakResult<TResult> UseConnection<TResult>(Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return UseConnection(useFun, RiakResult<TResult>.Error);
        }

        private TRiakResult UseConnection<TRiakResult>(Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, bool, TRiakResult> onError)
            where TRiakResult : RiakResult
        {
            if(_disposing) return onError(ResultCode.ShuttingDown, "Connection is shutting down", true);

            var response = _connections.Consume(useFun);
            if(response.Item1)
            {
                return response.Item2;
            }
            return onError(ResultCode.NoConnections, "Unable to acquire connection", true);
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
            where TResult : RiakResult
        {
            if(_disposing) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "Connection is shutting down", true);

            var response = _connections.DelayedConsume(useFun);
            if(response.Item1)
            {
                return response.Item2;
            }
            return RiakResult<IEnumerable<TResult>>.Error(ResultCode.NoConnections, "Unable to acquire connection", true);
        }

        public void Dispose()
        {
            _disposing = true;
            _connections.Dispose();
        }
    }
}