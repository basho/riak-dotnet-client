namespace Test.Unit.Erlang
{
    using NUnit.Framework;
    using RiakClient.Erlang;

    [TestFixture, UnitTest]
    public class OtpInputStreamTests
    {
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
    }
}
