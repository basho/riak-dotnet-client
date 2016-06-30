namespace Test.Unit.TS
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

    public class GetTests : TimeseriesTest
    {
        [Test]
        public void Should_Build_Req()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .Build();

            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructRequest(false);
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));
            Assert.IsFalse(pb.timeoutSpecified);

            Assert.True(pb.key[0].boolean_valueSpecified);
            Assert.AreEqual(Cells0[0].Value, pb.key[0].boolean_value);

            Assert.True(pb.key[1].double_valueSpecified);
            Assert.AreEqual(Cells0[1].Value, pb.key[1].double_value);

            Assert.True(pb.key[2].sint64_valueSpecified);
            Assert.AreEqual(Cells0[2].Value, pb.key[2].sint64_value);

            var dt = (DateTime)Cells0[3].Value;
            Assert.True(pb.key[3].timestamp_valueSpecified);
            Assert.AreEqual(DateTimeUtil.ToUnixTimeMillis(dt), pb.key[3].timestamp_value);

            var s = RiakString.ToBytes((string)Cells0[4].Value);
            Assert.True(pb.key[4].varchar_valueSpecified);
            CollectionAssert.AreEqual(s, pb.key[4].varchar_value);
        }

        [Test]
        public void Should_Build_Req_With_Timeout()
        {
            Get cmd = BuildGetReqWithTimeout();
            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructRequest(false);
            Assert.AreEqual(Table, RiakString.FromBytes(pb.table));

            Assert.IsTrue(pb.timeoutSpecified);
            Assert.AreEqual(Timeout.TotalMilliseconds, pb.timeout);
        }

        [Test]
        public void Should_Parse_Resp()
        {
            var rsp = new TsGetResp();
            rsp.columns.AddRange(TsCols);
            rsp.rows.AddRange(TsRows);

            Get cmd = BuildGetReqWithTimeout();
            cmd.OnSuccess(rsp);

            GetResponse response = cmd.Response;

            var rcols = response.Columns.ToArray();
            CollectionAssert.AreEqual(Columns, rcols);

            var rr = response.Value.ToArray();
            for (int i = 0; i < rr.Length; i++)
            {
                TsRow tsr = TsRows[i];
                TsCell[] tscs = tsr.cells.ToArray();

                Row r = rr[i];
                Cell[] rcs = r.Cells.ToArray();

                Assert.AreEqual(tsr.cells.Count, rcs.Length);

                for (int j = 0; j < tscs.Length; j++)
                {
                    TsCell tsc = tscs[j];
                    Cell c = rcs[j];

                    if (tsc.boolean_valueSpecified)
                    {
                        Assert.AreEqual(tsc.boolean_value, c.Value);
                    }
                    else if (tsc.double_valueSpecified)
                    {
                        Assert.AreEqual(tsc.double_value, c.Value);
                    }
                    else if (tsc.sint64_valueSpecified)
                    {
                        Assert.AreEqual(tsc.sint64_value, c.Value);
                    }
                    else if (tsc.timestamp_valueSpecified)
                    {
                        DateTime dt = (DateTime)c.Value;
                        Assert.AreEqual(
                            tsc.timestamp_value,
                            DateTimeUtil.ToUnixTimeMillis(dt));
                    }
                    else if (tsc.varchar_valueSpecified)
                    {
                        byte[] tsc_val = tsc.varchar_value;
                        byte[] cell_val = RiakString.ToBytes((string)c.Value);
                        CollectionAssert.AreEqual(tsc_val, cell_val);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
        }
    }
}
