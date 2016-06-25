namespace Test.Integration.CRDT
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Commands.KV;

    public class FetchPreflistTests : TestBase
    {
        protected override RiakString BucketType
        {
            get { return new RiakString("plain"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("fetch_preflists"); }
        }

        [Test]
        public void Can_Fetch_Preflist()
        {
            RiakMinVersion(2, 1, 0);

            IRCommand cmd = new FetchPreflist.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey("key_1")
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            if (rslt.IsSuccess)
            {
                var fcmd = (FetchPreflist)cmd;
                PreflistResponse response = fcmd.Response;
                Assert.IsNotNull(response);

                Assert.AreEqual(3, response.Value.Count());
            }
            else
            {
                // TODO: remove this else case when this fix is in Riak:
                // https://github.com/basho/riak_kv/pull/1116
                // https://github.com/basho/riak_core/issues/706
                bool foundMessage =
                    (rslt.ErrorMessage.IndexOf("Permission denied") >= 0 && rslt.ErrorMessage.IndexOf("riak_kv.get_preflist") >= 0) ||
                    (rslt.ErrorMessage.IndexOf("error:badarg") >= 0 && rslt.ErrorMessage.IndexOf("characters_to_binary") >= 0);
                Assert.True(foundMessage, rslt.ErrorMessage);
            }
        }
    }
}
