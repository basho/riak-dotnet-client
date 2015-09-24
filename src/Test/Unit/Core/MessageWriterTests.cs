namespace Test.Unit
{
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak.Core;
    using RiakClient.Commands;
    using RiakClient.Commands.KV;

    [TestFixture, UnitTest]
    public class MessageWriterTests
    {
        [Test]
        public async Task Can_Write_Ping_Command_To_Stream_And_Get_Expected_Bytes()
        {
            var expected = new byte[] { 0, 0, 0, 1, 1 };
            var buf = new byte[5];
            var cmd = new Ping();

            using (var stream = new MemoryStream(buf, true))
            {
                var writer = new MessageWriter(cmd, stream);
                await writer.WriteAsync();
            }

            CollectionAssert.AreEqual(expected, buf);
        }

        [Test]
        public async Task Can_Write_FetchPreflist_Command_To_Stream_And_Get_Expected_Bytes()
        {
            const string Bucket = "bar";
            const string Key = "baz";
            const string BucketType = "foo";

            var expected = new byte[] { 0, 0, 0, 16, 33, 10, 3, 98, 97, 114, 18, 3, 98, 97, 122, 26, 3, 102, 111, 111 };
            var actual = new byte[expected.Length];

            var o = new FetchPreflistOptions(BucketType, Bucket, Key);
            var cmd = new FetchPreflist(o);

            using (var stream = new MemoryStream(actual, true))
            {
                var writer = new MessageWriter(cmd, stream);
                await writer.WriteAsync();
            }

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}