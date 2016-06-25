namespace Test.Integration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, IntegrationTest]
    public class NodeTests
    {
        [Test]
        public async Task Creating_Node_And_Starting_Starts_MinConnections()
        {
            ushort count = 16;
            ushort connCount = 0;
            TimeSpan timeout = Constants.ThirtySeconds;
            TimeSpan healthCheckInterval = TimeSpan.FromMilliseconds(500);
            IRCommandBuilder healthCheckBuilder = new Ping.Builder();

            Func<TcpClient, bool> onConn = (client) =>
            {
                connCount++;
                client.Close();
                return connCount == count;
            };

            Node n = null;
            using (var l = new TestListener(onConn))
            {
                var o = new NodeOptions(
                    l.EndPoint,
                    count,
                    count,
                    timeout,
                    timeout,
                    timeout,
                    healthCheckInterval,
                    healthCheckBuilder);
                using (n = new Node(o))
                {
                    await n.StartAsync();

                    l.WaitOnConn();
                    Assert.AreEqual(count, connCount);
                }
            }

            Assert.AreEqual(Node.State.Shutdown, n.GetState());
        }

        [Test]
        public async Task Node_Recovers_From_Connection_Error_Via_Ping_Check()
        {
            ushort connects = 0;
            TimeSpan fiftyMillis = TimeSpan.FromMilliseconds(125);

            var expectedStates = new[]
            {
                Node.State.Running,
                Node.State.HealthChecking,
                Node.State.Running,
                Node.State.ShuttingDown,
                Node.State.Shutdown
            };

            Func<TcpClient, Task<bool>> oca = async (c) =>
            {
                connects++;

                if (connects == 1)
                {
                    c.Close();
                }
                else
                {
                    await TestListener.ReadWritePingRespAsync(c, true);
                }

                return true;
            };

            var evt = new ManualResetEvent(false);
            var observedStates = new List<Node.State>();
            Action<Node.State> observer = (s) =>
            {
                observedStates.Add(s);
                if (observedStates.Count == 3 || observedStates.Count == expectedStates.Length)
                {
                    evt.Set();
                }
            };

            using (var l = new TestListener(onConnAsync: oca))
            {
                var o = new NodeOptions(address: l.EndPoint, minConnections: 1, healthCheckInterval: fiftyMillis);
                using (var n = new TestNode(o, observer))
                {
                    await n.StartAsync();

                    IRCommand cmd = new Ping();
                    ExecuteResult r = await n.ExecuteAsync(cmd);
                    Assert.False(r.Executed);

                    Assert.True(evt.WaitOne());
                    Assert.True(evt.Reset());
                }
            }

            // NB: wait until we see expected three states
            // then continue to shut down node
            Assert.True(evt.WaitOne());

            CollectionAssert.AreEqual(expectedStates, observedStates);
        }
    }
}
