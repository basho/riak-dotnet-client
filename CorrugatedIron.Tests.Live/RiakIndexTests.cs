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
using CorrugatedIron.Models.Index;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CorrugatedIron.Tests.Live
{
    [TestFixture]
    public class WhenUsingIndexes : RiakMapReduceTests
    {
        public WhenUsingIndexes()
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
            result.Value.IndexKeyTerms.Count().ShouldEqual(10);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = int.Parse(v.Key);
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
            result.Value.IndexKeyTerms.Count().ShouldEqual(10);

            foreach (var v in result.Value.IndexKeyTerms)
            {
                var key = int.Parse(v.Key);
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

            var result = Client.IndexGet(Bucket, "age", 27, 30);
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
            result.Value.IndexKeyTerms.Count().ShouldEqual(4);
        }

        [Test]
        public void AllKeysReturnsListOfKeys()
        {
            var bucket = Bucket + "_" + Guid.NewGuid();
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
            var bucket = Bucket + "_" + Guid.NewGuid();
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

        [Test]
        public void GettingKeysWithReturnTermsReturnsThem()
        {
            var bucket = Bucket;
            var keysAndTerms = new Dictionary<string, int>();

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("sandwiches").Set(i);
                keysAndTerms.Add(o.Key, i);

                Client.Put(o);
            }

            var results = Client.IndexGet(bucket, "sandwiches", 2, 5, new RiakIndexGetOptions().SetReturnTerms(true).SetMaxResults(100).SetStream(false));

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
            var bucket = Bucket;

            for (var i = 0; i < 1000; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("position").Set(i);

                Client.Put(o);
            }

            var results = Client.IndexGet(bucket, "position", 10, 500, new RiakIndexGetOptions().SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(10);

            results.Done.ShouldNotEqual(true);
            results.Done.ShouldEqual(null);
            results.Continuation.ShouldNotBeNull();
        }

        [Test]
        public void StreamingIndexGetReturnsAllData()
        {
            var bucket = Bucket;

            for (var i = 0; i < 10; i++)
            {
                var o = new RiakObject(bucket, Guid.NewGuid().ToString(), "{ value: \"this is an object\" }");
                o.IntIndex("position").Set(i % 2);

                Client.Put(o, new RiakPutOptions().SetW(RiakConstants.QuorumOptions.All)
                                                  .SetDw(RiakConstants.QuorumOptions.All));
            }

            var results = Client.StreamIndexGet(bucket, "position", 0);

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.IndexKeyTerms.Count().ShouldEqual(5);
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

            var results = Client.IndexGet(bucket, "position", 1, 500, new RiakIndexGetOptions().SetTimeout(1));

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

            var results = Client.IndexGet(bucket, "lessthan500", "a", "z", new RiakIndexGetOptions().SetTermRegex("^less"));

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
                o.IntIndex("position").Set(i);

                Client.Put(o);
            }

            var results = Client.IndexGet(bucket, "position", 1, 500, new RiakIndexGetOptions().SetPaginationSort(true).SetReturnTerms(true).SetMaxResults(10));

            results.IsSuccess.ShouldBeTrue(results.ErrorMessage);
            results.Value.ShouldNotBeNull();

            var keyTerms = results.Value.IndexKeyTerms.ToList();
            keyTerms[0].Term.ShouldEqual("1");

            var results2 = Client.IndexGet(bucket, "position", 1, 500,
                new RiakIndexGetOptions().SetPaginationSort(true)
                    .SetReturnTerms(true)
                    .SetMaxResults(10)
                    .SetContinuation(results.Continuation));

            var keyTerms2 = results2.Value.IndexKeyTerms.ToList();
            keyTerms2[0].Term.ShouldEqual("11");

        }

        // TODO: Test Streaming 2i Queries
    }
}

