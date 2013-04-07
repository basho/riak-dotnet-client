// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Comms;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenUsingIndexes : RiakMapReduceTests
    {
        public WhenUsingIndexes ()
        {
            Bucket = "riak_index_tests";
        }
        
        [SetUp]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            Client = Cluster.CreateClient();
        }
        
        [TearDown]
        public void TearDown()
        {
            Client.DeleteBucket(Bucket);
        }
        
        [Test]
        public void IndexesAreSavedWithAnObject()
        {
            var o = new RiakObject(Bucket, "the_object", "{ value: \"this is an object\" }");

            o.BinIndex("tacos").Set("are great!");
            o.IntIndex("age").Set(12);
            
            Client.Put(o);
            
            var result = Client.Get(o.ToRiakObjectId());
            
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            var ro = result.Value;
            
            ro.BinIndexes.Count.ShouldEqual(1);
            ro.IntIndexes.Count.ShouldEqual(1);

            ro.BinIndex("tacos").Values[0].ShouldEqual("are great!");
            ro.IntIndex("age").Values[0].ShouldEqual(12);
        }

        [Test]
        public void IntIndexGetReturnsListOfKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("age").Add(32);
                
                Client.Put(o);
            }

            var result = Client.IndexGet(Bucket, "age", 32);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.Count.ShouldEqual(10);

            foreach (var v in result.Value)
            {
                var key = int.Parse(v);
                key.ShouldBeLessThan(10);
                key.ShouldBeGreaterThan(-1);
            }
        }
        
        [Test]
        public void BinIndexGetReturnsListOfKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.BinIndex("age").Set("32");
                
                Client.Put(o);
            }

            var result = Client.IndexGet(Bucket, "age", "32");
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.Count.ShouldEqual(10);

            foreach (var v in result.Value)
            {
                var key = int.Parse(v);
                key.ShouldBeLessThan(10);
                key.ShouldBeGreaterThan(-1);
            }
        }
        
        [Test]
        public void QueryingByIntIndexReturnsAListOfKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, Guid.NewGuid().ToString(), "{\"value\":\"this is an object\"}");
                o.IntIndex("age").Set(32, 20);
                
                Client.Put(o);
            }
            
            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Match(Bucket, "age", 32));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();
            
            keys.Count().ShouldEqual(10);
            
            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
            }
        }
        
        [Test]
        public void IntRangeQueriesReturnMultipleKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("age").Set(25 + i);
                
                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Range(Bucket, "age", 27, 30));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).Count().ShouldEqual(4);
            
            // TODO write tests verifying results
        }

        [Test]
        public void AllKeysReturnsListOfKeys()
        {
            var bucket = Bucket + "_" + Guid.NewGuid().ToString();
            var originalKeys = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                originalKeys.Add(o.Key);

                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.AllKeys(bucket));

            var result = Client.MapReduce(mr);
            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            keys.Count.ShouldEqual(10);

            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key.Key).ShouldBeTrue();
            }
        }

        [Test]
        public void KeysReturnsSelectiveListOfKeys()
        {
            var bucket = Bucket + "_" + Guid.NewGuid().ToString();
            var originalKeys = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");
                originalKeys.Add(o.Key);

                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Keys(bucket, "2", "6"))
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity")
                .Argument("do_prereduce")
                .Keep(true));

            var result = Client.MapReduce(mr);
            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            keys.Count.ShouldEqual(5);

            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key.Key).ShouldBeTrue();
            }
        }

        [Test]
        public void ListKeysUsingIndexReturnsAllKeys()
        {
            var bucket = Bucket + "_" + Guid.NewGuid().ToString();
            var originalKeys = new HashSet<string>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                originalKeys.Add(o.Key);
                
                Client.Put(o);
            }
            
            var result = Client.ListKeysFromIndex(bucket);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value;
            keys.Count.ShouldEqual(10);
            
            foreach (var key in keys)
            {
                key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key).ShouldBeTrue();
            }
        }
    }
}

