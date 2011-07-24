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
using System.Threading.Tasks;
using CorrugatedIron.Comms.LoadBalancing;
using CorrugatedIron.Config;
using CorrugatedIron.Messages;

namespace CorrugatedIron.Comms
{
    public interface IRiakCluster : IDisposable
    {
        IRiakClient CreateClient();
        int RetryWaitTime { get; set; }
        RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts);
        RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun, int retryAttempts);
        RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult;
    }

    public class RiakCluster : IRiakCluster
    {
        private readonly byte[] _pollClientId = new byte[] { 1, 1, 1, 1 };
        private readonly RoundRobinStrategy _loadBalancer;
        private readonly List<IRiakNode> _nodes;
        private readonly ConcurrentQueue<IRiakNode> _offlineNodes;
        private readonly int _nodePollTime;
        private readonly int _defaultRetryCount;
        private bool _disposing;

        public int RetryWaitTime { get; set; }

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration, IRiakConnectionFactory connectionFactory)
        {
            _nodePollTime = clusterConfiguration.NodePollTime;
            _nodes = clusterConfiguration.RiakNodes.Select(rn => new RiakNode(rn, connectionFactory)).Cast<IRiakNode>().ToList();
            _loadBalancer = new RoundRobinStrategy();
            _loadBalancer.Initialise(_nodes);
            _offlineNodes = new ConcurrentQueue<IRiakNode>();
            _defaultRetryCount = clusterConfiguration.DefaultRetryCount;
            RetryWaitTime = clusterConfiguration.DefaultRetryWaitTime;

            Task.Factory.StartNew(NodeMonitor);
        }

        public IRiakClient CreateClient()
        {
            return new RiakClient(this)
            {
                RetryCount = _defaultRetryCount
            };
        }

        public RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun, int retryAttempts)
        {
            return UseConnection(clientId, useFun, RiakResult.Error, retryAttempts);
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun, int retryAttempts)
        {
            return UseConnection(clientId, useFun, RiakResult<TResult>.Error, retryAttempts);
        }

        private TRiakResult UseConnection<TRiakResult>(byte[] clientId, Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, string, TRiakResult> onError, int retryAttempts)
            where TRiakResult : RiakResult
        {
            if(retryAttempts < 0) return onError(ResultCode.NoRetries, "Unable to access a connection on the cluster.");
            if (_disposing) return onError(ResultCode.ShuttingDown, "System currently shutting down");

            var node = _loadBalancer.SelectNode();

            if (node != null)
            {
                var result = node.UseConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        Thread.Sleep(RetryWaitTime);
                        return UseConnection(clientId, useFun, onError, retryAttempts - 1);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        DeactivateNode(node);
                        Thread.Sleep(RetryWaitTime);
                        return UseConnection(clientId, useFun, onError, retryAttempts - 1);
                    }

                    // use the onError function so that we know the return value is the right type
                    return onError(result.ResultCode, result.ErrorMessage);
                }
                return (TRiakResult)result;
            }
            return onError(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        public RiakResult<IEnumerable<TResult>> UseDelayedConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun, int retryAttempts)
            where TResult : RiakResult
        {
            if(retryAttempts < 0) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.NoRetries, "Unable to access a connection on the cluster.");
            if (_disposing) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "System currently shutting down");

            var node = _loadBalancer.SelectNode();

            if (node != null)
            {
                var result = node.UseDelayedConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        Thread.Sleep(RetryWaitTime);
                        return UseDelayedConnection(clientId, useFun, retryAttempts - 1);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        DeactivateNode(node);
                        Thread.Sleep(RetryWaitTime);
                        return UseDelayedConnection(clientId, useFun, retryAttempts - 1);
                    }
                }
                return result;
            }
            return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        private void DeactivateNode(IRiakNode node)
        {
            lock (node)
            {
                if (!_offlineNodes.Contains(node))
                {
                    _loadBalancer.RemoveNode(node);
                    _offlineNodes.Enqueue(node);
                }
            }
        }

        private void NodeMonitor()
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
