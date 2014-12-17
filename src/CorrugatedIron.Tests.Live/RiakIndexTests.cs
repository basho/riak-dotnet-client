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
using CorrugatedIron.Tests.Live.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenUsingIndexes : RiakMapReduceTests
    {
        private const string BucketType = "leveldb_type";
        private const string LegacyBucket = "riak_index_tests";

        public WhenUsingIndexes()
        {
            Bucket = LegacyBucket;
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
            Client.DeleteBucket(BucketType, Bucket);
            Client.DeleteBucket(LegacyBucket);
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

            Client.DeleteBucket(LegacyBucket);
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

            var idxid = new RiakIndexId(BucketType, Bucket, "age");
            var result = Client.GetSecondaryIndex(idxid, 32);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(keyCount);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = int.Parse(v.Key);
                key.ShouldBeLessThan(keyCount);
                key.ShouldBeGreaterThan(-1);
            }

            Client.DeleteBucket(BucketType, Bucket);
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

            var result = Client.GetSecondaryIndex(new RiakIndexId(Bucket, "age"), "32");
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
                .Inputs(RiakIndex.Match(LegacyBucket, "age", 32));

            var result = Client.MapReduce(mr);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();

            keys.Count().ShouldEqual(keyCount);

            foreach (var key in keys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
            }

            Client.DeleteBucket(LegacyBucket);
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

            var result = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket, "age"), 27, 30);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(4);
            Client.DeleteBucket(LegacyBucket);
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
                .Inputs(RiakIndex.Keys(LegacyBucket, idxStart.ToString(), idxEnd.ToString()))
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
            Client.DeleteBucket(LegacyBucket);
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

            var result = Client.ListKeysFromIndex(LegacyBucket);

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var keys = result.Value;
            keys.Count.ShouldEqual(keyCount);

            foreach (var key in keys)
            {
                key.ShouldNotBeNullOrEmpty();
                originalKeys.Contains(key).ShouldBeTrue();
            }
            Client.DeleteBucket(LegacyBucket);
        }

        [Test]
        public void GettingKeysWithReturnTermsReturnsThem()
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

            var results = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket,
                "sandwiches"), 2, 5, new RiakIndexGetOptions().SetReturnTerms(true).SetMaxResults(100).SetStream(false));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(4);

            foreach (var indexResult in results.Value.IndexKeyTerms)
            {
                keysAndTerms.Keys.ShouldContain(indexResult.Key);
                keysAndTerms[indexResult.Key].ShouldEqual(int.Parse(indexResult.Term));
            }

            Client.DeleteBucket(LegacyBucket);
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

            var results = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket,
                "position"), 10, 500, new RiakIndexGetOptions().SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(10);

            results.Done.ShouldNotEqual(true);
            results.Done.ShouldEqual(null);
            results.Continuation.ShouldNotBeNull();

            Client.DeleteBucket(LegacyBucket);
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

            var results = Client.StreamGetSecondaryIndex(new RiakIndexId(Bucket, "position"), 0);

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(5);

            Client.DeleteBucket(Bucket);
        }

        private RiakObject GetRiakObjectInLegacyBucket()
        {
            var id = new RiakObjectId(LegacyBucket, Guid.NewGuid().ToString());
            return GetRiakObject(id, "{ value: \"this is an object\" }");
        }

        private RiakObject GetRiakObjectInLegacyBucket(int key)
        {
            var id = new RiakObjectId(LegacyBucket, key.ToString());
            return GetRiakObject(id, "{ value: \"this is an object\" }");
        }

        [Test]
        public void TimeoutOptionWorks()
        {
            var bucket = Bucket;

            for (var i = 0; i < 1000; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("position").Set(i);

                Client.Put(o);
            }

            var results = Client.GetSecondaryIndex(new RiakIndexId(bucket, "position"), 1, 500, new RiakIndexGetOptions().SetTimeout(1));

            results.IsSuccess.ShouldBeFalse();
            results.ErrorMessage.Contains("timeout").ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldBeNull();

            results.Done.ShouldNotEqual(true);
            results.Done.ShouldEqual(null);
            results.Continuation.ShouldBeNull();
        }

        [Test]
        public void UsingTermRegexOnARangeFiltersTheResults()
        {
            var bucket = Bucket;

            for (var i = 0; i < 750; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.BinIndex("lessthan500").Set(i < 500 ? "less" : "more");

                Client.Put(o);
            }

            var results = Client.GetSecondaryIndex(new RiakIndexId(bucket, "lessthan500"), "a", "z", new RiakIndexGetOptions().SetTermRegex("^less"));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldNotBeNull();
            var keyTerms = results.Value.IndexKeyTerms.ToList();
            keyTerms.Count.ShouldEqual(500);
        }

        [Test]
        public void UsingPaginationSortWillSortResultsWhilePaging()
        {
            var bucket = Bucket;

            for (var i = 0; i < 1000; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("positionSorting").Set(i);

                Client.Put(o);
            }

            var results = Client.GetSecondaryIndex(new RiakIndexId(bucket, "positionSorting"), 1, 500, new RiakIndexGetOptions().SetPaginationSort(true).SetReturnTerms(true).SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldNotBeNull();

            var keyTerms = results.Value.IndexKeyTerms.ToList();
            keyTerms[0].Term.ShouldEqual("1");

            var results2 = Client.GetSecondaryIndex(new RiakIndexId(bucket, "positionSorting"), 1, 500,
                new RiakIndexGetOptions().SetPaginationSort(true)
                    .SetReturnTerms(true)
                    .SetMaxResults(10)
                    .SetContinuation(results.Continuation));

            var keyTerms2 = results2.Value.IndexKeyTerms.ToList();
            keyTerms2[0].Term.ShouldEqual("11");

        }

        // TODO: Test Streaming 2i Queries
        private RiakObject GetRiakObject(int key, string value)
        {
            var id = new RiakObjectId(BucketType, Bucket, key.ToString());
            return GetRiakObject(id, value);
        }

        private RiakObject GetRiakObject(RiakObjectId objectId, string value)
        {
            return new RiakObject(objectId, value);
        }
    }
}
