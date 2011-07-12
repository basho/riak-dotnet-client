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

using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Containers;

namespace CorrugatedIron.Comms.LoadBalancing
{
    public class RoundRobinStrategy : ILoadBalancingStrategy
    {
        private readonly object _nodesLock = new object();
        private List<IRiakNode> _nodes;
        private IConcurrentEnumerator<IRiakNode> _roundRobin;

        public void Initialise(IEnumerable<IRiakNode> nodes)
        {
            _nodes = nodes.ToList();

            _roundRobin = new ConcurrentEnumerable<IRiakNode>(RoundRobin()).GetEnumerator();
        }

        public IRiakNode SelectNode()
        {
            IRiakNode node = null;
            if (_roundRobin.TryMoveNext(out node))
            {
                return node;
            }
            return null;
        }

        public void RemoveNode(IRiakNode node)
        {
            lock (_nodesLock)
            {
                _nodes.Remove(node);
            }
        }

        public void AddNode(IRiakNode node)
        {
            lock (_nodesLock)
            {
                _nodes.Add(node);
            }
        }

        private IEnumerable<IRiakNode> RoundRobin()
        {
            while (true)
            {
                var nodes = _nodes.GetEnumerator();
                while (nodes.MoveNext() && nodes.Current != null)
                {
                    yield return nodes.Current;
                }
            }
        }
    }
}
