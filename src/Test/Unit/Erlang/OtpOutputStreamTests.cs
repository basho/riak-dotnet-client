namespace Test.Unit.Erlang
{
    using System;
    using NUnit.Framework;
    using RiakClient.Erlang;

    [TestFixture, UnitTest]
    public class OtpOutputStreamTests
    {
        [Test]
        public void Write_Byte()
        {
            byte want = 123;
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write(want);
                got = os.ToArray();
            }

            Assert.AreEqual(want, got[0]);
        }

        [Test]
        public void Write_2BE()
        {
            byte[] want = { 0xAB, 0xCD };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write2BE(0xFFFFABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_4BE()
        {
            byte[] want = { 0x98, 0x76,  0xAB, 0xCD };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write4BE(0x9876ABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_8BE()
        {
            byte[] want = { 0x12, 0xAD, 0xBE, 0xEF, 0x98, 0x76,  0xAB, 0xCD };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write8BE(0x12ADBEEF9876ABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_LE()
        {
            int val = 0x1234BEEF;
            int sz = sizeof(int);

            byte[] want = { 0xEF, 0xBE, 0x34, 0x12 };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.WriteLE(val, sz);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);

            Assert.IsTrue(BitConverter.IsLittleEndian);
            want = BitConverter.GetBytes(val);
            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_2LE()
        {
            byte[] want = { 0xCD, 0xAB };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write2LE(0xFFFFABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_4LE()
        {
            byte[] want = { 0xCD, 0xAB,  0x76, 0x98 };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write4LE(0x9876ABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_8LE()
        {
            byte[] want = { 0xCD, 0xAB, 0x76, 0x98, 0xEF, 0xBE,  0xAD, 0x12 };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write8LE(0x12ADBEEF9876ABCD);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_Boolean()
        {
            byte[] want = { 131, 100, 0, 4, 116, 114, 117, 101 };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write(OtpExternal.VersionTag);
                os.WriteBoolean(true);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }
    }
}
