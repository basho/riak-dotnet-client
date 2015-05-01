// <copyright file="UpdateMapTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
//
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

namespace Test.Integration.CRDT
{
    using System;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Models;

    public class UpdateMapTests : TestBase
    {
        private const string BucketType = "maps";
        private const string Bucket = "map_tests";
        private const string Key = "map_1";

        [Test]
        public void Fetching_A_Map_Produces_Expected_Values()
        {
            var fetch = new FetchMap.Builder()
                    .WithBucketType(BucketType)
                    .WithBucket(Bucket)
                    .WithKey(Key)
                    .Build();

            RiakResult rslt = client.Execute(fetch);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);

            MapResponse response = fetch.Response;
            Assert.IsNotNull(response);

            Assert.IsNotEmpty(response.Context);
            Assert.IsNotNull(response.Map);
            Assert.AreEqual(1, response.Map.Counters["counter_1"]);
            Assert.AreEqual((RiakString)"value_1", (RiakString)response.Map.Sets["set_1"][0]);
            Assert.AreEqual((RiakString)"register_value_1", (RiakString)response.Map.Registers["register_1"]);
            Assert.AreEqual(true, response.Map.Flags["flag_1"]);

            Map map2 = response.Map.Maps["map_2"];
            Assert.AreEqual(2, map2.Counters["counter_1"]);
            Assert.AreEqual(RiakString.ToBytes("value_1"), map2.Sets["set_1"][0]);
            Assert.AreEqual(RiakString.ToBytes("register_value_1"), map2.Registers["register_1"]);
            Assert.AreEqual(true, map2.Flags["flag_1"]);

            Map map3 = map2.Maps["map_3"];
            Assert.AreEqual(3, map3.Counters["counter_1"]);
        }

        protected override void TestFixtureSetUp()
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

            var update = new UpdateMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithMapOperation(mapOp)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            RiakResult rslt = client.Execute(update);
            Assert.IsTrue(rslt.IsSuccess, rslt.ErrorMessage);
        }

        protected override void TestFixtureTearDown()
        {
            var id = new RiakObjectId(BucketType, Bucket, Key);
            client.Delete(id);
            /*
            Test.cleanBucket(cluster, Test.mapBucketType, Test.bucketName, function() {
                cluster.on('stateChange', function(state) { if (state === RiakCluster.State.SHUTDOWN) { done();} });
                cluster.stop();
            });
             */
        }
    }
}

#if COMMENTEDOUT
    it('Should fetch a map', function(done) {
       
        var callback = function(err, resp) {
            assert(!err, err);
            assert(resp.context);
            assert(resp.map);
            assert.equal(resp.map.counters.counter_1, 50);
            assert.equal(resp.map.sets.set_1[0], 'value_1');
            assert.equal(resp.map.registers.register_1.toString('utf8'), 'register_value_1');
            assert.equal(resp.map.flags.flag_1, true);
            done();
        };
        
        var fetch = new FetchMap.Builder()
                .withBucketType(Test.mapBucketType)
                .withBucket(Test.bucketName)
                .withKey('map_1')
                .withCallback(callback)
                .build();

        cluster.execute(fetch);

        
    });
    
    it('Should remove stuff from a map', function(done) {
        
        var callback = function(err, resp) {
            assert(!err, err);
            assert(resp.context);
            assert(resp.map);
            assert(!resp.map.counters.counter_1);
            assert.equal(resp.map.sets.set_1[0], 'value_1');
            assert.equal(resp.map.registers.register_1.toString('utf8'), 'register_value_1');
            assert.equal(resp.map.flags.flag_1, true);
            done();
        };
        
        var mapOp = new UpdateMap.MapOperation().removeCounter('counter_1');
        
        var update = new UpdateMap.Builder()
            .withBucketType(Test.mapBucketType)
            .withBucket(Test.bucketName)
            .withKey('map_1')
            .withContext(context)
            .withMapOperation(mapOp)
            .withCallback(callback)
            .withTimeout(20000)
            .build();
    
        cluster.execute(update);
        
        
    });
    
});
#endif
