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
using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Config;
using CorrugatedIron.Containers;
using CorrugatedIron.Extensions;

namespace CorrugatedIron.Comms
{
    public interface IRiakCluster : IDisposable
    {
        RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun);
        RiakResult UseConnection(byte[] clientId, Func<IRiakConnection, RiakResult> useFun);
    }

    public class RiakCluster : IRiakCluster
    {
        private readonly object _nodeMoveLock = new object();
        private readonly Dictionary<string, IRiakNode> _liveNodes;
        private readonly Dictionary<string, IRiakNode> _brokenNodes;
        private readonly IConcurrentEnumerator<IRiakNode> _roundRobin;
        private List<IRiakNode> _activeNodes;
        private bool _disposing;

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration, IRiakNodeFactory nodeFactory, IRiakConnectionFactory connectionFactory)
        {
            _brokenNodes = new Dictionary<string, IRiakNode>();
            _liveNodes = clusterConfiguration.RiakNodes.Select(rn => nodeFactory.CreateNode(rn, connectionFactory)).ToDictionary(n => n.Name);
            _activeNodes = _liveNodes.Values.ToList();
            Func<IEnumerable<IRiakNode>> cycleFunc = () => _activeNodes;
            
            _roundRobin = new ConcurrentEnumerable<IRiakNode>(cycleFunc.Cycle()).GetEnumerator();
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
            if (_disposing) return onError(ResultCode.ShuttingDown, "The application is in the process of shutting down");

            IRiakNode node;
            if (_roundRobin.TryMoveNext(out node))
            {
                var result = node.UseConnection(clientId, useFun);

                if (!result.IsSuccess && result.ResultCode == ResultCode.CommunicationError)
                {
                    // node is misbehaving, pull it from the pool
                    DeactivateNode(node);

                    // try again on another node
                    return UseConnection(clientId, useFun, onError);
                }
                return (TRiakResult)result;
            }
            return onError(ResultCode.ClusterOffline, "No functioning nodes left in the cluster.");
        }

        public void Dispose()
        {
            _disposing = true;

            _liveNodes.ForEach(n => n.Value.Dispose());
            _brokenNodes.ForEach(n => n.Value.Dispose());
        }

        private void DeactivateNode(IRiakNode node)
        {
            lock (_nodeMoveLock)
            {
                if (_liveNodes.ContainsKey(node.Name))
                {
                    _liveNodes.Remove(node.Name);
                    _brokenNodes.Add(node.Name, node);
                    _activeNodes = _liveNodes.Values.ToList();
                }
            }
        }
    }
}
