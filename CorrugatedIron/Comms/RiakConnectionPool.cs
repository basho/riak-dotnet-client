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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Comms
{
    internal class RiakConnectionPool : IRiakConnectionManager
    {
        private readonly string _hostAddress;
        private readonly int _poolSize;
        private readonly int _bufferSize;
        private readonly string _restScheme;
        private readonly int _restPort;
        private readonly int _pbcPort;
        private readonly int _networkReadTimeout;
        private readonly int _networkWriteTimeout;
        private readonly int _idleTimeout;
        private readonly TimeSpan _createSocketTimeout;
        private readonly SocketAwaitablePool _pool;
        private readonly BlockingBufferManager _blockingBufferManager;
        private List<RiakPbcSocket> _allResources;
        private BlockingCollection<RiakPbcSocket> _resources;
        private readonly string _serverUrl;
        private bool _disposing;

        public RiakConnectionPool(IRiakNodeConfiguration nodeConfig)
        {
            _hostAddress = nodeConfig.HostAddress;
            _poolSize = nodeConfig.PoolSize;
            _pool = new SocketAwaitablePool(_poolSize);
            _bufferSize = nodeConfig.BufferSize;
            _restScheme = nodeConfig.RestScheme;
            _restPort = nodeConfig.RestPort;
            _pbcPort = nodeConfig.PbcPort;
            _networkReadTimeout = nodeConfig.NetworkReadTimeout;
            _networkWriteTimeout = nodeConfig.NetworkWriteTimeout;
            _idleTimeout = nodeConfig.IdleTimeout;
            _createSocketTimeout = TimeSpan.FromMilliseconds(nodeConfig.IdleTimeout);
            
            _blockingBufferManager = new BlockingBufferManager(_bufferSize, _poolSize);
            _serverUrl = @"{0}://{1}:{2}".Fmt(_restScheme, _hostAddress, _restPort);

            Init();
        }

        private void Init()
        {
            _allResources = new List<RiakPbcSocket>();

            for (var i = 0; i < _poolSize; ++i)
            {
                var socket = new RiakPbcSocket(
                    _hostAddress,
                    _pbcPort,
                    _networkReadTimeout,
                    _networkWriteTimeout,
                    _idleTimeout,
                    _pool,
                    _blockingBufferManager);

                _allResources.Add(socket);
            }

            _resources = new BlockingCollection<RiakPbcSocket>(new ConcurrentQueue<RiakPbcSocket>(_allResources));
        }

        public void Dispose()
        {
            if(_disposing) return;

            _disposing = true;

            foreach(var conn in _allResources)
            {
                conn.Dispose();
            }
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
            if (_disposing) throw new ObjectDisposedException(this.GetType().Name);

            RiakPbcSocket socket = null;

            if (_resources.TryTake(out socket, _createSocketTimeout))
            {
                return socket;
            }

            throw new TimeoutException("Unable to create socket with in " + _createSocketTimeout);
        }

        public async Task Release(RiakPbcSocket socket)
        {
            if (_disposing) return;

            _resources.Add(socket);
        }

        public async Task ReleaseAll()
        {
            if (_disposing) return;

            //We going to let other RiakPbcSockets die a natural garbage collection death, as we may have a few open sockets
            Init();
        }
    }
}
