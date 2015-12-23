namespace Test.Integration.Core
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, IntegrationTest]
    public class ConnectionTests
    {
        [Test]
        public async Task Create_Connection_And_Start_Should_Connect()
        {
            bool sawConn = false;

            Func<TcpClient, bool> onConn = (client) =>
            {
                sawConn = true;
                client.Close();
                return true;
            };

            Connection c = null;
            using (var l = new TestListener(onConn))
            {
                var o = new ConnectionOptions(l.EndPoint);
                using (c = new Connection(o))
                {
                    Assert.AreEqual(Connection.State.Created, c.GetState());
                    await c.ConnectAsync();

                    l.WaitOnConn();
                    Assert.True(sawConn);
                    Assert.AreEqual(Connection.State.Active, c.GetState());
                }

                Assert.AreEqual(Connection.State.Inactive, c.GetState());
            }
        }

        [Test]
        public async Task Execute_Ping_On_Closed_Connection_Should_Throw_Exception()
        {
            bool sawConn = false;

            Func<TcpClient, bool> onConn = (client) =>
            {
                sawConn = true;
                client.Close();
                return true;
            };

            using (var l = new TestListener(onConn))
            {
                var o = new ConnectionOptions(l.EndPoint);
                Connection c = null;
                using (c = new Connection(o))
                {
                    Assert.AreEqual(Connection.State.Created, c.GetState());
                    await c.ConnectAsync();

                    l.WaitOnConn();
                    Assert.True(sawConn);
                    Assert.AreEqual(Connection.State.Active, c.GetState());

                    var cmd = new Ping();
                    Assert.Throws<IOException>(async () =>
                    {
                        await c.ExecuteAsync(cmd);
                    });
                }

                Assert.AreEqual(Connection.State.Inactive, c.GetState());
            }
        }
    }
}
