namespace Test.Unit.Core
{
    using System;
    using System.Net;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;

    [TestFixture, UnitTest]
    public class ConnectionTests
    {
        [Test]
        public void Creating_ConnectionOptions_Requires_Valid_IPAddress_Argument()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new ConnectionOptions((string)null, 8087);
            });

            Assert.Throws<FormatException>(() =>
            {
                new ConnectionOptions(string.Empty, 8087);
            });

            Assert.Throws<FormatException>(() =>
            {
                new ConnectionOptions("1,2.3.4", 8087);
            });
        }

        [Test]
        public void Creating_ConnectionOptions_Requires_Valid_Port_Argument()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new ConnectionOptions(IPAddress.Parse("10.0.0.1"), 0);
            });
        }

        [Test]
        public void ConnectionOptions_Uses_Default_Timeouts()
        {
            var opts = new ConnectionOptions("10.0.0.1", 8087);
            Assert.AreEqual(Constants.DefaultConnectTimeout, opts.ConnectTimeout);
            Assert.AreEqual(Constants.DefaultRequestTimeout, opts.RequestTimeout);
        }

        [Test]
        public void ConnectionOptions_With_Custom_Timeouts_Should_Use_Those_Timeouts()
        {
            var ct = TimeSpan.FromMilliseconds(1234);
            var rt = TimeSpan.FromMilliseconds(5678);
            var opts = new ConnectionOptions(new IPEndPoint(IPAddress.Parse("10.0.0.1"), 8087), ct, rt);
            Assert.AreEqual(1234, (int)opts.ConnectTimeout.TotalMilliseconds);
            Assert.AreEqual(5678, (int)opts.RequestTimeout.TotalMilliseconds);
        }

        [Test]
        public void Creating_Connection_Without_Options_Should_Throw()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Connection(null);
            });
        }

        [Test]
        public void Creating_Connection_With_Bad_Address_Should_Throw()
        {
            Assert.Throws<FormatException>(() =>
            {
                var opts = new ConnectionOptions("FOO.BAR.BAZ.BAT", 8087);
                new Connection(opts);
            });
        }

        [Test]
        public void New_Connection_Should_Be_In_Created_State()
        {
            var o = new ConnectionOptions(IPAddress.Parse("10.0.0.1"), 8087);
            var c = new Connection(o);
            Assert.AreEqual(Connection.State.Created, c.GetState());
        }
    }
}
