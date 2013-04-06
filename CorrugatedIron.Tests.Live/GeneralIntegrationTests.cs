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

using System.Collections.Generic;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;

namespace CorrugatedIron.Tests.Live.GeneralIntegrationTests
{
    [TestFixture]
    public class WhenTalkingToRiak : LiveRiakConnectionTestBase
    {
        [Test]
        public void ServerInfoIsSuccessfullyExtracted()
        {
            var result = Client.GetServerInfo();
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
        }

        [Test]
        public void ServerInfoIsSuccessfullyExtractedAsynchronously()
        {
            var result = Client.Async.GetServerInfo().Result;
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
        }

        [Test]
        public void PingRequstResultsInPingResponse()
        {
            var result = Client.Ping();
            result.IsSuccess.ShouldBeTrue(result.ErrorMessage);
        }

        [Test]
        public void ReadingMissingValueDoesntBreak()
        {
            var readResult = Client.Get("nobucket", "novalue");
            readResult.IsSuccess.ShouldBeFalse(readResult.ErrorMessage);
            readResult.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void GetsWithBucketAndKeyReturnObjectsThatAreMarkedAsNotChanged()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);
            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.IsSuccess.ShouldBeTrue();
            readResult.Value.ShouldNotBeNull();
            readResult.Value.HasChanged.ShouldBeFalse();
        }

        [Test]
        public void GetWithInvalidBucketReturnsInvalidRequest()
        {
            var getResult = Client.Get("", "key");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get("foo/bar", "key");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get("  ", "key");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get(null, "key");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithInvalidKeyReturnsInvalidRequest()
        {
            var getResult = Client.Get("bucket", "");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get("bucket", "foo/bar");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get("bucket", "  ");
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
            getResult = Client.Get("bucket", null);
            getResult.ResultCode.ShouldEqual(ResultCode.InvalidRequest);
        }

        [Test]
        public void MultiGetWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var getResults = Client.Get(new List<RiakObjectId>
            {
                new RiakObjectId(null, "key"),
                new RiakObjectId("", "key"),
                new RiakObjectId("  ", "key"),
                new RiakObjectId("foo/bar", "key"),
                new RiakObjectId(TestBucket, TestKey)
            }).ToList();

            getResults.Count(r => r.IsSuccess).ShouldEqual(1);
        }

        [Test]
        public void MultiPutWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var putResults = Client.Put(new List<RiakObject>
            {
                new RiakObject(null, "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("  ", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("foo/bar", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson)
            }).ToList();

            putResults.Count(r => r.IsSuccess).ShouldEqual(1);
        }

        [Test]
        public void MultiDeleteWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var deleteResults = Client.Delete(new List<RiakObjectId>
            {
                new RiakObjectId(null, "key"),
                new RiakObjectId("", "key"),
                new RiakObjectId("  ", "key"),
                new RiakObjectId("foo/bar", "key"),
                new RiakObjectId(TestBucket, TestKey)
            }).ToList();

            deleteResults.Count(r => r.IsSuccess).ShouldEqual(1);
            var deletedItemGetResult = Client.Get(TestBucket, TestKey);
            deletedItemGetResult.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void GetsWithBucketAndKeyReturnObjects()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.IsSuccess.ShouldBeTrue();
            readResult.Value.ShouldNotBeNull();

