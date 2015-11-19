namespace Test.Unit.Core
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, UnitTest]
    public class NodeManagerTests
    {
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

            var cmd = new Ping();

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
            INodeManager m = new LeastExecutingNodeManager();

            var nodes = new[]
            {
                new TestNode(1),
                new TestNode(4),
                new TestNode(1),
                new TestNode(6),
                new TestNode(1),
            };

            var cmd = new Ping();

            var rslt = await m.ExecuteAsyncOnNode(nodes, cmd);

            ushort lastExecuteCount = nodes[0].ExecuteCount;
            foreach (var n in nodes)
            {
                Assert.True(n.ExecuteTried);
                Assert.GreaterOrEqual(n.ExecuteCount, lastExecuteCount);
            }

            Assert.False(rslt.Executed);
        }

        private class TestNode : INode
        {
            private static ushort idx = 0;

            private readonly ushort executeCount = 0;

            private bool executeTried = false;
            private ushort executeIdx = 0;

            public TestNode()
            {
            }

            public TestNode(ushort executeCount)
            {
                this.executeCount = executeCount;
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

            public ushort ExecuteCount
            {
                get
                {
                    return executeCount;
                }
            }

            public Task<ExecuteResult> ExecuteAsync(IRCommand cmd)
            {
                executeTried = true;
                executeIdx = idx;
                idx++;
                return Task.FromResult(new ExecuteResult(executed: false));
            }
        }
    }
}
