namespace Test.Unit.CRDT
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class FetchSetTests
    {
        private static readonly RiakString BucketType = "sets";
        private static readonly RiakString Bucket = "myBucket";
        private static readonly RiakString Key = "set_1";

        [Test]
        public void Should_Build_DtFetchReq_Correctly()
        {
            var fetch = new FetchSet.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithR((Quorum)1)
                .WithPR((Quorum)2)
                .WithNotFoundOK(true)
                .WithBasicQuorum(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            DtFetchReq protobuf = (DtFetchReq)fetch.ConstructPbRequest();

            Assert.AreEqual(BucketType, RiakString.FromBytes(protobuf.type));
            Assert.AreEqual(Bucket, RiakString.FromBytes(protobuf.bucket));
            Assert.AreEqual(Key, RiakString.FromBytes(protobuf.key));
            Assert.AreEqual(1, protobuf.r);
            Assert.AreEqual(2, protobuf.pr);
            Assert.AreEqual(true, protobuf.notfound_ok);
            Assert.AreEqual(true, protobuf.basic_quorum);
            Assert.AreEqual(20000, protobuf.timeout);
        }

        [Test]
        public void Should_Construct_SetResponse_From_DtFetchResp()
        {
            var set_item_1 = new RiakString("set_item_1");
            var set_item_2 = new RiakString("set_item_2");
            var value = new DtValue();
            value.set_value.Add(set_item_1);
            value.set_value.Add(set_item_2);

            var fetchResp = new DtFetchResp();
            fetchResp.value = value;
            fetchResp.type = DtFetchResp.DataType.SET;

            IRCommand cmd = new FetchSet.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .Build();

            cmd.OnSuccess(fetchResp);

            var fcmd = (FetchSet)cmd;
            var itemList = fcmd.Response.Value.Select(v => new RiakString(v)).ToList();
            Assert.AreEqual(set_item_1, itemList[0]);
            Assert.AreEqual(set_item_2, itemList[1]);
        }
    }
}
