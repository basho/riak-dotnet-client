// <copyright file="UpdateMapTests.cs" company="Basho Technologies, Inc.">
// Copyright © 2015 - Basho Technologies, Inc.
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

namespace Test.Integration.CRDT
{
    using System.Text;
    using Common.Logging;
    using RiakClient.Models;

    public class UpdateMapTests : TestBase
    {
        private static readonly ILog log = Logging.GetLogger(typeof(UpdateMapTests));

        protected const string Bucket = "riak_dt_bucket";
        protected readonly DeserializeObject<string> Deserializer = (b, type) => Encoding.UTF8.GetString(b);
        protected readonly SerializeObjectToByteArray<string> Serializer = s => Encoding.UTF8.GetBytes(s);
    }
}

#if COMMENTEDOUT
var Test = require('../testparams');
var UpdateMap = require('../../../lib/commands/crdt/updatemap');
var FetchMap = require('../../../lib/commands/crdt/fetchmap');
var RiakNode = require('../../../lib/core/riaknode');
var RiakCluster = require('../../../lib/core/riakcluster');
var assert = require('assert');

describe('Update and Fetch Map - Integration', function() {
    
    var cluster;
    this.timeout(10000);
    var context;
    
    before(function(done) {
        var nodes = RiakNode.buildNodes(Test.nodeAddresses);
        cluster = new RiakCluster({ nodes: nodes});
        cluster.start();

        var callback = function(err, resp) {
            assert(!err, err);
            assert(resp.context);
            context = resp.context;
            done();
        };

        var mapOp = new UpdateMap.MapOperation();
           
        mapOp.incrementCounter('counter_1', 50)
            .addToSet('set_1', 'value_1')
            .setRegister('register_1', new Buffer('register_value_1'))
            .setFlag('flag_1', true);

        mapOp.map('map_2').incrementCounter('counter_1', 50)
            .addToSet('set_1', 'value_1')
            .setRegister('register_1', new Buffer('register_value_1'))
            .setFlag('flag_1', true)
            .map('map_3');


        var update = new UpdateMap.Builder()
            .withBucketType(Test.mapBucketType)
            .withBucket(Test.bucketName)
            .withKey('map_1')
            .withMapOperation(mapOp)
            .withCallback(callback)
            .withReturnBody(true)
            .withTimeout(20000)
            .build();

        cluster.execute(update);

    });
    
    after(function(done) {
        Test.cleanBucket(cluster, Test.mapBucketType, Test.bucketName, function() {
            cluster.on('stateChange', function(state) { if (state === RiakCluster.State.SHUTDOWN) { done();} });
            cluster.stop();
        });
    });
    
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