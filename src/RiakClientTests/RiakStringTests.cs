namespace RiakClientTests
{
    using System.Text;
    using NUnit.Framework;
    using RiakClient;

    [TestFixture, UnitTest]
    public class RiakStringTests
    {
        const string testString = "test1234";
        private static readonly byte[] testBytes = Encoding.UTF8.GetBytes(testString);

        [Test]
        public void Can_Cast_To_Byte_Array()
        {
            var rs = new RiakString(testString);
            Assert.AreEqual(testBytes, (byte[])rs);
        }

        [Test]
        public void Can_Cast_From_Byte_Array()
        {
            RiakString rs = testBytes;
            Assert.AreEqual(testString, (string)rs);
        }

        [Test]
        public void Can_Construct_From_Byte_Array()
        {
            var rs = new RiakString(testBytes);
            Assert.AreEqual(testBytes, (byte[])rs);
        }

        [Test]
        public void Can_Construct_From_Null()
        {
            var rs = new RiakString((byte[])null);
            Assert.False(rs.HasValue);
            Assert.IsNull((byte[])rs);
            Assert.IsNull(rs.ToString());
        }

        [Test]
        public void Can_Convert_To_Boolean_To_Indicate_Non_Null_Value()
        {
            var rs = new RiakString(testString);
            Assert.True(rs);
            Assert.True(rs.HasValue);

            rs = new RiakString((string)null);
            Assert.False(rs);
            Assert.False(rs.HasValue);

            Assert.Greater(rs.GetHashCode(), 0);

            Assert.DoesNotThrow(() =>
            {
                RiakString.ToBytes(rs);
            });
        }
    }
}
