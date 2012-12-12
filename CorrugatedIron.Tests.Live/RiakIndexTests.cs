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
using System.Linq;

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
            o.AddIndex("tacos", "are great!");
            o.AddIndex("age", 12);
            
            Client.Put(o);
            
            var result = Client.Get(o.ToRiakObjectId());
            
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            var ro = result.Value;
            
            ro.BinIndexes.Count.ShouldEqual(1);
            ro.IntIndexes.Count.ShouldEqual(1);
        }

        [Test]
        public void IntIndexGetReturnsListOfKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIndex("age", 32);
                
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
                o.AddIndex("age", "32");
                
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
                var o = new RiakObject(Bucket, i.ToString(), "{\"value\":\"this is an object\"}");
                o.AddIndex("age", 32);
                
                Client.Put(o);
            }
            
            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Match(Bucket, "age", 32));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjects<RiakObjectId>()).ToList();
            
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
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIndex("age", 25 + i);
                
                Client.Put(o);
            }
            
            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Range(Bucket, "age", 27, 30))
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            
            // TODO write tests verifying results
        }
    }
}

