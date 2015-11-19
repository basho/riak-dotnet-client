namespace Test.Unit.Core
{
    using System;
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

            foreach (var n in nodes)
            {
                Assert.True(n.ExecuteTried);
            }

            Assert.False(rslt.Executed);
        }

        private class TestNode : INode
        {
            private bool executeTried = false;

            public bool ExecuteTried
            {
                get
                {
                    return executeTried;
                }
            }

            public Task<ExecuteResult> ExecuteAsync(IRCommand cmd)
            {
                executeTried = true;
                return Task.FromResult(new ExecuteResult(executed: false));
            }
        }
    }
}
