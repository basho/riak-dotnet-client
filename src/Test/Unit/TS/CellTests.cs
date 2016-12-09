namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient.Commands.TS;

    public class CellTests
    {
        [Test]
        [TestCase(null, ColumnType.Null)]
        [TestCase(null, ColumnType.Boolean)] // NB: ct is ignored for null values
        [TestCase(true, ColumnType.Boolean)]
        [TestCase(false, ColumnType.Boolean)]
        [TestCase(double.MinValue, ColumnType.Double)]
        [TestCase(double.MaxValue, ColumnType.Double)]
        [TestCase(long.MinValue, ColumnType.SInt64)]
        [TestCase(long.MaxValue, ColumnType.SInt64)]
        [TestCase(1443796900987, ColumnType.Timestamp)]
        [TestCase("frazzle", ColumnType.Varchar)]
        [TestCase("时间序列", ColumnType.Varchar)]
        [TestCase("frazzle", ColumnType.Blob)]
        [TestCase("时间序列", ColumnType.Blob)]
        [TestCase("временные ряды", ColumnType.Varchar)]
        [TestCase("временные ряды", ColumnType.Blob)]
        public void Equality(object v, ColumnType ct)
        {
            var c0 = new Cell(v, ct);
            var c1 = new Cell(v, ct);
            Assert.AreEqual(c0, c1);
            if (ct == ColumnType.Varchar || ct == ColumnType.Blob)
            {
                Assert.AreEqual(v, c0.ValueAsString);
                Assert.AreEqual(v, c1.ValueAsString);
            }
            else
            {
                Assert.AreEqual(v, c0.Value);
                Assert.AreEqual(v, c1.Value);
            }
        }
    }
}
