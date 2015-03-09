// <copyright file="DevUsingBasicsExamples.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientExamples
{
    using NUnit;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;

    public class Dev_Using_Basics_Tests
    {
        public void Example1()
        {
            using (IRiakEndPoint endpoint = RiakCluster.FromConfig(@"riakConfig", @"riak-test-app.config"))
            {
                IRiakClient client = endpoint.CreateClient();

                // ex1
                var ex1_id = new RiakObjectId("animals", "dogs", "rufus");
                Assert.AreEqual("animals", ex1_id.BucketType);
                Assert.AreEqual("dogs", ex1_id.Bucket);
                Assert.AreEqual("rufus", ex1_id.Key);
                
                var ex1_rslt = client.Get(ex1_id);
                Assert.IsFalse(ex1_rslt.IsSuccess);
                Assert.AreEqual(ResultCode.NotFound, ex1_rslt.ResultCode);
                
                // ex2
                var ex2_id = new RiakObjectId("animals", "dogs", "rufus");
                var ex2_obj = new RiakObject(ex2_id, "WOOF!", RiakConstants.ContentTypes.TextPlain);
                var ex2_rslt = client.Put(ex2_obj);
                
                // IntIndex intIdx = riakObject.IntIndex("test-int-idx");
                // intIdx.Add(9000);
                // var id = new RiakObjectId("users", null);
                ///var obj = new RiakObject(id, @"{'user':'data'}", "application/json");
                /// var  result = client.Put(obj);
                // result.Value.Dump();
                // Console.WriteLine(result.Value.Key);
                
                /*
                var id = new RiakObjectId("orders", "4");
                var result = client.Get(id);
                var vclock = result.Value.VectorClock;
                Console.WriteLine(Convert.ToBase64String(vclock));
                */
                
                /*
                var riakIndexId = new RiakIndexId("orders", "test-int-idx");
                var indexRiakResult = client.StreamGetSecondaryIndex(riakIndexId, 9000);
                var indexResult = indexRiakResult.Value;
                foreach (var key in indexResult.IndexKeyTerms)
                {
                    key.Dump();
                }
                */
                
                /*
                var id = new RiakObjectId("sets", "travel", "cities");
                var citiesSet = client.DtFetchSet(id);
                var adds = new List<string> { "Toronto", "Montreal" };
                var result = client.DtUpdateSet(id, obj => Encoding.UTF8.GetBytes(obj), citiesSet.Context, adds);
                result.Dump();
                
                var fetchResult = client.DtFetchSet(id);
                foreach (var value in fetchResult.Values)
                {
                    Encoding.UTF8.GetString(value).Dump();
                }
                */
            }
        }
    }
}
