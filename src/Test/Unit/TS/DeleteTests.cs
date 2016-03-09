namespace Test.Unit.TS
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

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
            Assert.IsFalse(pb.timeoutSpecified);
        }
    }
}
