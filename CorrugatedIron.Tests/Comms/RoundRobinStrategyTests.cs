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
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CorrugatedIron.Tests.Comms.RoundRobinStrategyTests
{
    [TestFixture]
    public class WhenAddingAndRemovingNodesConstantlyOnDifferentThreads
    {
        [Test]
        public void NoExceptionsShouldBeThrown()
        {
            // three sets for three different threads
            var nodes = new[] { CreateMockNodes(), CreateMockNodes(), CreateMockNodes() };
            var roundRobin = new RoundRobinStrategy();

            roundRobin.Initialise(nodes.SelectMany(n => n));

            var results = new Exception[3];

            Parallel.For(0, 3, i =>
            {
                results[i] = DoStuffWithNodes(roundRobin, CreateMockNodes());
            });

            foreach (var result in results)
            {
                Assert.IsNull(result);
            }
        }

        private static Exception DoStuffWithNodes(ILoadBalancingStrategy strategy, IEnumerable<IRiakNode> nodes)
        {
            var rnd = new Random();
            var availableNodes = new Queue<IRiakNode>(nodes);
            var unavailableNodes = new Queue<IRiakNode>();

            try
            {
                for (var i = 0; i < 1000000; ++i)
                {
                    switch (rnd.Next(0, 3))
                    {
                        case 1:
                            strategy.SelectNode();
                            break;
                        case 2:
                            if (unavailableNodes.Count > 0)
                            {
                                var node = unavailableNodes.Dequeue();
                                strategy.AddNode(node);
                                availableNodes.Enqueue(node);
                            }
                            else
                            {
                                --i;
                            }
                            break;
                        default:
                            if (availableNodes.Count > 0)
                            {
                                var node = availableNodes.Dequeue();
                                strategy.RemoveNode(node);
                                unavailableNodes.Enqueue(node);
                            }
                            else
                            {
                                --i;
                            }
                            break;

                    }
                }

                while (availableNodes.Count > 0)
                {
                    strategy.RemoveNode(availableNodes.Dequeue());
                }

                strategy.SelectNode();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        private static IEnumerable<IRiakNode> CreateMockNodes()
        {
            var nodes = new List<IRiakNode>();
            for (var i = 0; i < 10; ++i)
            {
                nodes.Add(new Mock<IRiakNode>().Object);
            }
            return nodes;
        }
    }
}
