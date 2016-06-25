namespace Test.Integration.CRDT
{
    using System;
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Commands.CRDT;

    public class UpdateAndFetchCounterTests : TestBase
    {
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchCounterTests));
        private static readonly long DefaultIncrement = 10;

        protected override RiakString BucketType
        {
            get { return new RiakString("counters"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("counter_tests"); }
        }

        [Test]
        public void Fetching_A_Counter_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveCounter(key);

            IRCommand cmd = new FetchCounter.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var fcmd = (FetchCounter)cmd;
            CounterResponse response = fcmd.Response;
            Assert.IsNotNull(response);

            Assert.AreEqual(DefaultIncrement, response.Value);
        }

        [Test]
        public void Can_Update_A_Counter()
        {
            string key = Guid.NewGuid().ToString();
            SaveCounter(key);

            IRCommand cmd = new UpdateCounter.Builder(DefaultIncrement)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var ucmd = (UpdateCounter)cmd;
            CounterResponse response = ucmd.Response;
            Assert.AreEqual(20, response.Value);
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            CounterResponse r = SaveCounter();
            Assert.IsNotNullOrEmpty(r.Key);
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_Counter_Results_In_Not_Found()
        {
            IRCommand cmd = new FetchCounter.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var fcmd = (FetchCounter)cmd;
            CounterResponse response = fcmd.Response;
            Assert.IsTrue(response.NotFound);
        }

        private CounterResponse SaveCounter(string key = null)
        {
            var updateBuilder = new UpdateCounter.Builder(DefaultIncrement)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            IRCommand cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var ucmd = (UpdateCounter)cmd;
            CounterResponse response = ucmd.Response;
            Keys.Add(response.Key);

            return response;
        }
    }
}
