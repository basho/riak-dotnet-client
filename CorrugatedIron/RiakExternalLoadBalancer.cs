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

using System.Threading.Tasks;
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using System;
using CorrugatedIron.Exceptions;

namespace CorrugatedIron
{
    public class RiakExternalLoadBalancer : IRiakEndPoint
    {
        private readonly IRiakExternalLoadBalancerConfiguration _lbConfiguration;
        private readonly RiakNode _node;
        private bool _disposing;

        public RiakExternalLoadBalancer(IRiakExternalLoadBalancerConfiguration lbConfiguration)
        {
            _lbConfiguration = lbConfiguration;
            _node = new RiakNode(_lbConfiguration.Target);
        }

        public static IRiakEndPoint FromConfig(string configSectionName)
        {
            return new RiakExternalLoadBalancer(RiakExternalLoadBalancerConfiguration.LoadFromConfig(configSectionName));
        }

        public static IRiakEndPoint FromConfig(string configSectionName, string configFileName)
        {
            return new RiakExternalLoadBalancer(RiakExternalLoadBalancerConfiguration.LoadFromConfig(configSectionName, configFileName));
        }

        public void Dispose()
        {
            _disposing = true;
            _node.Dispose();
        }

        public IRiakClient CreateClient()
        {
            return new RiakClient(this, new RiakConnection());
        }

        public async Task GetSingleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var result = await _node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);

            return result;
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            var result = await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
            return result;
        }

        public async Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetMultipleResultViaPbc(useFun).ConfigureAwait(false);
        }

        public async Task GetMultipleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _node;
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            await riakEndPointContext.Node.GetMultipleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
        }

        public async Task GetSingleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
        }

        public async Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var result = await _node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
            return result;
        }

        public async Task GetMultipleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            await _node.GetMultipleResultViaRest(useFun).ConfigureAwait(false);
        }
    }
}