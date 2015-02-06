// <copyright file="RoundRobinStrategy.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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
// </copyright>

namespace RiakClient.Comms.LoadBalancing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Containers;

    public class RoundRobinStrategy : ILoadBalancingStrategy
    {
        private readonly object nodesLock = new object();
        private IList<IRiakNode> nodes;
        private IConcurrentEnumerator<IRiakNode> roundRobin;

        public void Initialise(IEnumerable<IRiakNode> nodes)
        {
            this.nodes = nodes.ToList();
            this.roundRobin = new ConcurrentEnumerable<IRiakNode>(RoundRobin()).GetEnumerator();
        }

        public IRiakNode SelectNode()
        {
            IRiakNode node = null;

            if (roundRobin.TryMoveNext(out node))
            {
                return node;
            }

            return null;
        }

        public void RemoveNode(IRiakNode node)
        {
            lock (nodesLock)
            {
                if (nodes.Contains(node))
                {
                    nodes.Remove(node);
                }
            }
        }

        public void AddNode(IRiakNode node)
        {
            lock (nodesLock)
            {
                if (!nodes.Contains(node))
                {
                    nodes.Add(node);
                }
            }
        }

        private IEnumerable<IRiakNode> RoundRobin()
        {
            while (true)
            {
                IList<IRiakNode> list;

                lock (nodesLock)
                {
                    list = new List<IRiakNode>(nodes);
                }

                if (list.Count > 0)
                {
                    var nodes = list.GetEnumerator();
                    while (nodes.MoveNext() && nodes.Current != null)
                    {
                        yield return nodes.Current;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}
