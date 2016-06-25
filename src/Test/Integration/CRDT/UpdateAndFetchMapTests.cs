namespace Test.Integration.CRDT
{
    using System;
    using Common.Logging;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands;
    using RiakClient.Commands.CRDT;
    using RiakClient.Util;

    public class UpdateAndFetchMapTests : TestBase
    {
        private static readonly ILog Log = Logging.GetLogger(typeof(UpdateAndFetchMapTests));

        protected override RiakString BucketType
        {
            get { return new RiakString("maps"); }
        }

        protected override RiakString Bucket
        {
            get { return new RiakString("map_tests"); }
        }

        [Test]
        public void Fetching_A_Map_Produces_Expected_Values()
        {
            string key = Guid.NewGuid().ToString();
            SaveMap(key);

            IRCommand cmd = new FetchMap.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(key)
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var fcmd = (FetchMap)cmd;
            MapResponse response = fcmd.Response;
            Assert.IsNotNull(response);

            Assert.IsNotEmpty(response.Context);
            Assert.IsNotNull(response.Value);
            Assert.AreEqual(1, response.Value.Counters["counter_1"]);
            Assert.AreEqual((RiakString)"value_1", (RiakString)response.Value.Sets["set_1"][0]);
            Assert.AreEqual((RiakString)"register_value_1", (RiakString)response.Value.Registers["register_1"]);
            Assert.AreEqual(true, response.Value.Flags["flag_1"]);

            Map map2 = response.Value.Maps["map_2"];
            Assert.AreEqual(2, map2.Counters["counter_1"]);
            Assert.AreEqual(RiakString.ToBytes("value_1"), map2.Sets["set_1"][0]);
            Assert.AreEqual(RiakString.ToBytes("register_value_1"), map2.Registers["register_1"]);
            Assert.AreEqual(true, map2.Flags["flag_1"]);

            Map map3 = map2.Maps["map_3"];
            Assert.AreEqual(3, map3.Counters["counter_1"]);
        }

        [Test]
        public void Can_Remove_Data_From_A_Map()
        {
            string key = Guid.NewGuid().ToString();
            MapResponse r = SaveMap(key);

            var mapOp = new UpdateMap.MapOperation();
            mapOp.RemoveCounter("counter_1");

            IRCommand cmd = new UpdateMap.Builder(mapOp)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(key)
                .WithContext(r.Context)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var ucmd = (UpdateMap)cmd;
            MapResponse response = ucmd.Response;
            Assert.False(response.Value.Counters.ContainsKey("counter_1"));
        }

        [Test]
        public void Riak_Can_Generate_Key()
        {
            MapResponse r = SaveMap();
            Assert.IsNotNullOrEmpty(r.Key);
            Log.DebugFormat("Riak Generated Key: {0}", r.Key);
        }

        [Test]
        public void Fetching_An_Unknown_Map_Results_In_Not_Found()
        {
            IRCommand cmd = new FetchMap.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Guid.NewGuid().ToString())
                    .Build();

            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var fcmd = (FetchMap)cmd;
            MapResponse response = fcmd.Response;
            Assert.IsTrue(response.NotFound);
        }

        private MapResponse SaveMap(string key = null)
        {
            var mapOp = new UpdateMap.MapOperation();
            mapOp.IncrementCounter("counter_1", 1)
                .AddToSet("set_1", "value_1")
                .SetRegister("register_1", "register_value_1")
                .SetFlag("flag_1", true);

            var map_2 = mapOp.Map("map_2");
            map_2.IncrementCounter("counter_1", 2)
                .AddToSet("set_1", "value_1")
                .SetRegister("register_1", "register_value_1")
                .SetFlag("flag_1", true);

            var map_3 = map_2.Map("map_3");
            map_3.IncrementCounter("counter_1", 3);

            var updateBuilder = new UpdateMap.Builder(mapOp)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000));

            if (!string.IsNullOrEmpty(key))
            {
                updateBuilder.WithKey(key);
            }

            IRCommand cmd = updateBuilder.Build();
            RiakResult rslt = client.Execute(cmd);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            var ucmd = (UpdateMap)cmd;
            MapResponse response = ucmd.Response;
            Keys.Add(response.Key);

            Assert.True(EnumerableUtil.NotNullOrEmpty(response.Context));

            return response;
        }
    }
}
