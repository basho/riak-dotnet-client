namespace Test.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Riak;
    using Riak.Core;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Commands.KV;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class MessageReaderTests
    {
        [Test]
        public async Task Reads_RpbErrorResp_And_Returns_Response()
        {
            const string ErrMsg = "this is an error";

            var resp = new RpbErrorResp();
            resp.errcode = 1234;
            resp.errmsg = Encoding.UTF8.GetBytes(ErrMsg);

            var cmd = new Ping();

            using (var ms = new MemoryStream())
            {
                await MessageWriter.SerializeAndStreamAsync(resp, MessageCode.RpbErrorResp, ms);

                ms.Seek(0, SeekOrigin.Begin);

                var reader = new MessageReader(cmd, ms);
                Result rslt = await reader.ReadAsync();
                Assert.False(rslt.Success);
                Assert.AreEqual(ErrMsg, rslt.Error.Message);
                Assert.AreSame(rslt.Error, cmd.Error);
            }
        }

        [Test]
        public async Task Throws_Exception_When_Message_Code_From_Riak_Does_Not_Match_Expected()
        {
            var cmd = new FetchServerInfo();

            using (var ms = new MemoryStream())
            {
                await MessageWriter.SerializeAndStreamAsync(null, MessageCode.RpbPingResp, ms);

                ms.Seek(0, SeekOrigin.Begin);

                var reader = new MessageReader(cmd, ms);

                Assert.Throws<ConnectionException>(async () =>
                {
                    Result rslt = await reader.ReadAsync();
                    Assert.Null(rslt);
                });
            }
        }

        [Test]
        public async Task Reads_RpbGetServerInfoResp_And_Returns_Response()
        {
            const string Node = "dev1";
            const string Version = "2.2.0";

            var resp = new RpbGetServerInfoResp();
            resp.node = RiakString.ToBytes(Node);
            resp.server_version = RiakString.ToBytes(Version);

            var cmd = new FetchServerInfo();

            using (var ms = new MemoryStream())
            {
                await MessageWriter.SerializeAndStreamAsync(resp, MessageCode.RpbGetServerInfoResp, ms);

                ms.Seek(0, SeekOrigin.Begin);

                var reader = new MessageReader(cmd, ms);
                Result rslt = await reader.ReadAsync();
                Assert.True(rslt.Success);
                Assert.Null(rslt.Error);

                var info = cmd.Response.Value;
                Assert.AreEqual(Node, (string)info.Node);
                Assert.AreEqual(Version, (string)info.ServerVersion);
            }
        }

        [Test]
        public async Task Reads_Streaming_RpbListBucketsResp_And_Returns_Response()
        {
            const string BucketType = "bucketType";
            const ushort ResponseCount = 16;

            var responses = new RpbListBucketsResp[ResponseCount];
            for (ushort i = 0; i < ResponseCount; i++)
            {
                var resp = new RpbListBucketsResp();

                for (ushort j = 0; j < ResponseCount; j++)
                {
                    resp.buckets.Add(RiakString.ToBytes(string.Format("bucket_{0}_{1}", i, j)));
                }

                if (i == ResponseCount - 1)
                {
                    resp.done = true;
                }

                responses[i] = resp;
            }

            Assert.AreEqual(ResponseCount, responses.Length);

            ushort called = 0;
            var readBuckets = new List<RiakString>();
            Action<IEnumerable<RiakString>> cb = (buckets) =>
                {
                    readBuckets.AddRange(buckets);
                    called++;
                };

            var opts = new ListBucketsOptions(BucketType, true, cb, Timeout.DefaultCommandTimeout);
            var cmd = new ListBuckets(opts);

            using (var ms = new MemoryStream())
            {
                foreach (RpbListBucketsResp resp in responses)
                {
                    await MessageWriter.SerializeAndStreamAsync(resp, MessageCode.RpbListBucketsResp, ms);
                }

                ms.Seek(0, SeekOrigin.Begin);

                var reader = new MessageReader(cmd, ms);
                Result rslt = await reader.ReadAsync();
                Assert.True(rslt.Success);
                Assert.Null(rslt.Error);
                Assert.Null(cmd.Response.Key);
                CollectionAssert.IsEmpty(cmd.Response.Value);
            }

            Assert.AreEqual(ResponseCount, called);
        }
    }
}