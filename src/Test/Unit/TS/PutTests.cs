namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class PutTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            var cmd = new Put.Builder()
                .WithTable(Table)
                .Build();

            Assert.AreEqual(MessageCode.TsPutResp, cmd.ExpectedCode);

            TsPutReq pb = (TsPutReq)cmd.ConstructPbRequest();
        }
    }
}
