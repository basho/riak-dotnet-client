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

            var l = new TestListener(onConn);
            var w = l.Start();

            var o = new ConnectionOptions(l.EndPoint);
            Connection c = null;
            using (c = new Connection(o))
            {
                Assert.AreEqual(Connection.ConnectionState.Created, c.State);
                await c.Connect();

                l.Wait(w);

                Assert.True(sawConn);
                Assert.AreEqual(Connection.ConnectionState.Active, c.State);
            }

            Assert.AreEqual(Connection.ConnectionState.Inactive, c.State);

            l.Stop();
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

            var l = new TestListener(onConn);
            var w = l.Start();

            var o = new ConnectionOptions(l.EndPoint);
            var c = new Connection(o);
            Assert.AreEqual(Connection.ConnectionState.Created, c.State);
            await c.Connect();

            l.Wait(w);

            Assert.True(sawConn);
            Assert.AreEqual(Connection.ConnectionState.Active, c.State);

            var cmd = new Ping();

            Assert.Throws<IOException>(async () =>
            {
                await c.Execute(cmd);
            });

            c.Close();
            l.Stop();
        }
    }
}
