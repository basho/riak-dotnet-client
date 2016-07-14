namespace Test.Unit.Erlang
{
    using NUnit.Framework;
    using RiakClient.Erlang;
    using RiakClient.Exceptions;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class TtbErrorDecoderTests
    {
        private const string ErrMsg = "error-message";
        private const int ErrCode = 234;

        [Test]
        public void Can_Parse_Bare_RpbErrorResp()
        {
            byte[] b = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteAtom(TtbErrorDecoder.RpbErrorRespAtom);
                os.Flush();
                b = os.ToArray();
            }

            var ex = Assert.Throws<RiakException>(() => new TsTtbResp(b));
            Assert.IsTrue(ex.Message.Contains(TtbErrorDecoder.RpbErrorRespEmpty));
        }

        [Test]
        public void Can_Parse_RpbErrorResp_In_1_Tuple()
        {
            byte[] b = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteTupleHead(1);
                os.WriteAtom(TtbErrorDecoder.RpbErrorRespAtom);
                os.Flush();
                b = os.ToArray();
            }

            var ex = Assert.Throws<RiakException>(() => new TsTtbResp(b));
            Assert.IsTrue(ex.Message.Contains(TtbErrorDecoder.RpbErrorRespEmpty));
        }

        [Test]
        public void Can_Parse_RpbErrorResp_In_2_Tuple_With_String()
        {
            byte[] b = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteTupleHead(2);
                os.WriteAtom(TtbErrorDecoder.RpbErrorRespAtom);
                os.WriteStringAsBinary(ErrMsg);
                os.Flush();
                b = os.ToArray();
            }

            var ex = Assert.Throws<RiakException>(() => new TsTtbResp(b));
            Assert.IsTrue(ex.Message.Contains(ErrMsg));
        }

        [Test]
        public void Can_Parse_RpbErrorResp_In_2_Tuple_With_Code()
        {
            byte[] b = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteTupleHead(2);
                os.WriteAtom(TtbErrorDecoder.RpbErrorRespAtom);
                os.WriteLong(ErrCode);
                os.Flush();
                b = os.ToArray();
            }

            var ex = Assert.Throws<RiakException>(() => new TsTtbResp(b));
            Assert.IsTrue(ex.Message.Contains(ErrCode.ToString()));
        }

        [Test]
        public void Can_Parse_RpbErrorResp_In_3_Tuple()
        {
            byte[] b = null;
            using (var os = new OtpOutputStream())
            {
                os.WriteTupleHead(3);
                os.WriteAtom(TtbErrorDecoder.RpbErrorRespAtom);
                os.WriteLong(ErrCode);
                os.WriteStringAsBinary(ErrMsg);
                os.Flush();
                b = os.ToArray();
            }

            var ex = Assert.Throws<RiakException>(() => new TsTtbResp(b));
            Assert.IsTrue(ex.Message.Contains(ErrCode.ToString()));
            Assert.IsTrue(ex.Message.Contains(ErrMsg));
        }
    }
}
