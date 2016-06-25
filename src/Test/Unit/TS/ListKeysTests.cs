namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    public class ListKeysTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            var cmd = new ListKeys.Builder()
                .WithTable(Table)
                .Build();

            Assert.AreEqual(MessageCode.TsListKeysResp, cmd.ResponseCode);

            TsListKeysReq pb = (TsListKeysReq)cmd.ConstructPbRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));
            Assert.IsFalse(pb.timeoutSpecified);
        }

        [Test]
        public void Should_Build_Req_With_Timeout()
        {
            var cmd = new ListKeys.Builder()
                .WithTable(Table)
                .WithTimeout(Timeout)
                .Build();

            Assert.AreEqual(MessageCode.TsListKeysResp, cmd.ResponseCode);

            TsListKeysReq pb = (TsListKeysReq)cmd.ConstructPbRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));

            Assert.IsTrue(pb.timeoutSpecified);
            Assert.AreEqual(Timeout.TotalMilliseconds, pb.timeout);
        }
    }
}
