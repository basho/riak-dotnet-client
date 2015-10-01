namespace Test.Integration.CRDT
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Util;

    public class UpdateAndFetchSetTests : TestBase
    {
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchSetTests));

        private static readonly ISet<byte[]> DefaultAdds = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("add_1"),
                Encoding.UTF8.GetBytes("add_2")
            };

        protected override RiakString BucketType
        {
            get { return new RiakString("sets"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("set_tests"); }
        }

        [Test]
        public void Fetching_A_Set_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveSet(key);

            var fetch = new FetchSet.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNotNull(response.Context);
            Assert.IsNotEmpty(response.Context);
            Assert.AreEqual(DefaultAdds, response.Value);
        }

        [Test]
        public void Can_Update_A_Set()
        {
            string key = Guid.NewGuid().ToString();
            SetResponse resp = SaveSet(key);

            var add_3 = new RiakString("add_3");
            var adds = new HashSet<string> { add_3 };

            var add_1 = new RiakString("add_1");
            var removes = new HashSet<string> { add_1 };

            var update = new UpdateSet.Builder(adds, removes)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithContext(resp.Context)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = update.Response;
            bool found_add_1 = false;
            bool found_add_3 = false;
            foreach (RiakString value in response.Value)
            {
                if (value.Equals(add_1))
                {
                    found_add_1 = true;
                }

                if (value.Equals(add_3))
                {
                    found_add_3 = true;
                }
            }

            Assert.True(found_add_3);
            Assert.False(found_add_1);
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            SetResponse r = SaveSet();
            Assert.IsNotNullOrEmpty(r.Key);
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_Set_Results_In_Not_Found()
        {
            var fetch = new FetchSet.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
            SetResponse response = fetch.Response;
            Assert.IsTrue(response.NotFound);
        }

        private SetResponse SaveSet(string key = null)
        {
            var updateBuilder = new UpdateSet.Builder(DefaultAdds, null)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            UpdateSet cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            SetResponse response = cmd.Response;
            Keys.Add(response.Key);

            Assert.True(EnumerableUtil.NotNullOrEmpty(response.Context));

            return response;
        }
    }
}
