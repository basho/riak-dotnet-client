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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorrugatedIron.Comms.LoadBalancing;
using CorrugatedIron.Config;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Comms
{
    public interface IRiakCluster : IDisposable
    {
        RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun);
        RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun);
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun);
    }

    public class RiakCluster : IRiakCluster
    {
        private readonly byte[] _pollClientId = new byte[] { 1, 1, 1, 1 };
        private readonly RoundRobinStrategy _loadBalancer;
        private readonly List<IRiakNode> _nodes;
        private readonly ConcurrentQueue<IRiakNode> _offlineNodes;
        private readonly int _nodePollTime;
        private bool _disposing;

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration, IRiakNodeFactory nodeFactory, IRiakConnectionFactory connectionFactory)
        {
            _nodePollTime = clusterConfiguration.NodePollTime;
            _nodes = clusterConfiguration.RiakNodes.Select(rn => nodeFactory.CreateNode(rn, connectionFactory)).ToList();
            _loadBalancer = new RoundRobinStrategy();
            _loadBalancer.Initialise(_nodes);
            _offlineNodes = new ConcurrentQueue<IRiakNode>();

            ThreadPool.QueueUserWorkItem(NodeMonitor);
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
            if (_disposing) return onError(ResultCode.ShuttingDown, "System currently shutting down");

            var node = _loadBalancer.SelectNode();

            if (node != null)
            {
                var result = node.UseConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        return UseConnection(clientId, useFun, onError);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        DeactivateNode(node);
                        return UseConnection(clientId, useFun, onError);
                    }

                    // use the onError function so that we know the return value is the right type
                    return onError(result.ResultCode, result.ErrorMessage);
                }
                return (TRiakResult)result;
            }
            return onError(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
        {
            if (_disposing) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "System currently shutting down");

            var node = _loadBalancer.SelectNode();

            if (node != null)
            {
                var result = node.UseDelayedConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        return UseDelayedConnection(clientId, useFun);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        DeactivateNode(node);
                        return UseDelayedConnection(clientId, useFun);
                    }
                }
                return result;
            }
            return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        private void DeactivateNode(IRiakNode node)
        {
            _offlineNodes.Enqueue(node);
        }

        private void NodeMonitor(object _)
        {
            while (!_disposing)
            {
                var deadNodes = new List<IRiakNode>();
                IRiakNode node = null;
                while (_offlineNodes.TryDequeue(out node) && !_disposing)
                {
                    var result = node.UseConnection(_pollClientId, c => c.PbcWriteRead<RpbPingReq, RpbPingResp>(new RpbPingReq()));
                    if (result.IsSuccess)
                    {
                        _loadBalancer.AddNode(node);
                    }
                    else
                    {
                        deadNodes.Add(node);
                    }
                }

                if (!_disposing)
                {
                    foreach (var deadNode in deadNodes)
                    {
                        _offlineNodes.Enqueue(deadNode);
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
    }
}
