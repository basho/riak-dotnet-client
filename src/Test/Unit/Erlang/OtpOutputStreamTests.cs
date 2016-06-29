namespace Test.Unit.Erlang
{
    using System;
    using System.Text;
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
        public void Write_Atom()
        {
            byte[] want = { 131, 100, 0, 18, 102, 114, 97, 122, 122, 108, 101, 100, 97, 122, 122, 108, 101, 45, 49, 50, 51, 52 };
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write(OtpExternal.VersionTag);
                os.WriteAtom("frazzledazzle-1234");
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        public void Write_Binary()
        {
            var r = new Random();
            byte[] rnd = new byte[65536];
            r.NextBytes(rnd);

            byte[] want = BuildBinBuffer(rnd);
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.WriteBinary(rnd);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        [TestCase("The quick brown fox jumped over the lazy dog.")]
        [TestCase("时间序列")]
        [TestCase("временные ряды")]
        public void Write_String_As_Binary(string s)
        {
            byte[] str = Encoding.UTF8.GetBytes(s);
            byte[] want = BuildBinBuffer(str);
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.WriteStringAsBinary(s);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        // Fun with doubles!
        // Max double value:
        // Bin = <<131,70,127,239,255,255,255,255,255,255>>, rp(binary_to_term(Bin)).
        // 1.7976931348623157e308
        // double.MaxValue.ToString("R")
        // 1.7976931348623157E+308
        // Min double value:
        // Bin = <<131,70,255,239,255,255,255,255,255,255>>, rp(binary_to_term(Bin)).
        // -1.7976931348623157e308
        // double.MinValue.ToString("R")
        // -1.7976931348623157E+308
        [Test]
        [TestCase(1234.5678D, new byte[] { 131, 70, 64, 147, 74, 69, 109, 92, 250, 173 })]
        [TestCase(-4.567D, new byte[] { 131, 70, 192, 18, 68, 155, 165, 227, 83, 248 })]
        [TestCase(double.MaxValue, new byte[] { 131, 70, 127, 239, 255, 255, 255, 255, 255, 255 })]
        [TestCase(double.MinValue, new byte[] { 131, 70, 255, 239, 255, 255, 255, 255, 255, 255 })]
        public void Write_Double(double d, byte[] want)
        {
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.WriteByte(OtpExternal.VersionTag);
                os.WriteDouble(d);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        [TestCase(byte.MaxValue, new byte[] { 131, 97, 255 })]
        [TestCase(byte.MinValue, new byte[] { 131, 97, 0 })]
        [TestCase(256, new byte[] { 131, 98, 0, 0, 1, 0 })] // byte.MaxValue + 1
        [TestCase(int.MaxValue, new byte[] { 131, 98, 127, 255, 255, 255 })]
        [TestCase(int.MinValue, new byte[] { 131, 98, 128, 0, 0, 0 })]
        [TestCase(2147483648, new byte[] { 131, 110, 4, 0, 0, 0, 0, 128 })] // int.MaxValue + 1
        [TestCase(-2147483649, new byte[] { 131, 110, 4, 1, 1, 0, 0, 128 })] // int.MinValue - 1
        [TestCase(long.MaxValue, new byte[] { 131, 110, 8, 0, 255, 255, 255, 255, 255, 255, 255, 127 })]
        [TestCase(long.MinValue, new byte[] { 131, 110, 8, 1, 0, 0, 0, 0, 0, 0, 0, 128 })]
        public void Write_Long(long l, byte[] want)
        {
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.WriteByte(OtpExternal.VersionTag);
                os.WriteLong(l);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        [Test]
        [TestCase(true, new byte[] { 131, 100, 0, 4, 116, 114, 117, 101 })]
        [TestCase(false, new byte[] { 131, 100, 0, 5, 102, 97, 108, 115, 101 })]
        public void Write_Boolean(bool b, byte[] want)
        {
            byte[] got;
            using (var os = new OtpOutputStream())
            {
                os.Write(OtpExternal.VersionTag);
                os.WriteBoolean(b);
                Assert.AreEqual(want.Length, os.Position);
                got = os.ToArray();
            }

            CollectionAssert.AreEqual(want, got);
        }

        private static byte[] BuildBinBuffer(byte[] src)
        {
            int totalLen = src.Length + 5;
            byte[] dst = new byte[totalLen];
            dst[0] = OtpExternal.BinTag;

            byte[] len = BitConverter.GetBytes(src.Length);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(len);
            }

            Buffer.BlockCopy(len, 0, dst, 1, len.Length);
            Buffer.BlockCopy(src, 0, dst, 5, src.Length);

            return dst;
        }
    }
}
