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
        RiakResult<IEnumerable<TResult>> UseStreamConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun);
    }

    public class RiakCluster : IRiakCluster
    {
        private bool _disposing;
        private readonly List<IRiakNode> _nodes;
        private readonly IConcurrentEnumerator<IRiakNode> _roundRobin;

        public RiakCluster(IRiakClusterConfiguration clusterConfiguration, IRiakNodeFactory nodeFactory, IRiakConnectionFactory connectionFactory)
        {
            _nodes = clusterConfiguration.RiakNodes.Select(rn => nodeFactory.CreateNode(rn, connectionFactory)).ToList();
            
            _roundRobin = new ConcurrentEnumerable<IRiakNode>(_nodes.Cycle()).GetEnumerator();
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

            IRiakNode node;
            if (_roundRobin.TryMoveNext(out node))
            {
                var result = node.UseConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        // TODO: this is where we need to retry on another node
                        //return UseConnection(clientId, useFun, onError);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        // TODO: pull this node from the cluster and retry on another node
                        // DeactivateNode(node);
                        //return UseConnection(clientId, useFun, onError);
                    }

                    // use the onError function so that we know the return value is the right type
                    return onError(result.ResultCode, result.ErrorMessage);
                }
                return (TRiakResult)result;
            }
            return onError(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        public RiakResult<IEnumerable<TResult>> UseStreamConnection<TResult>(byte[] clientId, Func<IRiakConnection, Action, RiakResult<IEnumerable<TResult>>> useFun)
        {
            if (_disposing) return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ShuttingDown, "System currently shutting down");

            IRiakNode node;
            if (_roundRobin.TryMoveNext(out node))
            {
                var result = node.UseStreamConnection(clientId, useFun);
                if (!result.IsSuccess)
                {
                    if (result.ResultCode == ResultCode.NoConnections)
                    {
                        // TODO: this is where we need to retry on another node
                        //return UseConnection(clientId, useFun, onError);
                    }

                    if (result.ResultCode == ResultCode.CommunicationError)
                    {
                        // TODO: pull this node from the cluster and retry on another node
                        // DeactivateNode(node);
                        //return UseConnection(clientId, useFun, onError);
                    }
                }
                return result;
            }
            return RiakResult<IEnumerable<TResult>>.Error(ResultCode.ClusterOffline, "Unable to access functioning Riak node");
        }

        public void Dispose()
        {
            _disposing = true;

            _nodes.ForEach(n => n.Dispose());
        }
    }
}
