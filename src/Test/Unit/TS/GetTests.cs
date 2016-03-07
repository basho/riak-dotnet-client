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

        [Test]
        public void Should_Build_Req_Correctly()
        {
            var now = DateTime.Now;

            var cells = new Cell[]
            {
                new Cell<bool>(false),
                new Cell<double>(12.34F),
                new Cell<long>(32),
                new Cell<DateTime>(now),
                new Cell<string>("foobar"),
            };

            var key = new Row(cells);

            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(key)
                .Build();

            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructPbRequest();

            Assert.True(pb.key[0].boolean_valueSpecified);
            Assert.AreEqual(cells[0].AsObject, pb.key[0].boolean_value);

            Assert.True(pb.key[1].double_valueSpecified);
            Assert.AreEqual(cells[1].AsObject, pb.key[1].double_value);

            Assert.True(pb.key[2].sint64_valueSpecified);
            Assert.AreEqual(cells[2].AsObject, pb.key[2].sint64_value);

            var dt = (DateTime)cells[3].AsObject;
            Assert.True(pb.key[3].timestamp_valueSpecified);
            Assert.AreEqual(dt.ToUnixTimeMillis(), pb.key[3].timestamp_value);

            var s = RiakString.ToBytes((string)cells[4].AsObject);
            Assert.True(pb.key[4].varchar_valueSpecified);
            CollectionAssert.AreEqual(s, pb.key[4].varchar_value);
        }
    }
}
