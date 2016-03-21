namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    public class DeleteTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            var cmd = new Delete.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .Build();

            Assert.AreEqual(MessageCode.TsDelResp, cmd.ExpectedCode);

            TsDelReq pb = (TsDelReq)cmd.ConstructPbRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));
            Assert.IsFalse(pb.timeoutSpecified);
        }

        [Test]
        public void Should_Build_Req_With_Timeout()
        {
            var cmd = new Delete.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .WithTimeout(Timeout)
                .Build();

            Assert.AreEqual(MessageCode.TsDelResp, cmd.ExpectedCode);

            TsDelReq pb = (TsDelReq)cmd.ConstructPbRequest();
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));
            Assert.IsTrue(pb.timeoutSpecified);
            Assert.AreEqual(Timeout.TotalMilliseconds, pb.timeout);
        }
    }
}