            var otherDoc = readResult.Value;
            otherDoc.Bucket.ShouldEqual(TestBucket);
            otherDoc.Bucket.ShouldEqual(doc.Bucket);
            otherDoc.Key.ShouldEqual(TestKey);
            otherDoc.Key.ShouldEqual(doc.Key);
        }

        [Test]
        public void GetsWithRiakObjectIdReturnObjects()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var riakObjectId = new RiakObjectId(TestBucket, TestKey);
            var readResult = Client.Get(riakObjectId);

            var otherDoc = readResult.Value;
            otherDoc.Bucket.ShouldEqual(TestBucket);
            otherDoc.Bucket.ShouldEqual(doc.Bucket);
            otherDoc.Key.ShouldEqual(TestKey);
            otherDoc.Key.ShouldEqual(doc.Key);
        }

        [Test]
        public void WritingThenReadingJsonIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);

            var writeResult = Client.Put(doc);
            writeResult.IsSuccess.ShouldBeTrue(writeResult.ErrorMessage);

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.IsSuccess.ShouldBeTrue(readResult.ErrorMessage);

            var loadedDoc = readResult.Value;

            loadedDoc.Bucket.ShouldEqual(doc.Bucket);
            loadedDoc.Key.ShouldEqual(doc.Key);
            loadedDoc.Value.ShouldEqual(doc.Value);
            loadedDoc.VectorClock.ShouldNotBeNull();
        }

        [Test]
        public void BulkInsertFetchDeleteWorksAsExpected()
        {
            var keys = new[] { 1, 2, 3, 4, 5 }.Select(i => TestKey + i).ToList();
            var docs = keys.Select(k => new RiakObject(TestBucket, k, TestJson, RiakConstants.ContentTypes.ApplicationJson)).ToList();

            var writeResult = Client.Put(docs);

            writeResult.All(r => r.IsSuccess).ShouldBeTrue();

            var objectIds = keys.Select(k => new RiakObjectId(TestBucket, k)).ToList();
            var loadedDocs = Client.Get(objectIds).ToList();
            loadedDocs.All(d => d.IsSuccess).ShouldBeTrue();
            loadedDocs.All(d => d.Value != null).ShouldBeTrue();

            var deleteResults = Client.Delete(objectIds);
            deleteResults.All(r => r.IsSuccess).ShouldBeTrue();
        }

        [Test]
        public void BulkInsertFetchDeleteWorksAsExpectedInBatch()
        {
            var keys = new[] { 1, 2, 3, 4, 5 }.Select(i => TestKey + i).ToList();
            var docs = keys.Select(k => new RiakObject(TestBucket, k, TestJson, RiakConstants.ContentTypes.ApplicationJson)).ToList();

            Client.Batch(batch =>
                {
                    var writeResult = batch.Put(docs);

                    writeResult.All(r => r.IsSuccess).ShouldBeTrue();

                    var objectIds = keys.Select(k => new RiakObjectId(TestBucket, k)).ToList();
                    var loadedDocs = batch.Get(objectIds);
                    loadedDocs.All(d => d.IsSuccess).ShouldBeTrue();
                    loadedDocs.All(d => d.Value != null).ShouldBeTrue();

                    var deleteResults = batch.Delete(objectIds);
                    deleteResults.All(r => r.IsSuccess).ShouldBeTrue();
                });
        }

        [Test]
        public void DeletingIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.Get(TestBucket, TestKey);
            result.IsSuccess.ShouldBeTrue();

            Client.Delete(doc.Bucket, doc.Key).IsSuccess.ShouldBeTrue();
            result = Client.Get(TestBucket, TestKey);
            result.IsSuccess.ShouldBeFalse();
            result.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void DeletingIsSuccessfulInBatch()
        {
            Client.Batch(batch =>
                {
                    var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
                    batch.Put(doc).IsSuccess.ShouldBeTrue();

                    // yup, just to be sure the data is there on the next node
                    var result = batch.Get(TestBucket, TestKey);
                    result.IsSuccess.ShouldBeTrue();

                    batch.Delete(doc.Bucket, doc.Key).IsSuccess.ShouldBeTrue();
                    result = batch.Get(TestBucket, TestKey);
                    result.IsSuccess.ShouldBeFalse();
                    result.ResultCode.ShouldEqual(ResultCode.NotFound);
                });
        }

        [Test]
        public void MapReduceQueriesReturnData()
        {
            var bucket = Guid.NewGuid().ToString();

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc).IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(bucket)
                .MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))
                .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(query);
            result.IsSuccess.ShouldBeTrue();

            var mrRes = result.Value;
            mrRes.PhaseResults.ShouldNotBeNull();
            mrRes.PhaseResults.Count().ShouldEqual(2);

            mrRes.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrRes.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            //mrRes.PhaseResults.ElementAt(0).Values.ShouldBeNull();
            foreach(var v in mrRes.PhaseResults.ElementAt(0).Values)
            {
                v.ShouldBeNull();
            }
            mrRes.PhaseResults.ElementAt(1).Values.ShouldNotBeNull();
   
            var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            //var values = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(result.Value.PhaseResults.ElementAt(1).Values.First().FromRiakString());
            values[0].ShouldEqual(10);
        }

        [Test]
        public void MapReduceQueriesReturnDataInBatch()
        {
            var bucket = Guid.NewGuid().ToString();

            Client.Batch(batch =>
                {
                    for (var i = 1; i < 11; i++)
                    {
                        var doc = new RiakObject(bucket, i.ToString(), new { value = i });
                        batch.Put(doc).IsSuccess.ShouldBeTrue();
                    }

                    var query = new RiakMapReduceQuery()
                        .Inputs(bucket)
                        .MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))
                        .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));

                    var result = batch.MapReduce(query);
                    result.IsSuccess.ShouldBeTrue();

                    var mrRes = result.Value;
                    mrRes.PhaseResults.ShouldNotBeNull();
                    mrRes.PhaseResults.Count().ShouldEqual(2);

                    mrRes.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
                    mrRes.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

                    //mrRes.PhaseResults.ElementAt(0).Values.ShouldBeNull();
                    foreach(var v in mrRes.PhaseResults.ElementAt(0).Values)
                    {
                        v.ShouldBeNull();
                    }
                    mrRes.PhaseResults.ElementAt(1).Values.ShouldNotBeNull();
    
                    var values = result.Value.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
                    values[0].ShouldEqual(10);
                });
        }

        [Test]
        public void ListBucketsIncludesTestBucket()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.ListBuckets();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestBucket);
        }

        [Test]
        public void ListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.ListKeys(TestBucket);
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestKey);
        }

        [Test]
        public void StreamListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.StreamListKeys(TestBucket);
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestKey);
        }

        [Test]
        public void WritesWithAllowMultProducesMultiple()
        {
            DoAllowMultProducesMultipleTest(Client);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleInBatch()
        {
            Client.Batch(DoAllowMultProducesMultipleTest);
        }

        private static void DoAllowMultProducesMultipleTest(IRiakBatchClient client)
        {
            // delete first if something does exist
            client.Delete(MultiBucket, MultiKey);

            // Do this via the REST interface - will be substantially slower than PBC
            var props = new RiakBucketProperties().SetAllowMultiple(true).SetLastWriteWins(false);
            props.CanUsePbc.ShouldBeFalse();
            client.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult1 = client.Put(doc);
            writeResult1.IsSuccess.ShouldBeTrue();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult2 = client.Put(doc);
            writeResult2.IsSuccess.ShouldBeTrue();
            writeResult2.Value.Siblings.Count.ShouldBeGreaterThan(2);

            var result = client.Get(MultiBucket, MultiKey);
            result.Value.Siblings.Count.ShouldBeGreaterThan(2);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleVTags()
        {
            // Do this via the PBC - noticable quicker than REST
            var props = new RiakBucketProperties().SetAllowMultiple(true);
            props.CanUsePbc.ShouldBeTrue();
            Client.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.Get(MultiBucket, MultiKey);

            result.Value.VTags.ShouldNotBeNull();
            result.Value.VTags.Count.IsAtLeast(2);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleVTagsInBatch()
        {
            Client.Batch(batch =>
                {
                    // Do this via the PBC - noticable quicker than REST
                    var props = new RiakBucketProperties().SetAllowMultiple(true);
                    props.CanUsePbc.ShouldBeTrue();
                    batch.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

                    var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
                    batch.Put(doc).IsSuccess.ShouldBeTrue();

                    doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
                    batch.Put(doc).IsSuccess.ShouldBeTrue();

                    var result = batch.Get(MultiBucket, MultiKey);

                    result.Value.VTags.ShouldNotBeNull();
                    result.Value.VTags.Count.IsAtLeast(2);
                });
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucketInBatch()
        {
            // add multiple keys
            var bucket = Guid.NewGuid().ToString();

            Client.Batch(batch =>
                {
                    for (var i = 1; i < 11; i++)
                    {
                        var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                        batch.Put(doc);
                    }

                    var keyList = batch.ListKeys(bucket);
                    keyList.Value.Count().ShouldEqual(10);

                    batch.DeleteBucket(bucket);

                    // This might fail if you check straight away
                    // because deleting takes time behind the scenes.
                    // So wait in case (yup, you can shoot me if you like!)
                    Thread.Sleep(4000);

                    keyList = batch.ListKeys(bucket);
                    keyList.Value.Count().ShouldEqual(0);
                    batch.ListBuckets().Value.Contains(bucket).ShouldBeFalse();
                });
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucket()
        {
            // add multiple keys
            var bucket = Guid.NewGuid().ToString();

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc);
            }

            var keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(10);

            Client.DeleteBucket(bucket);

            // This might fail if you check straight away
            // because deleting takes time behind the scenes.
            // So wait in case (yup, you can shoot me if you like!)
            Thread.Sleep(4000);

            keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(0);
            Client.ListBuckets().Value.Contains(bucket).ShouldBeFalse();
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucketAsynchronously()
        {
            // add multiple keys
            var bucket = Guid.NewGuid().ToString();

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc);
            }

            var keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(10);

            var result = Client.Async.DeleteBucket(bucket).Result.ToList();
            result.ForEach(x => x.IsSuccess.ShouldBeTrue(x.ErrorMessage));
            
            // This might fail if you check straight away
            // because deleting takes time behind the scenes.
            // So wait in case (yup, you can shoot me if you like!)
            Thread.Sleep(4000);

            keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(0);
            Client.ListBuckets().Value.Contains(bucket).ShouldBeFalse();
        }

        [Test]
        public void DeletingAnObjectDeletesAnObject()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var deleteResult = Client.Delete(doc.Bucket, doc.Key);
            deleteResult.IsSuccess.ShouldBeTrue();

            var getResult = Client.Get(doc.Bucket, doc.Key);
            getResult.IsSuccess.ShouldBeFalse();
            getResult.Value.ShouldBeNull();
            getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void DeletingAnObjectDeletesAnObjectInBatch()
        {
            Client.Batch(batch =>
                {
                    var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
                    batch.Put(doc).IsSuccess.ShouldBeTrue();

                    var deleteResult = batch.Delete(doc.Bucket, doc.Key);
                    deleteResult.IsSuccess.ShouldBeTrue();

                    var getResult = batch.Get(doc.Bucket, doc.Key);
                    getResult.IsSuccess.ShouldBeFalse();
                    getResult.Value.ShouldBeNull();
                    getResult.ResultCode.ShouldEqual(ResultCode.NotFound);
                });
        }

        [Test]
        public void AsyncListKeysReturnsTheCorrectNumberOfResults()
        {
            var bucket = Guid.NewGuid().ToString();
            
            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                var r = Client.Put(doc);
                r.IsSuccess.ShouldBeTrue();
            }

            var result = Client.Async.ListKeys(bucket).Result;

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.Count().ShouldEqual(10);
        }
    }
}
