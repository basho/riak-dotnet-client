namespace Test.Unit.TS
{
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Util;

    [TestFixture, UnitTest]
    public class TsCellTests : TimeseriesTest
    {
        [Test]
        public void Are_Equatable()
        {
            TsCell c0 = new TsCell { boolean_value = Boolean0 };
            TsCell c1 = new TsCell { boolean_value = Boolean0 };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));

            c0 = new TsCell { double_value = Double0 };
            c1 = new TsCell { double_value = Double0 };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));

            c0 = new TsCell { sint64_value = Long0 };
            c1 = new TsCell { sint64_value = Long0 };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));

            var ut = DateTimeUtil.ToUnixTimeMillis(Timestamp0);
            c0 = new TsCell { timestamp_value = ut };
            c1 = new TsCell { timestamp_value = ut };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));

            c0 = new TsCell { varchar_value = RiakString.ToBytes(Varchar0) };
            c1 = new TsCell { varchar_value = RiakString.ToBytes(Varchar0) };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));

            c0 = new TsCell { varchar_value = Blob0 };
            c1 = new TsCell { varchar_value = Blob0 };
            Assert.AreEqual(c0, c1, string.Format("c0 {0} c1 {1}", c0, c1));
        }
    }
}
