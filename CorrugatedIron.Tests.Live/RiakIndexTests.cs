// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
// 
// This file is provided to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file
// except in compliance with the License.  You may obtain
// a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;

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
            o.AddBinIndex("tacos", "are great!");
            o.AddIntIndex("age", 12);
            
            Client.Put(o);
            
            var result = Client.Get(o.ToRiakObjectId());
            
            result.IsSuccess.ShouldBeTrue();
            var ro = result.Value;
            
            ro.BinIndexes.Count.ShouldEqual(1);
            ro.IntIndexes.Count.ShouldEqual(1);
        }
        
        [Test]
        public void QueryingByIntIndexReturnsAListOfKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIntIndex("age_int", 32);
                
                Client.Put(o);
            }
            
            var input = new RiakIntIndexEqualityInput(Bucket, "age_int", 32);
            
            var mr = new RiakMapReduceQuery();
            mr.Inputs(input)
                .ReduceJs(r => r.Name("mapValuesJson").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            
            var keys = result.Value.PhaseResults.OrderBy(pr => pr.Phase).ElementAt(0).GetObjects<RiakObjectId>();
            
            keys.Count().ShouldEqual(10);
            
            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
            }
        }
        
        [Test]
        public void RiakObjectIdCanBeCreatedFromJsonArrayOrObject()
        {
            for (int i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIntIndex("age_int", 32);
                
                Client.Put(o);
            }
            
            var input = new RiakIntIndexEqualityInput(Bucket, "age_int", 32);
            
            var mr = new RiakMapReduceQuery()
                .Inputs(input)
                .ReduceJs(r => r.Name("mapValuesJson").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            var keysOne = result.Value.PhaseResults.OrderBy(pr => pr.Phase).ElementAt(0).GetObjects<RiakObjectId>();
            
            mr = new RiakMapReduceQuery()
                .Inputs(input)
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            
            result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            var keysTwo = result.Value.PhaseResults.OrderBy(pr => pr.Phase).ElementAt(0).GetObjects<RiakObjectId>();
            
            keysOne.Count().ShouldEqual(keysTwo.Count());
            
            foreach (var key in keysOne)
            {
                key.Key.ShouldNotBeNullOrEmpty();
                key.Bucket.ShouldNotBeNullOrEmpty();
                
                keysTwo.Contains(key).ShouldBeTrue();
            }
            
            foreach (var key in keysTwo)
            {
                key.Key.ShouldNotBeNullOrEmpty();
                key.Bucket.ShouldNotBeNullOrEmpty();
                
                keysOne.Contains(key).ShouldBeTrue();
            }
        }
        
        [Test]
        public void IntRangeQueriesReturnMultipleKeys()
        {
            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(Bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIntIndex("age_int", 25 + i);
                
                Client.Put(o);
            }
            
            var input = new RiakIntIndexRangeInput(Bucket, "age_int", 27, 30);
            
            var mr = new RiakMapReduceQuery()
                .Inputs(input)
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            
            // TODO write tests verifying results
        }
    }
}

