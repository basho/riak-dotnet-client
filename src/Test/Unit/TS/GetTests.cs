namespace Test.Unit.TS
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

    [TestFixture, UnitTest]
    public class GetTests
    {
        private static readonly DateTime Now = DateTime.Now;
        private static readonly DateTime NowPlusFive = Now.AddSeconds(5);
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

        private static readonly RiakString Table = "GeoCheckin";

        private static readonly bool Boolean0 = false;
        private static readonly bool Boolean1 = true;

        private static readonly double Double0 = 12.34F;
        private static readonly double Double1 = 56.78F;

        private static readonly long Long0 = 32;
        private static readonly long Long1 = 54;

        private static readonly DateTime Timestamp0 = Now;
        private static readonly DateTime Timestamp1 = NowPlusFive;

        private static readonly string Varchar0 = "foobar";
        private static readonly string Varchar1 = "bazbat";

        private static readonly Cell[] Cells0 = new Cell[]
            {
                new Cell<bool>(Boolean0),
                new Cell<double>(Double0),
                new Cell<long>(Long0),
                new Cell<DateTime>(Timestamp0),
                new Cell<string>(Varchar0),
            };

        private static readonly Row Key = new Row(Cells0);

        private static readonly Column[] Columns = new[]
        {
            new Column("bool", ColumnType.Boolean),
            new Column("double", ColumnType.Double),
            new Column("long", ColumnType.Int64),
            new Column("timestamp", ColumnType.Timestamp),
            new Column("string", ColumnType.Varchar),
        };

        private static readonly TsColumnDescription[] TsCols = new[]
        {
            new TsColumnDescription
            {
                name = RiakString.ToBytes("bool"),
                type = TsColumnType.BOOLEAN
            },
            new TsColumnDescription
            {
                name = RiakString.ToBytes("double"),
                type = TsColumnType.DOUBLE
            },
            new TsColumnDescription
            {
                name = RiakString.ToBytes("long"),
                type = TsColumnType.SINT64
            },
            new TsColumnDescription
            {
                name = RiakString.ToBytes("timestamp"),
                type = TsColumnType.TIMESTAMP
            },
            new TsColumnDescription
            {
                name = RiakString.ToBytes("string"),
                type = TsColumnType.VARCHAR
            }
        };

        private static readonly TsCell[] TsCells0 = new[]
        {
            new TsCell { boolean_value = Boolean0 },
            new TsCell { double_value = Double0 },
            new TsCell { sint64_value = Long0 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp0) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar0) }
        };

        private static readonly TsCell[] TsCells1 = new[]
        {
            new TsCell { boolean_value = Boolean1 },
            new TsCell { double_value = Double1 },
            new TsCell { sint64_value = Long1 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp1) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar1) }
        };

        private static readonly TsRow[] TsRows = new[] { new TsRow(), new TsRow() };

        static GetTests()
        {
            TsRows[0].cells.AddRange(TsCells0);
            TsRows[1].cells.AddRange(TsCells1);
        }

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
            Assert.AreEqual(Cells0[0].AsObject, pb.key[0].boolean_value);

            Assert.True(pb.key[1].double_valueSpecified);
            Assert.AreEqual(Cells0[1].AsObject, pb.key[1].double_value);

            Assert.True(pb.key[2].sint64_valueSpecified);
            Assert.AreEqual(Cells0[2].AsObject, pb.key[2].sint64_value);

            var dt = (DateTime)Cells0[3].AsObject;
            Assert.True(pb.key[3].timestamp_valueSpecified);
            Assert.AreEqual(DateTimeUtil.ToUnixTimeMillis(dt), pb.key[3].timestamp_value);

            var s = RiakString.ToBytes((string)Cells0[4].AsObject);
            Assert.True(pb.key[4].varchar_valueSpecified);
            CollectionAssert.AreEqual(s, pb.key[4].varchar_value);
        }

        [Test]
        public void Should_Build_Req_With_Timeout()
        {
            Get cmd = BuildReqWithTimeout();
            Assert.AreEqual(MessageCode.TsGetResp, cmd.ExpectedCode);

            TsGetReq pb = (TsGetReq)cmd.ConstructPbRequest();
            Assert.IsTrue(pb.timeoutSpecified);
            Assert.AreEqual(Timeout.TotalMilliseconds, pb.timeout);
        }

        [Test]
        public void Should_Parse_Resp()
        {
            var rsp = new TsGetResp();
            rsp.columns.AddRange(TsCols);
            rsp.rows.AddRange(TsRows);

            Get cmd = BuildReqWithTimeout();
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
                        Assert.AreEqual(tsc.boolean_value, c.AsObject);
                    }
                    else if (tsc.double_valueSpecified)
                    {
                        Assert.AreEqual(tsc.double_value, c.AsObject);
                    }
                    else if (tsc.sint64_valueSpecified)
                    {
                        Assert.AreEqual(tsc.sint64_value, c.AsObject);
                    }
                    else if (tsc.timestamp_valueSpecified)
                    {
                        var dt = (Cell<DateTime>)c;
                        Assert.AreEqual(tsc.timestamp_value, DateTimeUtil.ToUnixTimeMillis(dt.Value));
                    }
                    else if (tsc.varchar_valueSpecified)
                    {
                        byte[] tsc_val = tsc.varchar_value;

                        var cell_str = (Cell<string>)c;
                        byte[] cell_val = RiakString.ToBytes(cell_str.Value);

                        CollectionAssert.AreEqual(tsc_val, cell_val);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        private static Get BuildReqWithTimeout()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .WithTimeout(Timeout)
                .Build();
            return cmd;
        }
    }
}
