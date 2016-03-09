namespace Test.Unit.TS
{
    using System;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Messages;
    using RiakClient.Util;

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
            new Cell<bool>(Boolean0),
            new Cell<double>(Double0),
            new Cell<long>(Long0),
            new Cell<DateTime>(Timestamp0),
            new Cell<string>(Varchar0),
        };

        protected static readonly Cell[] Cells1 = new Cell[]
        {
            new Cell<bool>(Boolean1),
            new Cell<double>(Double1),
            new Cell<long>(Long1),
            new Cell<DateTime>(Timestamp1),
            new Cell<string>(Varchar1),
        };

        protected static readonly Row[] Rows = new Row[]
        {
            new Row(Cells0),
            new Row(Cells1)
        };

        protected static readonly Row Key = new Row(Cells0);

        protected static readonly Column[] Columns = new[]
        {
            new Column("bool", ColumnType.Boolean),
            new Column("double", ColumnType.Double),
            new Column("long", ColumnType.Int64),
            new Column("timestamp", ColumnType.Timestamp),
            new Column("string", ColumnType.Varchar),
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

        protected static Get BuildReqWithTimeout()
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
