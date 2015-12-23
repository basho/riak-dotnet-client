namespace Test.Integration.Core
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak.Core;

    [TestFixture, IntegrationTest]
    public class ConnectionManagerTests
    {
        [Test]
        public async void Starting_ConnectionManager_Starts_Min_Connections()
        {
            ushort minConnections = 5;
            ushort maxConnections = 20;

            ushort connCount = 0;

            Func<TcpClient, bool> onConn = (client) =>
            {
                connCount++;
                client.Close();
                return connCount == minConnections; // NB: 'true' means close listener
            };

            using (var l = new TestListener(onConn))
            {
                var opts = new ConnectionManagerOptions(l.EndPoint, minConnections, maxConnections);
                using (var cm = new ConnectionManager(opts))
                {
                    await cm.StartAsync();
                }
            }

            Assert.AreEqual(minConnections, connCount);
        }

        [Test]
        public async Task ConnectionManager_Does_Not_Expire_Past_MinConnections()
        {
            ushort minConnections = 10;
            ushort maxConnections = 20;
            TimeSpan idleExpirationInterval = TimeSpan.FromMilliseconds(250);
            TimeSpan idleTimeout = TimeSpan.FromMilliseconds(10);

            using (var l = new TestListener())
            {
                var opts = new ConnectionManagerOptions(
                    l.EndPoint,
                    minConnections,
                    maxConnections,
                    idleExpirationInterval,
                    idleTimeout);

                using (var cm = new ConnectionManager(opts))
                {
                    await cm.StartAsync();

                    for (ushort i = 0; i < minConnections; i++)
                    {
                        await Task.Delay(125);
                        Assert.GreaterOrEqual(cm.ConnectionCount, minConnections);
                    }
                }
            }
        }
    }
}
