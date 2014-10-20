using System;
using System.Linq;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture, Ignore]
    public class RiakCleanupTest : LiveRiakConnectionTestBase
    {
        public RiakCleanupTest()
        {

        }

        [Test, Ignore]
        public void RemoveTestRiakBukets()
        {
            Client.DeleteBucket(TestBucket);
            Client.DeleteBucket(MapReduceBucket);
            Client.DeleteBucket(MultiBucket);
            Client.DeleteBucket(MultiKey);
            Client.DeleteBucket(PropertiesTestBucket);
            Client.DeleteBucket("riak_index_tests");
            Client.DeleteBucket("map_reduce_bucket");

            Client.DeleteBucket("fluent_key_bucket");
            Client.DeleteBucket("riak_search_bucket");
            Client.DeleteBucket("test_multi_bucket");

            Guid tempGuid;
            var buckets = Client.ListBuckets()
                .Where(x => x.StartsWith("test_bucket_")
                    || x.StartsWith("riak_index_tests_")
                    || x.StartsWith("_rsid_")
                    || Guid.TryParse(x, out tempGuid));

            foreach (var bucket in buckets)
            {
                Client.DeleteBucket(bucket);
            }
        }
    }
}
