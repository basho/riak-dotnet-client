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
        // 1449000732345
        protected static readonly DateTime Now = new DateTime(2015, 12, 1, 20, 12, 12, 345, DateTimeKind.Utc);

        // 1449000737345
        protected static readonly DateTime NowPlusFive = Now.AddSeconds(5);

        protected static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

        protected static readonly RiakString Table = "GeoCheckin";

        protected static readonly bool Boolean0 = false;
        protected static readonly bool Boolean1 = true;

        protected static readonly double Double0 = 12.34f;
        protected static readonly double Double1 = 56.78f;

        protected static readonly long Long0 = 32;
        protected static readonly long Long1 = 54;

        protected static readonly DateTime Timestamp0 = Now;
        protected static readonly DateTime Timestamp1 = NowPlusFive;

        protected static readonly string Varchar0 = "foobar";
        protected static readonly string Varchar1 = "bazbat";

        protected static readonly byte[] Blob0 = RiakString.ToBytes(Varchar0);
        protected static readonly byte[] Blob1 = RiakString.ToBytes(Varchar1);

        protected static readonly Cell[] Cells0 = new Cell[]
        {
            new Cell(Boolean0),
            new Cell(Double0),
            new Cell(Long0),
            new Cell(Timestamp0),
            new Cell(Varchar0),
            new Cell(Blob0)
        };

        protected static readonly Cell[] Cells1 = new Cell[]
        {
            new Cell(Boolean1),
            new Cell(Double1),
            new Cell(Long1),
            new Cell(Timestamp1),
            new Cell(Varchar1),
            new Cell(Blob1)
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
            new Column("blob", global::RiakClient.Commands.TS.ColumnType.Blob)
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
            },
            new TsColumnDescription
            {
                name = RiakString.ToBytes("blob"),
                type = TsColumnType.BLOB
            }
        };

        protected static readonly TsCell[] TsCells0 = new[]
        {
            new TsCell { boolean_value = Boolean0 },
            new TsCell { double_value = Double0 },
            new TsCell { sint64_value = Long0 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp0) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar0) },
            new TsCell { varchar_value = Blob0 }
        };

        protected static readonly TsCell[] TsCells1 = new[]
        {
            new TsCell { boolean_value = Boolean1 },
            new TsCell { double_value = Double1 },
            new TsCell { sint64_value = Long1 },
            new TsCell { timestamp_value = DateTimeUtil.ToUnixTimeMillis(Timestamp1) },
            new TsCell { varchar_value = RiakString.ToBytes(Varchar1) },
            new TsCell { varchar_value = Blob1 }
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

        /*
         * NB: MUST use 18.3 since R16 will generate floats as strings
        T = {tsputreq,<<"GeoCheckin">>,[],[
            {false,12.34,32,1449000732345,<<"foobar">>},
            {true,56.78,54,1449000737345,<<"bazbat">>}]}.
        rp(term_to_binary(T)).
        <<131,104,4,100,0,8,116,115,112,117,116,114,101,113,109,0,
          0,0,10,71,101,111,67,104,101,99,107,105,110,106,108,0,0,
          0,2,104,5,100,0,5,102,97,108,115,101,70,64,40,174,20,
          122,225,71,174,97,32,110,6,0,185,134,44,95,81,1,109,0,0,
          0,6,102,111,111,98,97,114,104,5,100,0,4,116,114,117,101,
          70,64,76,99,215,10,61,112,164,97,54,110,6,0,65,154,44,
          95,81,1,109,0,0,0,6,98,97,122,98,97,116,106>>
        */
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
