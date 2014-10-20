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
using CorrugatedIron.Comms.LoadBalancing;
using CorrugatedIron.Config;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CorrugatedIron
{
    public class RiakCluster : IRiakEndPoint
    {
        private readonly IRiakConnection _riakConnection;
        private readonly RoundRobinStrategy _loadBalancer;
        private readonly List<IRiakNode> _nodes;
        private readonly BlockingCollection<IRiakNode> _offlineNodes;

        private readonly int _nodePollTime;
        private bool _disposing;

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration)
        {
            _riakConnection = new RiakConnection();
            _nodePollTime = clusterConfiguration.NodePollTime;

            _nodes =
                clusterConfiguration.RiakNodes.Select(riakNodeConfiguration => new RiakNode(riakNodeConfiguration))
                    .Cast<IRiakNode>()
                    .ToList();

            _loadBalancer = new RoundRobinStrategy();
            _loadBalancer.Initialise(_nodes);
            _offlineNodes = new BlockingCollection<IRiakNode>(new ConcurrentQueue<IRiakNode>());

            Task.Factory.StartNew(NodeMonitor);
        }

        /// <summary>
        /// Creates an instance of <see cref="CorrugatedIron.IRiakClient"/> populated from from the configuration section
        /// specified by <paramref name="configSectionName"/>.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section to load the settings from.</param>
        /// <returns>A fully configured <see cref="IRiakEndPoint"/></returns>
        public static IRiakEndPoint FromConfig(string configSectionName)
        {
            return new RiakCluster(RiakClusterConfiguration.LoadFromConfig(configSectionName));
        }

        /// <summary>
        /// Creates an instance of <see cref="CorrugatedIron.IRiakClient"/> populated from from the configuration section
        /// specified by <paramref name="configSectionName"/>.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section to load the settings from.</param>
        /// <param name="configFileName">The full path and name of the config file to load the configuration from.</param>
        /// <returns>A fully configured <see cref="IRiakEndPoint"/></returns>
        public static IRiakEndPoint FromConfig(string configSectionName, string configFileName)
        {
            return new RiakCluster(RiakClusterConfiguration.LoadFromConfig(configSectionName, configFileName));
        }

        private void DeactivateNode(IRiakNode node)
        {
            lock (node)
            {
                if (!_offlineNodes.Contains(node))
                {
                    _loadBalancer.RemoveNode(node);
                    node.ReleaseAll().ConfigureAwait(false).GetAwaiter().GetResult();
                    _offlineNodes.Add(node);
                }
            }
        }

        private async void NodeMonitor()
        {
            while (!_disposing)
            {
                var deadNodes = new List<IRiakNode>();
                IRiakNode node = null;
                while (_offlineNodes.TryTake(out node, _nodePollTime) && !_disposing)
                {
                    try
                    {
                        var nodeToMonitor = node;

                       await
                            _riakConnection.PbcWriteRead(new RiakNodeEndpoint(nodeToMonitor), MessageCode.PingReq,
                                MessageCode.PingResp).ConfigureAwait(false);

                        _loadBalancer.AddNode(node);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            node.ReleaseAll().ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        finally
                        {
                            deadNodes.Add(node);
                        }
                    }
                }

                if (!_disposing)
                {
                    foreach (var deadNode in deadNodes)
                    {
                        _offlineNodes.Add(deadNode);
                    }

                    Thread.Sleep(_nodePollTime);
                }
            }
        }

        public void Dispose()
        {
            _disposing = true;
            _nodes.ForEach(n => n.Dispose());
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

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
                throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
            }

            try
            {
                await node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }

        public async Task GetSingleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _loadBalancer.SelectNode();

                if (riakEndPointContext.Node == null)
                {
                    throw new RiakException((uint) ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
                }
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            try
            {
                await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(riakEndPointContext.Node);
                }
                throw;
            }
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint) ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
               throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node",true);
            }

            try
            {
                var result = await node.GetSingleResultViaPbc(useFun).ConfigureAwait(false);
                return result;
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }

        public async Task<TResult> GetSingleResultViaPbc<TResult>(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _loadBalancer.SelectNode();

                if (riakEndPointContext.Node == null)
                {
                    throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
                }
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            try
            {
                var result = await riakEndPointContext.Node.GetSingleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
                return result;
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(riakEndPointContext.Node);
                }
                throw;
            }
        }

        public async Task GetMultipleResultViaPbc(Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
                throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
            }

            try
            {
                await node.GetMultipleResultViaPbc(useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }

        public async Task GetMultipleResultViaPbc(IRiakEndPointContext riakEndPointContext, Func<RiakPbcSocket, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            if (riakEndPointContext.Node == null)
            {
                riakEndPointContext.Node = _loadBalancer.SelectNode();

                if (riakEndPointContext.Node == null)
                {
                    throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
                }
            }

            if (riakEndPointContext.Socket == null)
            {
                riakEndPointContext.Socket = await riakEndPointContext.Node.CreateSocket();
            }

            try
            {
                await riakEndPointContext.Node.GetMultipleResultViaPbc(riakEndPointContext.Socket, useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(riakEndPointContext.Node);
                }
                throw;
            }
        }

        public async Task GetSingleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
                throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
            }

            try
            {
                await node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }

        public async Task<TResult> GetSingleResultViaRest<TResult>(Func<string, Task<TResult>> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
                throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
            }

            try
            {
                var result = await node.GetSingleResultViaRest(useFun).ConfigureAwait(false);
                return result;
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }

        public async Task GetMultipleResultViaRest(Func<string, Task> useFun)
        {
            if (_disposing)
            {
                throw new RiakException((uint)ResultCode.ShuttingDown, "System currently shutting down", true);
            }

            var node = _loadBalancer.SelectNode();

            if (node == null)
            {
                throw new RiakException((uint)ResultCode.ClusterOffline, "Unable to access functioning Riak node", true);
            }

            try
            {
                await node.GetMultipleResultViaRest(useFun).ConfigureAwait(false);
            }
            catch (RiakException riakException)
            {
                if (riakException.NodeOffline)
                {
                    DeactivateNode(node);
                }
                throw;
            }
        }
    }
}
