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
using CorrugatedIron.Collections;
using CorrugatedIron.Config;
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
            return UseConnection(clientId, useFun, code => RiakResult.Error(code));
        }

        public RiakResult<TResult> UseConnection<TResult>(byte[] clientId, Func<IRiakConnection, RiakResult<TResult>> useFun)
        {
            return UseConnection(clientId, useFun, code => RiakResult<TResult>.Error(code));
        }

        private TRiakResult UseConnection<TRiakResult>(byte[] clientId, Func<IRiakConnection, TRiakResult> useFun, Func<ResultCode, TRiakResult> onError)
            where TRiakResult : RiakResult
        {
            if (_disposing) return onError(ResultCode.ShuttingDown);

            IRiakNode node;
            if (_roundRobin.TryMoveNext(out node))
            {
                var result = (TRiakResult)node.UseConnection(clientId, useFun);
                return result;
            }
            return onError(ResultCode.CommunicationError);
        }

        public void Dispose()
        {
            _disposing = true;

            _nodes.ForEach(n => n.Dispose());
        }
    }
}
