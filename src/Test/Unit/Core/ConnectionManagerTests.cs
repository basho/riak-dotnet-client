namespace Test.Unit.Core
{
    using System;
    using System.Net;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;

    [TestFixture, UnitTest]
    public class ConnectionManagerTests
    {
        [Test]
        public void ConnectionManagerOptions_Uses_Default_Values()
        {
            const ushort Port = 1234;

            var addr = new IPEndPoint(IPAddress.Loopback, Port);

            var opts = new ConnectionManagerOptions(
                addr,
                default(ushort),
                default(ushort),
                default(TimeSpan),
                default(TimeSpan),
                default(TimeSpan),
                default(TimeSpan));

            Assert.AreEqual(Constants.DefaultMinConnections, opts.MinConnections);
            Assert.AreEqual(Constants.DefaultMaxConnections, opts.MaxConnections);
            Assert.AreEqual(Constants.DefaultIdleExpirationInterval, opts.IdleExpirationInterval);
            Assert.AreEqual(Constants.DefaultIdleTimeout, opts.IdleTimeout);
            Assert.AreEqual(Constants.DefaultConnectTimeout, opts.ConnectTimeout);
            Assert.AreEqual(Constants.DefaultRequestTimeout, opts.RequestTimeout);
        }
    }
}
