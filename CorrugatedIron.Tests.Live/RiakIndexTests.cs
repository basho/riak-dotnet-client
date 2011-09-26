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
using System.Collections.Generic;
using CorrugatedIron.Comms;
using CorrugatedIron.Util;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Extensions;
using NUnit.Framework;
using Newtonsoft.Json;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture()]
    public class WhenUsingIndexes : RiakMapReduceTests
    {
        private const string _ageIndexName = "age";
        
        public WhenUsingIndexes () : base()
        {
            bucket = "riak_index_tests";
        }
        
        [SetUp()]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakConnectionFactory());
            ClientGenerator = () => new RiakClient(Cluster);
            Client = ClientGenerator();
        }
        
        [TearDown()]
        public void TearDown()
        {
//            Client.DeleteBucket(bucket);
        }
        
        [Test()]
        public void IndexesAreSavedWithAnObject()
        {
            var o = new RiakObject(bucket, "the_object", "{ value: \"this is an object\" }");
            o.AddBinIndex("tacos", "are great!");
            o.AddIntIndex("age", 12);
            
            Client.Put(o);
            
            var result = Client.Get(o.ToRiakObjectId());
            
            result.IsSuccess.ShouldBeTrue();
            var ro = result.Value;
            
            ro.BinIndexes.Count.ShouldEqual(1);
            ro.IntIndexes.Count.ShouldEqual(1);
        }
        
        [Test()]
        public void QueryingByIntIndexReturnsAListOfKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIntIndex("age_int", 32);
                
                Client.Put(o);
            }
            
            var input = new RiakIntIndexEqualityInput(bucket, "age_int", 32);
            
            var mr = new RiakMapReduceQuery();
            mr.Inputs(input)
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            
            // TODO write tests verifying results
            var resultString = result.Value.PhaseResults.ElementAt(0).Value.FromRiakString();
            var keys = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(resultString);
            
            keys.Count.ShouldEqual(10);
        }
        
        [Test()]
        public void IntRangeQueriesReturnMultipleKeys()
        {
            for (int i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, i.ToString(), "{ value: \"this is an object\" }");
                o.AddIntIndex("age_int", 25 + i);
                
                Client.Put(o);
            }
            
            var input = new RiakIntIndexRangeInput(bucket, "age_int", 27, 30);
            
            var mr = new RiakMapReduceQuery()
                .Inputs(input)
                .ReduceErlang(r => r.ModFun("riak_kv_mapreduce", "reduce_identity").Keep(true));
            
            
            mr.ContentType = RiakConstants.ContentTypes.ApplicationOctetStream;
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue();
            
            // TODO write tests verifying results
        }
    }
}

