// <copyright file="RiakIndexTests.cs" company="Basho Technologies, Inc.">
// Copyright 2011 - OJ Reeves & Jeremiah Peschka
// Copyright 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Live
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using MapReduce;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Models;
    using RiakClient.Models.Index;
    using RiakClient.Models.MapReduce;
    using RiakClient.Models.MapReduce.Inputs;

    [TestFixture, IntegrationTest, SkipMono]
    public class WhenUsingIndexes : RiakMapReduceTestBase
    {
        private const string LegacyBucket = "riak_index_tests";
        private const int DefaultKeyCount = 10;

        public WhenUsingIndexes()
        {
            Bucket = LegacyBucket;
        }

        [Test]
        public void IndexesAreSavedWithAnObject()
        {
            var o = CreateGuidKeyedRiakObject("IndexesAreSaved", true);

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
            GenerateIntKeyObjects("IntIndex", (o, i) => o.IntIndex("age").Add(32));

            var idxid = new RiakIndexId(LegacyBucket, "age");
            var result = Client.GetSecondaryIndex(idxid, 32);
            Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
            CollectionAssert.IsNotEmpty(result.Value.IndexKeyTerms);
            Assert.GreaterOrEqual(result.Value.IndexKeyTerms.Count(), DefaultKeyCount);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = ParseIntKeyWithPrefix(v);
                key.ShouldBeLessThan(DefaultKeyCount);
                key.ShouldBeGreaterThan(-1);
            }
        }

        [Test]
        public void BinIndexGetReturnsListOfKeys()
        {
            GenerateIntKeyObjects("BinIndex", (o, i) => o.BinIndex("age").Set("32"));

            var result = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket, "age"), "32");
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(DefaultKeyCount);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = ParseIntKeyWithPrefix(v);
                key.ShouldBeLessThan(DefaultKeyCount);
                key.ShouldBeGreaterThan(-1);
            }
        }

        private static int ParseIntKeyWithPrefix(RiakIndexKeyTerm indexKeyTerm)
        {
            return int.Parse(indexKeyTerm.Key.Split('_')[1]);
        }

        [Test]
        public void QueryingByIntIndexReturnsAListOfKeys()
        {
            GenerateGuidKeyObjects("QueryByIntIndex", (o, i) => o.IntIndex("age").Set(32, 20));

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Match(new RiakIndexId(LegacyBucket, "age"), 32));

            var result = Client.MapReduce(mr);
            Assert.IsTrue(result.IsSuccess, result.ErrorMessage);

            var keys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();
            Assert.GreaterOrEqual(keys.Count(), DefaultKeyCount);

            foreach (var key in keys)
            {
                Assert.IsNotNullOrEmpty(key.Bucket);
                Assert.IsNotNullOrEmpty(key.Key);
            }
        }

        [Test]
        public void IntRangeQueriesReturnMultipleKeys()
        {
            GenerateGuidKeyObjects("IntRangeQueries", (o, i) => o.IntIndex("age").Set(25 + i));

            var result = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket, "age"), 27, 30);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(4);
        }

        [Test]
        public void AllKeysReturnsListOfKeys()
        {
            var insertedKeys = GenerateIntKeyObjects("AllKeys");

            var mr = new RiakMapReduceQuery().Inputs(RiakIndex.AllKeys(Bucket));

            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithTenResultsFound);

            var queriedKeys = result.Value.PhaseResults.SelectMany(x => x.GetObjectIds()).ToList();

            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            var foundKeys = string.Join("\",\r\n\t\"", queriedKeys);
            if (foundKeys.Length > 0)
            {
                foundKeys = "Found keys: \"" + foundKeys + "\"";
            }

            CollectionAssert.IsNotEmpty(queriedKeys);
            Assert.AreEqual(DefaultKeyCount, queriedKeys.Count());

            foreach (var key in queriedKeys)
            {
                key.Bucket.ShouldNotBeNullOrEmpty();
                key.Key.ShouldNotBeNullOrEmpty();
                insertedKeys.Contains(key.Key).ShouldBeTrue();
            }
        }

        [Test]
        public void KeysReturnsSelectiveListOfKeys()
        {
            const int keyStart = 10;
            const int keyCount = 10;
            const int idxStart = 12;
            const int idxEnd = 16;
            const string keyPrefix = "KeysReturnsSelectiveListOfKeys";
            var originalKeys = new List<string>();


            for (var i = keyStart; i < keyStart + keyCount; i++)
            {
                var o = CreateIntKeyedRiakObject(keyPrefix, true, i);
                originalKeys.Add(o.Key);
                Client.Put(o);
            }

            var mr = new RiakMapReduceQuery()
                .Inputs(RiakIndex.Keys(LegacyBucket, keyPrefix + "_" + idxStart, keyPrefix + "_" + idxEnd));

            var result = Client.RunMapReduceQuery(mr).WaitUntil(MapReduceTestHelpers.OnePhaseWithFiveResultsFound);

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
            var generatedKeys = GenerateGuidKeyObjects("ListKeysUsingIndex");
            var originalKeys = new HashSet<string>(generatedKeys);

            var result = Client.ListKeysFromIndex(LegacyBucket);
            Assert.True(result.IsSuccess, result.ErrorMessage);

            var keys = result.Value;
            CollectionAssert.IsNotEmpty(keys);
            foreach (var key in keys)
            {
                Assert.IsNotNullOrEmpty(key);
            }

            foreach (string origKey in originalKeys)
            {
                CollectionAssert.Contains(keys, origKey);
            }
        }

        [Test]
        public void GettingKeysWithReturnTermsReturnsThem()
        {
            const int keyCount = 10;
            var keysAndTerms = new Dictionary<string, int>();


            for (var i = 0; i < keyCount; i++)
            {
                var o = CreateGuidKeyedRiakObject("GettingKeysWithReturnTerms", true);
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
        }

        [Test]
        public void GettingKeysWithContinuationDoesNotSetDone()
        {
            const int keyCount = 1000;

            GenerateGuidKeyObjects("GettingKeysWithContinuation", (o, i) => o.IntIndex("position").Set(i), keyCount);

            var results = Client.GetSecondaryIndex(new RiakIndexId(LegacyBucket,
                "position"), 10, 500, new RiakIndexGetOptions().SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(10);

            results.Done.ShouldNotEqual(true);
            results.Done.ShouldEqual(null);
            results.Continuation.ShouldNotBeNull();
        }

        [Test]
        public void StreamingIndexGetReturnsAllData()
        {
            for (var i = 0; i < DefaultKeyCount; i++)
            {
                var o = CreateIntKeyedRiakObject("StreamingIndex", true, i);
                o.IntIndex("position").Set(i % 2);
                Client.Put(o, new RiakPutOptions().SetW(Quorum.WellKnown.All)
                    .SetDw(Quorum.WellKnown.All));
            }

            var results = Client.StreamGetSecondaryIndex(new RiakIndexId(Bucket, "position"), 0);

            Assert.IsTrue(results.IsSuccess, results.ErrorMessage);
            CollectionAssert.IsNotEmpty(results.Value.IndexKeyTerms);
            // TODO-BROKEN
            // results.Value.IndexKeyTerms.Count().ShouldEqual(5);
        }

        [Test]
        public void TimeoutOptionWorks()
        {
            const int keyCount = 1000;
            GenerateGuidKeyObjects("Timeout", (o, i) => o.IntIndex("position").Set(i), keyCount);

            var results = Client.GetSecondaryIndex(new RiakIndexId(Bucket, "position"), 1, 500,
                new RiakIndexGetOptions().SetTimeout((Timeout)1));

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
            const int keyCount = 750;

            GenerateGuidKeyObjects("TermRegex",
                (o, i) => o.BinIndex("lessthan500").Set(i < 500 ? "less" : "more"), keyCount);

            var results = Client.GetSecondaryIndex(new RiakIndexId(Bucket, "lessthan500"), "a", "z",
                new RiakIndexGetOptions().SetTermRegex("^less"));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldNotBeNull();
            var keyTerms = results.Value.IndexKeyTerms.ToList();
            keyTerms.Count.ShouldEqual(500);
        }

        [Test]
        public void UsingPaginationSortWillSortResultsWhilePaging()
        {
            const int keyCount = 1000;

            GenerateGuidKeyObjects("PaginationSort", (o, i) => o.IntIndex("positionSorting").Set(i), keyCount);

            var results = Client.GetSecondaryIndex(new RiakIndexId(Bucket, "positionSorting"), 1, 500,
                new RiakIndexGetOptions()
                    .SetPaginationSort(true)
                    .SetReturnTerms(true)
                    .SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldNotBeNull();

            var keyTerms = results.Value.IndexKeyTerms.ToList();
            keyTerms[0].Term.ShouldEqual("1");

            var results2 = Client.GetSecondaryIndex(new RiakIndexId(Bucket, "positionSorting"), 1, 500,
                new RiakIndexGetOptions()
                    .SetPaginationSort(true)
                    .SetReturnTerms(true)
                    .SetMaxResults(10)
                    .SetContinuation(results.Continuation));

            var keyTerms2 = results2.Value.IndexKeyTerms.ToList();
            keyTerms2[0].Term.ShouldEqual("11");
        }

        [Test]
        public void AsyncStreamingKeysReturnsListOfKeys()
        {
            const int keyCount = 100;
            var insertedKeys = GenerateGuidKeyObjects("AsyncStreamingMR",
                (o, i) => o.IntIndex("position").Set(i), keyCount);

            var asyncTask = Client.Async.StreamGetSecondaryIndex(new RiakIndexId(Bucket, "position"), 0, keyCount);
            asyncTask.Wait();

            var result = asyncTask.Result;
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);

            result.Value.ShouldNotBeNull(result.ErrorMessage);
            result.Value.IndexKeyTerms.ShouldNotBeNull(result.ErrorMessage);

            var keyTerms = result.Value.IndexKeyTerms.ToList();

            keyTerms.Count.ShouldEqual(insertedKeys.Count);
            var resultKeys = keyTerms.Select(kt => kt.Key).ToList();

            foreach (var insertedKey in insertedKeys)
            {
                resultKeys.ShouldContain(insertedKey,
                    string.Format("Could not find key {0} in index result set", insertedKey));
            }
        }

        private List<string> GenerateIntKeyObjects(
            string keyPrefix, Action<RiakObject, int> indexAction = null, int maxKeys = DefaultKeyCount,
            bool useLegacyBucket = true)
        {
            var insertedKeys = new List<string>();

            for (var idx = 0; idx < maxKeys; idx++)
            {
                var riakObject = CreateIntKeyedRiakObject(keyPrefix, useLegacyBucket, idx);

                if (indexAction != null)
                {
                    indexAction(riakObject, idx);
                }

                insertedKeys.Add(riakObject.Key);
                Client.Put(riakObject);
            }
            return insertedKeys;
        }

        private List<string> GenerateGuidKeyObjects(
            string keyPrefix, Action<RiakObject, int> indexAction = null,
            int maxKeys = DefaultKeyCount,
            bool useLegacyBucket = true)
        {
            var insertedKeys = new List<string>();

            for (var idx = 0; idx < maxKeys; idx++)
            {
                var riakObject = CreateGuidKeyedRiakObject(keyPrefix, useLegacyBucket);

                if (indexAction != null)
                {
                    indexAction(riakObject, idx);
                }

                insertedKeys.Add(riakObject.Key);
                Client.Put(riakObject);
            }
            return insertedKeys;
        }

        private RiakObject CreateGuidKeyedRiakObject(string keyPrefix, bool useLegacyBucket)
        {
            var key = string.Join("_", keyPrefix, Guid.NewGuid());
            return CreateRiakObject(useLegacyBucket, key);
        }

        private RiakObject CreateIntKeyedRiakObject(string keyPrefix, bool useLegacyBucket, int key)
        {
            var stringKey = string.Join("_", keyPrefix, key);
            return CreateRiakObject(useLegacyBucket, stringKey);
        }

        private RiakObject CreateRiakObject(bool useLegacyBucket, string key)
        {
            var id = useLegacyBucket
                ? new RiakObjectId(LegacyBucket, key)
                : new RiakObjectId(TestBucketType, Bucket, key);
            return CreateRiakObject(id, "{ value: \"this is an object\" }");
        }

        private RiakObject CreateRiakObject(RiakObjectId objectId, string value)
        {
            return new RiakObject(objectId, value);
        }
    }
}
