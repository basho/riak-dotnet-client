namespace Test.Unit.Erlang
{
    using System;
    using NUnit.Framework;
    using RiakClient.Erlang;

    [TestFixture, UnitTest]
    public class OtpInputStreamTests
    {
        [Test]
        public void Read_Throws_At_End()
        {
            byte[] inbuf = { 0, 1, 2, 3 };
            byte[] outbuf = new byte[1];
            using (var s = new OtpInputStream(inbuf))
            {
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                Assert.Throws(typeof(Exception), () => s.ReadN(outbuf));
            }
        }

        [Test]
        public void Read_Zero_Does_Not_Throw_At_End()
        {
            byte[] inbuf = { 0, 1, 2, 3 };
            byte[] outbuf = new byte[1];
            using (var s = new OtpInputStream(inbuf))
            {
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                Assert.DoesNotThrow(() => s.ReadN(outbuf, 0, 0));
                Assert.AreEqual(0, s.ReadN(outbuf, 0, 0));
            }
        }

        [Test]
        [TestCase(new byte[] { OtpExternal.SmallIntTag, 0 }, byte.MinValue)]
        [TestCase(new byte[] { OtpExternal.SmallIntTag, 255 }, byte.MaxValue)]
        [TestCase(new byte[] { OtpExternal.IntTag, 0, 0, 1, 0 }, 256)]
        [TestCase(new byte[] { OtpExternal.IntTag, 255, 255, 255, 255 }, -1)]
        [TestCase(new byte[] { OtpExternal.IntTag, 255, 255, 255, 133 }, -123)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.IntTag, 127, 255, 255, 255 }, int.MaxValue)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.IntTag, 128, 0, 0, 0 }, int.MinValue)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.SmallBigTag, 4, 0, 0, 0, 0, 128 }, 2147483648)] // int.MaxValue + 1
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.SmallBigTag, 4, 1, 1, 0, 0, 128 }, -2147483649)] // int.MinValue - 1
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.SmallBigTag, 8, 0, 255, 255, 255, 255, 255, 255, 255, 127 }, long.MaxValue)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.SmallBigTag, 8, 1, 0, 0, 0, 0, 0, 0, 0, 128 }, long.MinValue)]
        public void Read_Long(byte[] buf, long want)
        {
            long got = 0;
            using (var s = new OtpInputStream(buf))
            {
                got = s.ReadLong();
            }

            Assert.AreEqual(want, got);
        }

        [Test]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.AtomTag, 0, 4, 116, 114, 117, 101 }, true)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.AtomTag, 0, 5, 102, 97, 108, 115, 101 }, false)]
        public void Read_Boolean(byte[] buf, bool want)
        {
            using (var s = new OtpInputStream(buf))
            {
                Assert.AreEqual(want, s.ReadBoolean());
            }
        }
    }
}
