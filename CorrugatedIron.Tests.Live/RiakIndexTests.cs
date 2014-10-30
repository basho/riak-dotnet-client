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

using System;
using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Comms;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenUsingIndexes : RiakMapReduceTests
    {
        private readonly string bucketType = "leveldb_type";
        private readonly string legacyBucket = "riak_index_tests";

        public WhenUsingIndexes()
        {
            Bucket = legacyBucket;
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
            Client.DeleteBucket(bucketType, Bucket);
            Client.DeleteBucket(legacyBucket);
        }
        
        [Test]
        public void IndexesAreSavedWithAnObject()
        {
            var o = GetRiakObjectInLegacyBucket();

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

            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void IntIndexGetReturnsListOfKeys()
        {
            int keyCount = 10;

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObject(i, "{ value: \"this is an object\" }");
                o.IntIndex("age").Add(32);
                Client.Put(o);
            }

            var idxid = new RiakIndexId(bucketType, Bucket, "age_int");
            var result = Client.GetSecondaryIndex(idxid, 32);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(keyCount);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = int.Parse(v.Key);
                key.ShouldBeLessThan(keyCount);
                key.ShouldBeGreaterThan(-1);
            }

            Client.DeleteBucket(bucketType, Bucket);
        }
        
        [Test]
        public void BinIndexGetReturnsListOfKeys()
        {
            int keyCount = 10;

            for (var i = 0; i < keyCount; i++)
            {
                var id = new RiakObjectId(Bucket, i.ToString());
                var o = GetRiakObject(id, "{ value: \"this is an object\" }");
                o.BinIndex("age").Set("32");
                Client.Put(o);
            }

            var result = Client.IndexGet(Bucket, "age", "32");
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(keyCount);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = int.Parse(v.Key);
                key.ShouldBeLessThan(keyCount);
                key.ShouldBeGreaterThan(-1);
            }

            Client.DeleteBucket(Bucket);
        }
        
        [Test]
        public void QueryingByIntIndexReturnsAListOfKeys()
        {
            int keyCount = 10;

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket();
                o.IntIndex("age").Set(32, 20);
                Client.Put(o);
            }
            
            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Match(legacyBucket, "age", 32));
            
            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();
            
            keys.Count().ShouldEqual(keyCount);
            
            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
            }

            Client.DeleteBucket(legacyBucket);
        }
        
        [Test]
        public void IntRangeQueriesReturnMultipleKeys()
        {
            int keyCount = 10;

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket();
                o.IntIndex("age").Set(25 + i);
                Client.Put(o);
            }

            var result = Client.IndexGet(legacyBucket, "age", 27, 30);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(4);

            // TODO write tests verifying results
            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void AllKeysReturnsListOfKeys()
        {
            int keyCount = 10;
            var originalKeys = new List<string>();

            for (var i = 0; i < keyCount; i++)
            {
                var id = new RiakObjectId(Bucket, Guid.NewGuid().ToString());
                RiakObject o = GetRiakObject(id, "{ value: \"this is an object\" }");
                originalKeys.Add(o.Key);
                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery().Inputs(RiakIndex.AllKeys(Bucket));

            var result = Client.MapReduce(mr);
            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            keys.Count.ShouldEqual(keyCount);

            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key.Key).ShouldBeTrue();
            }

            Client.DeleteBucket(Bucket);
        }

        [Test]
        public void KeysReturnsSelectiveListOfKeys()
        {
            int keyStart = 10;
            int keyCount = 10;
            int idxStart = 12;
            int idxEnd = 16;
            var originalKeys = new List<string>();

            for (var i = keyStart; i < keyStart + keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket(i);
                originalKeys.Add(o.Key);
                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Keys(legacyBucket, idxStart.ToString(), idxEnd.ToString()))
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

            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void ListKeysUsingIndexReturnsAllKeys()
        {
            int keyCount = 10;
            var originalKeys = new HashSet<string>();

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket();
                originalKeys.Add(o.Key);
                Client.Put(o);
            }
            
            var result = Client.ListKeysFromIndex(legacyBucket);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value;
            keys.Count.ShouldEqual(keyCount);
            
            foreach (var key in keys)
            {
                key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key).ShouldBeTrue();
            }

            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void GettingKeysWithReturnTermsDoesAThing()
        {
            int keyCount = 10;
            var keysAndTerms = new Dictionary<string, int>();

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket();
                o.IntIndex("sandwiches").Set(i);
                keysAndTerms.Add(o.Key, i);
                Client.Put(o);
            }
            
            var results = Client.IndexGet(legacyBucket,
                "sandwiches", 2, 5, new RiakIndexGetOptions().SetReturnTerms(true).SetMaxResults(100).SetStream(false));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(4);

            foreach (var indexResult in results.Value.IndexKeyTerms)
            {
                keysAndTerms.Keys.ShouldContain(indexResult.Key);
                keysAndTerms[indexResult.Key].ShouldEqual(int.Parse(indexResult.Term));
            }

            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void GettingKeysWithContinuationDoesNotSetDone()
        {
            int keyCount = 1000;

            for (var i = 0; i < keyCount; i++)
            {
                var o = GetRiakObjectInLegacyBucket();
                o.IntIndex("position").Set(i);

                Client.Put(o);
            }

            var results = Client.IndexGet(legacyBucket,
                "position", 10, 500, new RiakIndexGetOptions().SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(10);

            results.Done.ShouldNotEqual(true);
            results.Done.ShouldEqual(null);
            results.Continuation.ShouldNotBeNull();

            Client.DeleteBucket(legacyBucket);
        }

        [Test]
        public void StreamingIndexGetReturnsAllData()
        {
            int keyCount = 10;

            for (var i = 0; i < keyCount; i++)
            {
                var o = new RiakObject(Bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("position").Set(i % 2);
                Client.Put(o, new RiakPutOptions().SetW(RiakConstants.QuorumOptions.All)
                                                  .SetDw(RiakConstants.QuorumOptions.All));
            }

            var results = Client.StreamIndexGet(Bucket, "position", 0);

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(5);

            Client.DeleteBucket(Bucket);
        }

        private RiakObject GetRiakObjectInLegacyBucket()
        {
            var id = new RiakObjectId(legacyBucket, Guid.NewGuid().ToString());
            return GetRiakObject(id, "{ value: \"this is an object\" }");
        }

        private RiakObject GetRiakObjectInLegacyBucket(int key)
        {
            var id = new RiakObjectId(legacyBucket, key.ToString());
            return GetRiakObject(id, "{ value: \"this is an object\" }");
        }

        private RiakObject GetRiakObject(int key, string value)
        {
            var id = new RiakObjectId(bucketType, Bucket, key.ToString());
            return GetRiakObject(id, value);
        }

        private RiakObject GetRiakObject(RiakObjectId objectId, string value)
        {
            return new RiakObject(objectId, value);
        }
    }
}
