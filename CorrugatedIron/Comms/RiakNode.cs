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
using CorrugatedIron.Config;
using CorrugatedIron.Containers;
using CorrugatedIron.Exceptions;

namespace CorrugatedIron.Comms
{
    public interface IRiakNode : IDisposable
    {
        string Name { get; }
        RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun);
        RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun);
    }

    internal class RiakNode : IRiakNode
    {
        private readonly ResourcePool<IRiakConnection> _connections;
        private bool _disposing;

        public string Name { get; private set; }

        public RiakNode(IRiakNodeConfiguration nodeConfiguration, IRiakConnectionFactory connectionFactory)
        {
            Name = nodeConfiguration.Name;
            _connections = new ResourcePool<IRiakConnection>(nodeConfiguration.PoolSize,
                nodeConfiguration.AcquireTimeout, () => connectionFactory.CreateConnection(nodeConfiguration),
                conn => conn.Dispose());
        }

        public RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun)
        {
            return UseConnection(clientId, useFun, RiakResult.Error);
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return UseConnection(clientId, useFun, RiakResult<TResult>.Error);
        }

        private TRiakResult UseConnection<TRiakResult>(byte[] clientId, Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, TRiakResult> onError)
            where TRiakResult : RiakResult
        {
            if (_disposing) return onError(ResultCode.ShuttingDown, null);

            Func<IRiakConnection, TRiakResult> wrapper = conn =>
                {
                    using (var mgr = new RiakConnectionUsageManager(conn, clientId))
                    {
                        if (mgr.SetupResult.IsSuccess)
                        {
                            return useFun(conn);
                        }
                        return onError(mgr.SetupResult.ResultCode, mgr.SetupResult.ErrorMessage);
                    }
                };

            try
            {
                var response = _connections.Consume(wrapper);
                if (response.Item1)
                {
                    return response.Item2;
                }
                return onError(ResultCode.CommunicationError, null);
            }
            catch (RiakException ex)
            {
                return onError(ResultCode.RiakError, ex.Message);
            }
        }

        public void Dispose()
        {
            _disposing = true;

            _connections.Dispose();
        }
    }
}
