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
        public void Read_Zero_Throws_At_End()
        {
            byte[] inbuf = { 0, 1, 2, 3 };
            byte[] outbuf = new byte[1];
            using (var s = new OtpInputStream(inbuf))
            {
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                s.ReadN(outbuf);
                Assert.Throws(typeof(Exception), () => s.ReadN(outbuf, 0, 0));
            }
        }

        [Test]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.AtomTag, 0, 4, 116, 114, 117, 101 }, true)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.AtomTag, 0, 5, 102, 97, 108, 115, 101 }, false)]
        public void Can_Peek(byte[] buf, bool want)
        {
            using (var s = new OtpInputStream(buf))
            {
                byte b = s.Read1();
                Assert.AreEqual(OtpExternal.VersionTag, b);
                b = s.Peek();
                Assert.AreEqual(OtpExternal.AtomTag, b);
                string atom = s.ReadAtom();
                Assert.AreEqual(want.ToString().ToLowerInvariant(), atom); 
            }
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

        [Test]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.AtomTag, 0, 18, 102, 114, 97, 122, 122, 108, 101, 100, 97, 122, 122, 108, 101, 45, 49, 50, 51, 52 }, "frazzledazzle-1234")]
        [TestCase(new byte[] { OtpExternal.AtomTag, 0, 7, 121, 111, 64, 109, 97, 109, 97 }, "yo@mama")]
        public void Read_Atom(byte[] buf, string want)
        {
            using (var s = new OtpInputStream(buf))
            {
                string atom = s.ReadAtom();
                Assert.AreEqual(want, atom);
            }
        }

        [Test]
        [TestCase("The quick brown fox jumped over the lazy dog.")]
        [TestCase("时间序列")]
        [TestCase("временные ряды")]
        public void Read_Binary(string want)
        {
            byte[] buf = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteStringAsBinary(want);
                buf = os.ToArray();
            }

            using (var s = new OtpInputStream(buf))
            {
                string got = s.ReadBinaryAsString();
                Assert.AreEqual(want, got);
            }
        }

        [Test]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.NewFloatTag, 64, 147, 74, 69, 109, 92, 250, 173 }, 1234.5678D)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.NewFloatTag, 192, 18, 68, 155, 165, 227, 83, 248 }, -4.567d)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.NewFloatTag, 127, 239, 255, 255, 255, 255, 255, 255 }, double.MaxValue)]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.NewFloatTag, 255, 239, 255, 255, 255, 255, 255, 255 }, double.MinValue)]
        public void Read_Double(byte[] buf, double want)
        {
            using (var s = new OtpInputStream(buf))
            {
                double got = s.ReadDouble();
                Assert.AreEqual(want, got);
            }
        }

        [Test]
        [TestCase(new byte[] { OtpExternal.VersionTag, OtpExternal.FloatTag, 55, 46, 55, 53, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 101, 43, 48, 48, 0, 0, 0, 0, 0 },  7.75f)]
        [TestCase(new byte[] { OtpExternal.VersionTag,  OtpExternal.FloatTag, 45, 57, 46, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 57, 50, 57, 55, 53, 55, 101, 43, 52, 52, 0, 0, 0, 0 },  -999999999999999999999999999999999999999999999.99d)]
        public void Read_FloatAsDouble(byte[] buf, double want)
        {
            using (var s = new OtpInputStream(buf))
            {
                double got = s.ReadDouble();
                Assert.AreEqual(want, got);
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
    }
}
