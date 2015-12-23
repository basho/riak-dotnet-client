namespace Test.Unit.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, UnitTest]
    public class NodeManagerTests
    {
        private readonly IRCommand cmd = new Ping();

        [Test]
        public async void RoundRobinNodeManager_Tries_All_Nodes_When_Executing()
        {
            INodeManager m = new RoundRobinNodeManager();

            var nodes = new[]
            {
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
                new TestNode(),
            };

            var rslt = await m.ExecuteAsyncOnNode(nodes, cmd);

            ushort i = 0;
            foreach (var n in nodes)
            {
                Assert.True(n.ExecuteTried);
                Assert.AreEqual(i, n.ExecuteIdx);
                i++;
            }

            Assert.False(rslt.Executed);
        }

        [Test]
        public async void LeastExecutingNodeManager_Tries_Lowest_Count_Nodes_First_When_Executing()
        {
            INodeManager m = new LeastExecutingNodeManager(false);

            var nodes = new[]
            {
                new TestNode(1),
                new TestNode(4),
                new TestNode(1),
                new TestNode(6),
                new TestNode(1),
            };

            var rslt = await m.ExecuteAsyncOnNode(nodes, cmd);

            var lastExecuteCount = nodes[0].ExecuteCount;
            foreach (var n in nodes)
            {
                Assert.True(n.ExecuteTried);
                Assert.GreaterOrEqual(n.ExecuteCount, lastExecuteCount);
            }

            Assert.False(rslt.Executed);
        }

        [Test]
        public async void LeastExecutingNodeManager_Shuffles_Nodes()
        {
            INodeManager m = new LeastExecutingNodeManager(true);

            var nodes = new[]
            {
                new TestNode(1),
                new TestNode(4),
                new TestNode(1),
                new TestNode(6),
                new TestNode(1),
            };

            var rslt = await m.ExecuteAsyncOnNode(nodes, cmd);

            int i = 0;
            var ne = TestNode.Executed;
            Assert.AreEqual(nodes.Length, ne.Count);

            for (; i < 3; i++)
            {
                Assert.True(ne[i].ExecuteTried);
                Assert.AreEqual(1, ne[i].ExecuteCount, string.Format("i: {0}", i));
            }

            for (; i < ne.Count; i++)
            {
                Assert.True(ne[i].ExecuteTried);
                Assert.GreaterOrEqual(ne[i].ExecuteCount, 1);
            }

            Assert.False(rslt.Executed);
        }

        [Test]
        public async void NodeManagers_Work_With_Single_Node()
        {
            var mgrs = new INodeManager[]
            {
                new RoundRobinNodeManager(),
                new LeastExecutingNodeManager(true)
            };

            foreach (var m in mgrs)
            {
                var nodes = new[]
                {
                    new TestNode(4)
                };

                var rslt = await m.ExecuteAsyncOnNode(nodes, cmd);

                var ne = TestNode.Executed;
                Assert.AreEqual(nodes.Length, ne.Count);

                Assert.True(ne[0].ExecuteTried);
                Assert.AreEqual(4, ne[0].ExecuteCount);

                Assert.False(rslt.Executed);
            }
        }

        private class TestNode : INode
        {
            private static ushort idx = 0;
            private static List<TestNode> executed = new List<TestNode>();

            private readonly ushort executeCount = 0;

            private bool executeTried = false;
            private ushort executeIdx = 0;

            public TestNode()
            {
                Reset();
            }

            public TestNode(ushort executeCount) : this()
            {
                this.executeCount = executeCount;
            }

            public static IList<TestNode> Executed
            {
                get
                {
                    return executed;
                }
            }

            public bool ExecuteTried
            {
                get
                {
                    return executeTried;
                }
            }

            public ushort ExecuteIdx
            {
                get
                {
                    return executeIdx;
                }
            }

            public int ExecuteCount
            {
                get
                {
                    return executeCount;
                }
            }

            public static void Reset()
            {
                idx = 0;
                if (executed.Count > 0)
                {
                    executed = new List<TestNode>();
                }
            }

            public Task<ExecuteResult> ExecuteAsync(IRCommand cmd)
            {
                executeTried = true;
                executeIdx = idx;
                idx++;
                executed.Add(this);
                return Task.FromResult(new ExecuteResult(executed: false));
            }
        }
    }
}
