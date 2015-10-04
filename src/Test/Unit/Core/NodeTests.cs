namespace Test.Unit.Core
{
    using System;
    using System.Net;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient.Commands;

    [TestFixture, UnitTest]
    public class NodeTests
    {
        [Test]
        public void Creating_NodeOptions_Requires_Valid_Arguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new NodeOptions(null);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var e = new IPEndPoint(IPAddress.Loopback, 0);
                new NodeOptions(e);
            });
        }

        [Test]
        public void Creating_NodeOptions_Uses_Default_Arguments()
        {
            var e = new IPEndPoint(IPAddress.Loopback, 8087);
            var o = new NodeOptions(e);

            Assert.AreEqual(o.Address.Address, IPAddress.Loopback);
            Assert.AreEqual(o.Address.Port, 8087);
            Assert.AreEqual(o.MinConnections, Constants.DefaultMinConnections);
            Assert.AreEqual(o.MaxConnections, Constants.DefaultMaxConnections);
            Assert.AreEqual(o.IdleTimeout, Constants.DefaultIdleTimeout);
            Assert.AreEqual(o.ConnectTimeout, Constants.DefaultConnectTimeout);
            Assert.AreEqual(o.RequestTimeout, Constants.DefaultRequestTimeout);
            Assert.AreEqual(o.HealthCheckInterval, Constants.DefaultHealthCheckInterval);
            Assert.IsAssignableFrom<Ping.Builder>(o.HealthCheckBuilder);
        }

        [Test]
        public void Creating_Node_Requires_Valid_Arguments()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Node(null);
            });
        }

        [Test]
        public void Creating_Node_Sets_State()
        {
            var a = new IPEndPoint(IPAddress.Loopback, 8087);
            var o = new NodeOptions(a);
            var n = new Node(o);

            Assert.AreEqual(Node.State.Created, n.GetState());
        }
    }
}
