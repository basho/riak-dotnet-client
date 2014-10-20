// Copyright (c) 2013 - OJ Reeves & Jeremiah Peschka
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

using System.Threading.Tasks;
using CorrugatedIron.Comms.Sockets;
using CorrugatedIron.Config;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Comms
{
    internal class RiakOnTheFlyConnection : IRiakConnectionManager
    {
        private readonly IRiakNodeConfiguration _nodeConfig;
        private readonly SocketAwaitablePool _pool;
        private readonly BlockingBufferManager _bufferManager;
        private readonly string _serverUrl;
        private bool _disposing;
        private readonly BlockingLimitedList<RiakPbcSocket> _resources;

        public RiakOnTheFlyConnection(IRiakNodeConfiguration nodeConfig, int bufferPoolSize = 20)
        {
            _nodeConfig = nodeConfig;
            _serverUrl = @"{0}://{1}:{2}".Fmt(nodeConfig.RestScheme, nodeConfig.HostAddress, nodeConfig.RestPort);
            _pool = new SocketAwaitablePool(nodeConfig.PoolSize);
            _bufferManager = new BlockingBufferManager(nodeConfig.BufferSize, bufferPoolSize);

            _resources = new BlockingLimitedList<RiakPbcSocket>(bufferPoolSize);
        }

        public void Dispose()
        {
            if(_disposing) return;

            _disposing = true;
        }

        public async Task<string> CreateServerUrl()
        {
            return _serverUrl;
        }

        public async Task Release(string serverUrl)
        {
        }

        public async Task<RiakPbcSocket> CreateSocket()
        {
            var socket = new RiakPbcSocket(
                    _nodeConfig.HostAddress,
                    _nodeConfig.PbcPort,
                    _nodeConfig.NetworkReadTimeout,
                    _nodeConfig.NetworkWriteTimeout,
                    _nodeConfig.IdleTimeout,
                    _pool,
                    _bufferManager);

            _resources.Enqueue(socket);

            return socket;
        }

        public async Task Release(RiakPbcSocket socket)
        {
            _resources.Dequeue(socket);

            if (_disposing) return;

            if (socket != null)
            {
                socket.Dispose();
            }
        }

        public async Task ReleaseAll()
        {
            //We going to let other RiakPbcSockets die a natural garbage collection death, as we may have a few open sockets
            _resources.Clear();
        }
    }
}
