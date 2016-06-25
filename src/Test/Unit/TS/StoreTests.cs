namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Messages;

    public class StoreTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            IRCommand cmd = BuildStoreReq();
            Assert.AreEqual(MessageCode.TsPutResp, cmd.ResponseCode);

            TsPutReq pb = (TsPutReq)cmd.ConstructPbRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));

            CollectionAssert.AreEqual(TsCols, pb.columns);
            CollectionAssert.AreEqual(TsRows, pb.rows);
        }
    }
}
