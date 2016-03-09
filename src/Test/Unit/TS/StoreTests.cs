namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    public class StoreTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            Store cmd = BuildStoreReq();
            Assert.AreEqual(MessageCode.TsPutResp, cmd.ExpectedCode);

            TsPutReq pb = (TsPutReq)cmd.ConstructPbRequest();

            CollectionAssert.AreEqual(TsCols, pb.columns);
            CollectionAssert.AreEqual(TsRows, pb.rows);
        }
    }
}
