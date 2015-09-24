namespace Test.Integration.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, IntegrationTest]
    public class ConnectionTests
    {
        [Test]
        public async Task Create_Connection_And_Start_Should_Connect()
        {
            ushort port = 1337;
            bool sawConn = false;

            Func<TcpClient, bool> onConn = (client) =>
            {
                sawConn = true;
                client.Close();
                return true;
            };

            var l = new TestListener(port, onConn);
            var w = l.Start();

            var o = new ConnectionOptions(IPAddress.Loopback, port);
            var c = new Connection(o);
            Assert.AreEqual(ConnectionState.Created, c.State);
            await c.Connect();

            l.Wait(w);

            Assert.True(sawConn);
            Assert.AreEqual(ConnectionState.Active, c.State);

            c.Close();
            Assert.AreEqual(ConnectionState.Inactive, c.State);

            l.Stop();
        }

        [Test]
        public async Task Execute_Ping_On_Closed_Connection_Should_Throw_Exception()
        {
            ushort port = 1337;
            bool sawConn = false;

            Func<TcpClient, bool> onConn = (client) =>
            {
                sawConn = true;
                client.Close();
                return true;
            };

            var l = new TestListener(port, onConn);
            var w = l.Start();

            var o = new ConnectionOptions(IPAddress.Loopback, port);
            var c = new Connection(o);
            Assert.AreEqual(ConnectionState.Created, c.State);
            await c.Connect();

            l.Wait(w);

            Assert.True(sawConn);
            Assert.AreEqual(ConnectionState.Active, c.State);

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
