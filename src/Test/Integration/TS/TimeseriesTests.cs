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
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(TwentyMinsAgo),
            new Cell("hurricane"),
            new Cell(82.3)
        };

        private static readonly Cell[] Cells1 = new Cell[]
        {
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(FifteenMinsAgo),
            new Cell("rain"),
            new Cell(79.0)
        };

        private static readonly Cell[] Cells2 = new Cell[]
        {
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(FiveMinsAgo),
            new Cell("wind"),
            Cell.Null
        };

        private static readonly Cell[] Cells3 = new Cell[]
        {
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(Now),
            new Cell("snow"),
            new Cell(20.1)
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
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(FifteenMinsAgo)
        };

        private static readonly Row KeyToDelete = new Row(KeyCells1);

        private static readonly Cell[] KeyCells = new Cell[]
        {
            new Cell("hash1"),
            new Cell("user2"),
            new Cell(FiveMinsAgo)
        };

        private static readonly Row Key = new Row(KeyCells);
        private static readonly Row RowToRestore = new Row(Cells1);

        private static readonly Column[] Columns = new[]
        {
            new Column("geohash",     global::RiakClient.Commands.TS.ColumnType.Varchar),
            new Column("user",        global::RiakClient.Commands.TS.ColumnType.Varchar),
            new Column("time",        global::RiakClient.Commands.TS.ColumnType.Timestamp),
            new Column("weather",     global::RiakClient.Commands.TS.ColumnType.Varchar),
            new Column("temperature", global::RiakClient.Commands.TS.ColumnType.Double)
        };

        /*
         * TODO NB: timeseries does not work with security yet
         */
        public TimeseriesTests()
            : base(useTtb: false, auth: false)
        {
        }

        public TimeseriesTests(bool useTtb, bool auth)
            : base(useTtb, auth)
        {
        }

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

        [Test]
        public void Query_Create_Table()
        {
            string tableName = Guid.NewGuid().ToString();
            string sqlFmt = string.Format(
                @"CREATE TABLE RTS-{0} (geohash varchar not null,
                                        user varchar not null,
                                        time timestamp not null,
                                        weather varchar not null,
                                        temperature double,
                                        data blob,
                  PRIMARY KEY((geohash, user, quantum(time, 15, m)), geohash, user, time))",
                tableName);
            var cmd = new Query.Builder()
                .WithTable(tableName)
                .WithQuery(sqlFmt)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);
            CollectionAssert.IsEmpty(rsp.Columns);
            CollectionAssert.IsEmpty(rsp.Value);
        }

        [Test]
        public void Query_Table_Description()
        {
            var cmd = new Query.Builder()
                .WithTable("GeoCheckin")
                .WithQuery("DESCRIBE GeoCheckin")
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);
            CollectionAssert.IsNotEmpty(rsp.Columns);
            CollectionAssert.IsNotEmpty(rsp.Value);

            int columnCount = rsp.Columns.Count();
            Assert.True(columnCount >= 5 && columnCount <= 8);
            Assert.AreEqual(Columns.Length, rsp.Value.Count());
            foreach (Row row in rsp.Value)
            {
                Assert.AreEqual(columnCount, row.Cells.Count());
            }
        }

        [Test]
        public void Query_Matching_No_Data()
        {
            var qry = "SELECT * from GeoCheckin WHERE time > 0 and time < 10 and geohash = 'hash1' and user = 'user1'";
            var cmd = new Query.Builder()
                .WithTable("GeoCheckin")
                .WithQuery(qry)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);

            CollectionAssert.IsEmpty(rsp.Columns);
            CollectionAssert.IsEmpty(rsp.Value);
        }

        [Test]
        public void Query_Matching_Some_Data()
        {
            var qfmt = "SELECT * FROM GeoCheckin WHERE time > {0} and time < {1} and geohash = 'hash1' and user = 'user2'";
            var q = string.Format(
                qfmt,
                DateTimeUtil.ToUnixTimeMillis(TenMinsAgo),
                DateTimeUtil.ToUnixTimeMillis(Now));

            var cmd = new Query.Builder()
                .WithTable("GeoCheckin")
                .WithQuery(q)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);

            Assert.AreEqual(Columns.Length, rsp.Columns.Count());
            Assert.AreEqual(1, rsp.Value.Count());
        }

        [Test]
        public void Query_Matching_All_Data()
        {
            var qfmt = "SELECT * FROM GeoCheckin WHERE time >= {0} and time <= {1} and geohash = 'hash1' and user = 'user2'";
            var q = string.Format(
                qfmt,
                DateTimeUtil.ToUnixTimeMillis(TwentyMinsAgo),
                DateTimeUtil.ToUnixTimeMillis(Now));

            var cmd = new Query.Builder()
                .WithTable("GeoCheckin")
                .WithQuery(q)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);

            Assert.AreEqual(Columns.Length, rsp.Columns.Count());
            Assert.AreEqual(Rows.Length, rsp.Value.Count());
        }

        [Test]
        public void Query_Streaming_Matching_All_Data()
        {
            var qfmt = "SELECT * FROM GeoCheckin WHERE time >= {0} and time <= {1} and geohash = 'hash1' and user = 'user2'";
            var q = string.Format(
                qfmt,
                DateTimeUtil.ToUnixTimeMillis(TwentyMinsAgo),
                DateTimeUtil.ToUnixTimeMillis(Now));

            ushort i = 0;
            Action<QueryResponse> cb = (QueryResponse qr) =>
            {
                i++;
                Assert.AreEqual(Columns.Length, qr.Columns.Count());
                CollectionAssert.IsNotEmpty(qr.Value);
            };

            var cmd = new Query.Builder()
                .WithTable("GeoCheckin")
                .WithQuery(q)
                .WithCallback(cb)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            QueryResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);
            Assert.Greater(i, 0);
        }

        [Test]
        public void List_Keys_In_Table()
        {
            TestFixtureSetUp();

            int i = 0;
            Action<ListKeysResponse> cb = (ListKeysResponse qr) =>
            {
                i += qr.Value.Count();
            };

            var cmd = new ListKeys.Builder()
                .WithTable("GeoCheckin")
                .WithCallback(cb)
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            ListKeysResponse rsp = cmd.Response;
            Assert.IsFalse(rsp.NotFound);
            Assert.AreEqual(4, i);
        }
    }
}
