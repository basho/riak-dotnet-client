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

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using CorrugatedIron.Config;
using System;

namespace CorrugatedIron.Comms
{
    public class RiakNode : IRiakNode
    {
        private readonly IRiakConnectionManager _connectionManager;

        public RiakNode(IRiakNodeConfiguration nodeConfiguration)
        {
            // assume that if the node has a pool size of 0 then the intent is to have the connections
            // made on the fly
            if (nodeConfiguration.PoolSize == 0)
            {
                _connectionManager = new RiakOnTheFlyConnection(nodeConfiguration);
            }
            else
            {
                _connectionManager = new RiakConnectionPool(nodeConfiguration);
            }
        }

        public async Task<RiakPbcSocket> CreateSocket()
        {
            return await _connectionManager.CreateSocket().ConfigureAwait(false);
        }

        public async Task Release(RiakPbcSocket socket)
        {
            await _connectionManager.Release(socket).ConfigureAwait(false);
        }

        public async Task ReleaseAll()
        {
            await _connectionManager.ReleaseAll().ConfigureAwait(false);
        }

        public async Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            RiakPbcSocket socket = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                socket = await _connectionManager.CreateSocket().ConfigureAwait(false);
                await useFun(socket).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(socket).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }
        }

        public async Task GetSingleResultViaPbc(RiakPbcSocket socket, Func<RiakPbcSocket, Task> useFun)
        {
            await useFun(socket).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            var result = default(TResult);
            RiakPbcSocket socket = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                socket = await _connectionManager.CreateSocket().ConfigureAwait(false);
                result = await useFun(socket).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(socket).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }

            return result;
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(RiakPbcSocket socket, Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            var result = await useFun(socket).ConfigureAwait(false);
            return result;
        }

        public async Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            RiakPbcSocket socket = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                socket = await _connectionManager.CreateSocket().ConfigureAwait(false);
                await useFun(socket).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(socket).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }
        }

        public async Task GetMultipleResultViaPbc(RiakPbcSocket socket, Func<RiakPbcSocket, Task> useFun)
        {
            await useFun(socket).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaRest(Func<string, Task> useFun)
        {
            string serverUrl = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                serverUrl = await _connectionManager.CreateServerUrl().ConfigureAwait(false);
                await useFun(serverUrl).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(serverUrl).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }
        }

        public async Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun)
        {
            var result = default(TResult);
            string serverUrl = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                serverUrl = await _connectionManager.CreateServerUrl().ConfigureAwait(false);
                result = await useFun(serverUrl).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(serverUrl).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }

            return result;
        }

        public async Task GetMultipleResultViaRest(Func<string, Task> useFun)
        {
            string serverUrl = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                serverUrl = await _connectionManager.CreateServerUrl().ConfigureAwait(false);
                await useFun(serverUrl).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            await _connectionManager.Release(serverUrl).ConfigureAwait(false);

            if (capturedException != null)
            {
                capturedException.Throw();
            }
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
        }
    }
}