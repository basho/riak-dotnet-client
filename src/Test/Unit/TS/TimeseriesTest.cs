namespace Test.Unit.TS
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

    [TestFixture, UnitTest]
    public abstract class TimeseriesTest
    {
        protected static readonly DateTime Now = DateTime.Now;
        protected static readonly DateTime NowPlusFive = Now.AddSeconds(5);
        protected static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

        protected static readonly RiakString Table = "GeoCheckin";

        protected static readonly bool Boolean0 = false;
        protected static readonly bool Boolean1 = true;

        protected static readonly double Double0 = 12.34F;
        protected static readonly double Double1 = 56.78F;

        protected static readonly long Long0 = 32;
        protected static readonly long Long1 = 54;

        protected static readonly DateTime Timestamp0 = Now;
        protected static readonly DateTime Timestamp1 = NowPlusFive;

        protected static readonly string Varchar0 = "foobar";
        protected static readonly string Varchar1 = "bazbat";

        protected static readonly Cell[] Cells0 = new Cell[]
        {
            new Cell(Boolean0),
            new Cell(Double0),
            new Cell(Long0),
            new Cell(Timestamp0),
            new Cell(Varchar0),
        };

        protected static readonly Cell[] Cells1 = new Cell[]
        {
            new Cell(Boolean1),
            new Cell(Double1),
            new Cell(Long1),
            new Cell(Timestamp1),
            new Cell(Varchar1),
        };

        protected static readonly Row[] Rows = new Row[]
        {
            new Row(Cells0),
            new Row(Cells1)
        };

        protected static readonly Row Key = new Row(Cells0);

        protected static readonly Column[] Columns = new[]
        {
            new Column("bool", global::RiakClient.Commands.TS.ColumnType.Boolean),
            new Column("double", global::RiakClient.Commands.TS.ColumnType.Double),
            new Column("long", global::RiakClient.Commands.TS.ColumnType.SInt64),
            new Column("timestamp", global::RiakClient.Commands.TS.ColumnType.Timestamp),
            new Column("string", global::RiakClient.Commands.TS.ColumnType.Varchar),
        };

        protected static readonly TsColumnDescription[] TsCols = new[]
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

        protected static readonly TsCell[] TsCells0 = new[]
        {
            new TsCell { boolean_value = Boolean0 },
            new TsCell { double_value = Double0 },
            new TsCell { sint64_value = Long0 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp0) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar0) }
        };

        protected static readonly TsCell[] TsCells1 = new[]
        {
            new TsCell { boolean_value = Boolean1 },
            new TsCell { double_value = Double1 },
            new TsCell { sint64_value = Long1 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp1) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar1) }
        };

        protected static readonly TsRow[] TsRows = new[] { new TsRow(), new TsRow() };

        static TimeseriesTest()
        {
            TsRows[0].cells.AddRange(TsCells0);
            TsRows[1].cells.AddRange(TsCells1);
        }

        protected static Get BuildGetReqWithTimeout()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .WithTimeout(Timeout)
                .Build();
            return cmd;
        }

        protected static Store BuildStoreReq()
        {
            var cmd = new Store.Builder()
                .WithTable(Table)
                .WithColumns(Columns)
                .WithRows(Rows)
                .Build();
            return cmd;
        }
    }
}
