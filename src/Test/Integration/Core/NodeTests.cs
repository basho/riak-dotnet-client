namespace Test.Integration.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
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

            using (var l = new TestListener(onConn))
            {
                var w = l.Start();

                var o = new NodeOptions(
                    l.EndPoint,
                    count,
                    count,
                    timeout,
                    timeout,
                    timeout,
                    healthCheckInterval,
                    healthCheckBuilder);
                using (var n = new Node(o))
                {
                    await n.StartAsync();
                }

                Assert.AreEqual(count, connCount);
                l.Wait(w);
            }
        }

        [Test, Ignore("TODO 3.0 CLIENTS-621")]
        public async Task Node_Recovers_From_Connection_Error_Via_Ping_Check()
        {
            ushort connects = 0;
            TimeSpan timeout = Constants.ThirtySeconds;
            TimeSpan fiftyMillis = TimeSpan.FromMilliseconds(50);
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

            var observedStates = new List<Node.State>();
            Action<Node.State> observer = (s) =>
            {
                observedStates.Add(s);
            };

            using (var l = new TestListener(onConnAsync: oca))
            {
                var w = l.Start();

                var o = new NodeOptions(address: l.EndPoint, minConnections: 1, healthCheckInterval: fiftyMillis);
                using (var n = new TestNode(o, observer))
                {
                    await n.StartAsync();

                    IRCommand cmd = new Ping();

                    ExecuteResult r = await n.ExecuteAsync(cmd);
                    Assert.True(r.Executed, "expected Ping to be executed");
                    Assert.Null(r.Error, "expected Ping Error to be null");

                    // NB: wait until we see expected four states
                    // then continue to shut down node
                    while (observedStates.Count < 4)
                    {
                        await Task.Delay(500);
                    }
                }

                l.Wait(w);
            }

            // NB: wait until we see expected three states
            // then continue to shut down node
            while (observedStates.Count < expectedStates.Length)
            {
                await Task.Delay(500);
            }

            CollectionAssert.AreEqual(expectedStates, observedStates);
        }
    }
}
