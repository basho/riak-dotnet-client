namespace Test.Unit.Erlang
{
    using NUnit.Framework;
    using RiakClient.Erlang;

    [TestFixture, UnitTest]
    public class OtpOutputStreamTests
    {
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
