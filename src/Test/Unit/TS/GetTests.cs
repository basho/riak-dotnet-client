namespace Test.Unit.TS
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class GetTests
    {
        private static readonly RiakString Table = "GeoCheckin";
        private static readonly Cell[] Cells = new Cell[]
            {
                new Cell<bool>(false),
                new Cell<double>(12.34F),
                new Cell<long>(32),
                new Cell<DateTime>(DateTime.Now),
                new Cell<string>("foobar"),
            };

        private static readonly Row Key = new Row(Cells);

        [Test]
        public void Should_Build_Req()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .Build();

            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructPbRequest();
            Assert.IsFalse(pb.timeoutSpecified);

            Assert.True(pb.key[0].boolean_valueSpecified);
            Assert.AreEqual(Cells[0].AsObject, pb.key[0].boolean_value);

            Assert.True(pb.key[1].double_valueSpecified);
            Assert.AreEqual(Cells[1].AsObject, pb.key[1].double_value);

            Assert.True(pb.key[2].sint64_valueSpecified);
            Assert.AreEqual(Cells[2].AsObject, pb.key[2].sint64_value);

            var dt = (DateTime)Cells[3].AsObject;
            Assert.True(pb.key[3].timestamp_valueSpecified);
            Assert.AreEqual(dt.ToUnixTimeMillis(), pb.key[3].timestamp_value);

            var s = RiakString.ToBytes((string)Cells[4].AsObject);
            Assert.True(pb.key[4].varchar_valueSpecified);
            CollectionAssert.AreEqual(s, pb.key[4].varchar_value);
        }

        [Test]
        public void Should_Build_Req_With_Timeout()
        {
            var timeout = TimeSpan.FromSeconds(1);
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .WithTimeout(timeout)
                .Build();

            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructPbRequest();
            Assert.IsTrue(pb.timeoutSpecified);
            Assert.AreEqual(timeout.TotalMilliseconds, pb.timeout);
        }
    }
}
