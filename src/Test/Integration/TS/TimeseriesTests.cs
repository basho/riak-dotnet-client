namespace Test.Integration.TS
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.TS;
    using RiakClient.Util;

    public class TimeseriesTests : TestBase
    {
        private static readonly RiakString Table = new RiakString("GeoCheckin");

        private static readonly long NowMs = 1443796900987;
        private static readonly DateTime Now = DateTimeUtil.FromUnixTimeMillis(NowMs);
        private static readonly DateTime FiveMinsAgo = Now.AddMinutes(-5);
        private static readonly DateTime TenMinsAgo = FiveMinsAgo.AddMinutes(-5);
        private static readonly DateTime FifteenMinsAgo = TenMinsAgo.AddMinutes(-5);
        private static readonly DateTime TwentyMinsAgo = FifteenMinsAgo.AddMinutes(-5);

        private static readonly Cell[] Cells0 = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(TwentyMinsAgo),
            new Cell<string>("hurricane"),
            new Cell<double>(82.3)
        };

        private static readonly Cell[] Cells1 = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(FifteenMinsAgo),
            new Cell<string>("rain"),
            new Cell<double>(79.0)
        };

        private static readonly Cell[] Cells2 = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(FiveMinsAgo),
            new Cell<string>("wind"),
            Cell.Null
        };

        private static readonly Cell[] Cells3 = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(Now),
            new Cell<string>("snow"),
            new Cell<double>(20.1)
        };

        private static readonly Row[] Rows = new Row[]
        {
            new Row(Cells0),
            new Row(Cells1),
            new Row(Cells2),
            new Row(Cells3)
        };

        private static readonly Cell[] KeyCells1 = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(FifteenMinsAgo)
        };

        private static readonly Row KeyToDelete = new Row(KeyCells1);

        private static readonly Cell[] KeyCells = new Cell[]
        {
            new Cell<string>("hash1"),
            new Cell<string>("user2"),
            new Cell<DateTime>(FiveMinsAgo)
        };

        private static readonly Row Key = new Row(KeyCells);
        private static readonly Row RowToRestore = new Row(Cells1);

        private static readonly Column[] Columns = new[]
        {
            new Column("geohash",     ColumnType.Varchar),
            new Column("user",        ColumnType.Varchar),
            new Column("time",        ColumnType.Timestamp),
            new Column("weather",     ColumnType.Varchar),
            new Column("temperature", ColumnType.Double)
        };

        public override void TestFixtureSetUp()
        {
            var cmd = new Store.Builder()
                    .WithTable(Table)
                    .WithColumns(Columns)
                    .WithRows(Rows)
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
        }

        [Test]
        public void Get_Returns_One_Row()
        {
            var cmd = new Get.Builder()
                .WithTable(Table)
                .WithKey(Key)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            GetResponse rsp = cmd.Response;

            Row[] rows = rsp.Value.ToArray();
            Assert.AreEqual(1, rows.Length);

            Cell[] cells = rows[0].Cells.ToArray();
            CollectionAssert.AreEqual(Cells2, cells);
        }

        [Test]
        public void Delete_One_Row()
        {
            var delete = new Delete.Builder()
                    .WithTable(Table)
                    .WithKey(KeyToDelete)
                    .Build();

            RiakResult rslt = client.Execute(delete);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            Assert.IsFalse(delete.Response.NotFound);

            var get = new Get.Builder()
                .WithTable(Table)
                .WithKey(KeyToDelete)
                .Build();

            rslt = client.Execute(get);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            Assert.IsTrue(get.Response.NotFound);

            var store = new Store.Builder()
                    .WithTable(Table)
                    .WithRow(RowToRestore)
                    .Build();

            rslt = client.Execute(store);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
        }
    }
}
