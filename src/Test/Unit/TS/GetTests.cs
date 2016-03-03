namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class GetTests
    {
        private static readonly RiakString Table = "GeoCheckin";
        private static readonly Row Key = new Row();

        [Test]
        public void Should_Build_Req_Correctly()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .Build();

            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq protobuf = (TsGetReq)cmd.ConstructPbRequest();
        }
    }
}
