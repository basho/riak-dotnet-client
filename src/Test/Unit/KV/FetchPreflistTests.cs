namespace Test.Unit.CRDT
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.KV;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class FetchPreflistTests
    {
        private static readonly RiakString BucketType = "myBucketType";
        private static readonly RiakString Bucket = "myBucket";
        private static readonly RiakString Key = "key_1";

        [Test]
        public void Should_Build_Req_Correctly()
        {
            var fetch = new FetchPreflist.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .Build();

            RpbGetBucketKeyPreflistReq protobuf = (RpbGetBucketKeyPreflistReq)fetch.ConstructPbRequest();

            Assert.AreEqual(BucketType, RiakString.FromBytes(protobuf.type));
            Assert.AreEqual(Bucket, RiakString.FromBytes(protobuf.bucket));
            Assert.AreEqual(Key, RiakString.FromBytes(protobuf.key));
        }

        [Test]
        public void Should_Construct_PreflistResponse_From_Resp()
        {
            string node_name = "node-foo";
            long partitionId = long.MaxValue;

            var preflistItem = new RpbBucketKeyPreflistItem();
            preflistItem.node = RiakString.ToBytes(node_name);
            preflistItem.partition = partitionId;
            preflistItem.primary = true;

            var fetchResp = new RpbGetBucketKeyPreflistResp();
            fetchResp.preflist.Add(preflistItem);

            var fetch = new FetchPreflist.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .Build();

            fetch.OnSuccess(fetchResp);

            Assert.AreEqual(1, fetch.Response.Value.Count());
        }
    }
}
