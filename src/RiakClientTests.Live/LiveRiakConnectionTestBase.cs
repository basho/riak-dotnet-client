namespace RiakClientTests.Live
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Config;
    using RiakClient.Extensions;

    public abstract class LiveRiakConnectionTestBase
    {
        private static readonly string testBucket = Guid.NewGuid().ToString();
        private static readonly string testKey = Guid.NewGuid().ToString();

        public static class BucketTypeNames
        {
            public const string Sets = "sets";
            public const string Counters = "counters";
            public const string Maps = "maps";
        }

        protected const string TestBucketType = "plain";
        protected static readonly string TestJson;
        protected const string MapReduceBucket = "map_reduce_bucket";

        // NB: allow_mult/last_write_wins set in devrel setup script
        protected const string MultiBucket = "test_multi_bucket";

        protected const string MultiKey = "test_multi_key";
        protected const string MultiBodyOne = @"{""dishes"": 9}";
        protected const string MultiBodyTwo = @"{""dishes"": 11}";
        protected readonly Random Random = new Random();

        protected IRiakEndPoint Cluster;
        protected IRiakClient Client;
        protected IRiakClusterConfiguration ClusterConfig;

        static LiveRiakConnectionTestBase()
        {
            RiakClient.DisableListKeysWarning = true;
            RiakClient.DisableListBucketsWarning = true;

            TestJson = new
            {
                @string = "value",
                @int = 100,
                @float = 2.34,
                array = new[] { 1, 2, 3 },
                dict = new Dictionary<string, string> {
                    { "foo", "bar" }
                }
            }.ToJson();
        }


        public LiveRiakConnectionTestBase()
        {
            string userName = Environment.GetEnvironmentVariable("USERNAME");
            string configName = "riak1NodeConfiguration";
#if NOAUTH
            configName = userName == "buildbot" ?
                "riak1NodeNoAuthConfiguration" : "riakDevrelNoAuthConfiguration";
#else
            if (MonoUtil.IsRunningOnMono)
            {
                configName = "riak1NodeNoAuthConfiguration";
            }
            else
            {
                configName = userName == "buildbot" ?
                    "riak1NodeConfiguration" : "riakDevrelConfiguration";
            }
#endif
            Cluster = RiakCluster.FromConfig(configName);
        }

        [SetUp]
        public virtual void SetUp()
        {
            Client = Cluster.CreateClient();
        }

        public string TestBucket
        {
            get { return testBucket; }
        }

        public string TestKey
        {
            get { return testKey; }
        }
    }
}
