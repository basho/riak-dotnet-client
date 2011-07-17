using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorrugatedIron.Comms;
using CorrugatedIron.Comms.LoadBalancing;
using Moq;
using NUnit.Framework;

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

            var waitHandles = new[] { new ManualResetEvent(false), new ManualResetEvent(false), new ManualResetEvent(false) };

            var results = new Exception[3];

            for (var i = 0; i < 3; ++i)
            {
                var x = i;
                Action<Exception> action = ex => results[x] = ex;
                ThreadPool.QueueUserWorkItem(DoStuffWithNodes, Tuple.Create(roundRobin, nodes[i], waitHandles[i], action));
            }

            foreach (var handle in waitHandles)
            {
                handle.WaitOne();
            }

            foreach (var result in results)
            {
                Assert.IsNull(result);
            }
        }

        private static void DoStuffWithNodes(object state)
        {
            var data = (Tuple<RoundRobinStrategy, IRiakNode[], ManualResetEvent, Action<Exception>>)state;
            var rnd = new Random();
            var availableNodes = new Queue<IRiakNode>(data.Item2);
            var unavailableNodes = new Queue<IRiakNode>();

            try
            {
                for (var i = 0; i < 1000000; ++i)
                {
                    switch (rnd.Next(0, 3))
                    {
                        case 1:
                            data.Item1.SelectNode();
                            break;
                        case 2:
                            if (unavailableNodes.Count > 0)
                            {
                                var node = unavailableNodes.Dequeue();
                                data.Item1.AddNode(node);
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
                                data.Item1.RemoveNode(node);
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
                    data.Item1.RemoveNode(availableNodes.Dequeue());
                }

                data.Item1.SelectNode();
            }
            catch (Exception ex)
            {
                data.Item4(ex);
            }

            data.Item3.Set();
        }

        private static IRiakNode[] CreateMockNodes()
        {
            var nodes = new List<IRiakNode>();
            for (var i = 0; i < 10; ++i)
            {
                nodes.Add(new Mock<IRiakNode>().Object);
            }
            return nodes.ToArray();
        }
    }
}
