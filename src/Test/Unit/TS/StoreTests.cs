namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    public class StoreTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            Store cmd = BuildStoreReq();
            Assert.AreEqual(MessageCode.TsPutResp, cmd.ExpectedCode);

            TsPutReq pb = (TsPutReq)cmd.ConstructRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));

            CollectionAssert.AreEqual(TsCols, pb.columns);
            CollectionAssert.AreEqual(TsRows, pb.rows);
        }
    }
}
