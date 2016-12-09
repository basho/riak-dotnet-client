namespace Test.Integration
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Config;
    using RiakClient.Models;
    using RiakClient.Util;

    [TestFixture, IntegrationTest]
    public abstract class TestBase : IDisposable
    {
        protected static readonly Random R = new Random();

        protected readonly IList<string> Keys = new List<string>();

        protected IRiakEndPoint cluster;
        protected IRiakClient client;
        protected IRiakClusterConfiguration clusterConfig;

        private static Version riakVersion = null;
        private bool disposing = false;

        static TestBase()
        {
            RiakClient.DisableListKeysWarning = true;
            RiakClient.DisableListBucketsWarning = true;
        }

        public TestBase(bool useTtb = false, bool auth = true)
        {
            var config = RiakClusterConfiguration.LoadFromConfig("riakConfiguration");
            var noAuthConfig = RiakClusterConfiguration.LoadFromConfig("riakNoAuthConfiguration");
            if (useTtb)
            {
                config.UseTtbEncoding = true;
                noAuthConfig.UseTtbEncoding = true;
            }
#if NOAUTH
            cluster = new RiakCluster(noAuthConfig);
#else
            if (auth == false || MonoUtil.IsRunningOnMono)
            {
                cluster = new RiakCluster(noAuthConfig);
            }
            else
            {
                cluster = new RiakCluster(config);
            }

#endif
            client = cluster.CreateClient();
        }

        protected virtual RiakString BucketType
        {
            get { return "bucketType_unset"; }
        }

        protected virtual RiakString Bucket
        {
            get { return "bucket_unset"; }
        }

        [OneTimeSetUp]
        public virtual void TestFixtureSetUp()
        {
        }

        [OneTimeTearDown]
        public virtual void TestFixtureTearDown()
        {
            if (EnumerableUtil.NotNullOrEmpty(Keys))
            {
                foreach (string key in Keys)
                {
                    var id = new RiakObjectId(BucketType, Bucket, key);
                    client.Delete(id);
                }
            }
        }

        public void Dispose()
        {
            disposing = true;
            Dispose(disposing);
            GC.SuppressFinalize(disposing);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && cluster != null)
            {
                cluster.Dispose();
            }
        }

        protected void RiakMinVersion(ushort major, ushort minor, ushort build)
        {
            if (client == null)
            {
                throw new InvalidOperationException("Expected a client!");
            }

            if (riakVersion == null)
            {
                var serverInfo = new FetchServerInfo();
                RiakResult rslt = client.Execute(serverInfo);
                Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

                var serverVersion = serverInfo.Response.Value.ServerVersion;
                if (!Version.TryParse(serverVersion, out riakVersion))
                {
                    Assert.Fail("Could not parse server version: {0}", serverVersion);
                }
            }

            if (!(riakVersion.Major >= major && riakVersion.Minor >= minor && riakVersion.Build >= build))
            {
                Assert.Pass("Test requires a newer version of Riak. Skipping!");
            }
        }
    }
}
