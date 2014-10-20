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
using System.Reactive.Linq;
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
            result.ShouldNotBeNull();
        }

        [Test]
        public void ServerInfoIsSuccessfullyExtractedAsynchronously()
        {
            var result = Client.Async.GetServerInfo().ConfigureAwait(false).GetAwaiter().GetResult();
            result.ShouldNotBeNull();
        }

        [Test]
        public void PingRequestResultsInPingResponse()
        {
            Client.Ping().ShouldNotBeNull();
        }

        [Test]
        public void ReadingMissingValueDoesntBreak()
        {
            var readResult = Client.Get("nobucket", "novalue");
            readResult.ShouldBeNull();
        }

        [Test]
        public void GetsWithBucketAndKeyReturnObjectsThatAreMarkedAsNotChanged()
        {
            var bucketName = "identity_configuration";
            var key = "RiakMembershipProviderTests";

            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);
            writeResult.ShouldNotBeNull();

            var readResult = Client.Get(bucketName, key);
            readResult.ShouldNotBeNull();
            readResult.HasChanged.ShouldBeFalse();
        }

        [Test]
        public void GetWithEmptyStringBucketNameReturnsInvalidRequest()
        {
            var result = Client.Async.Get("", "key").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithBucketNameWithForwardSlashReturnsInvalidRequest()
        {
            var result = Client.Async.Get("foo/bar", "key").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithNullBucketNameReturnsInvalidRequest()
        {
            var result = Client.Async.Get(null, "key").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithBucketNameWithJustSpacesReturnsInvalidRequest()
        {
            var result = Client.Async.Get("  ", "key").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithEmptyStringKeyReturnsInvalidRequest()
        {
            var result = Client.Async.Get("bucket", "").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }


        [Test]
        public void GetWithKeyWithForwardSlashReturnsInvalidRequest()
        {
            var result = Client.Async.Get("bucket", "foo/bar").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithKeyWithJustSpacesReturnsInvalidRequest()
        {
            var result = Client.Async.Get("bucket", "  ").Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void GetWithNullKeyReturnsInvalidRequest()
        {
            var result = Client.Async.Get("bucket", null).Result;

            result.IsLeft.ShouldBeTrue();
            var expectedException = result.Left;
            expectedException.ErrorCode.ShouldEqual((uint)ResultCode.InvalidRequest);
        }

        [Test]
        public void MultiGetWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.ShouldNotBeNull();

            var getResults = Client.Async.Get(new List<RiakObjectId>
            {
                new RiakObjectId(null, "key"),
                new RiakObjectId("", "key"),
                new RiakObjectId("  ", "key"),
                new RiakObjectId("foo/bar", "key"),
                new RiakObjectId(TestBucket, TestKey)
            })
            .ToEnumerable()
            .ToList();

            getResults
                .Count(r => !r.IsLeft)
                .ShouldEqual(1);
        }

        [Test]
        public void MultiPutWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.ShouldNotBeNull();

            var putResults = Client.Async.Put(new List<RiakObject>
            {
                new RiakObject((string)null, "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("  ", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject("foo/bar", "key", TestJson, RiakConstants.ContentTypes.ApplicationJson),
                new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson)
            })
            .ToEnumerable()
            .ToList();

            putResults
                .Count(r => !r.IsLeft)
                .ShouldEqual(1);
        }

        [Test]
        public void MultiDeleteWithValidAndInvalidBucketsBehavesCorrectly()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.ShouldNotBeNull();

            var deleteResults = Client.Async.Delete(new List<RiakObjectId>
            {
                new RiakObjectId(null, "key"),
                new RiakObjectId("", "key"),
                new RiakObjectId("  ", "key"),
                new RiakObjectId("foo/bar", "key"),
                new RiakObjectId(TestBucket, TestKey)
            })
            .ToEnumerable()
            .ToList();

            deleteResults
                .Count(x => !x.IsLeft)
                .ShouldEqual(1);

            var deletedItemGetResult = Client.Get(TestBucket, TestKey);
            deletedItemGetResult.ShouldBeNull();
        }

        [Test]
        public void GetsWithBucketAndKeyReturnObjects()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.ShouldNotBeNull();

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.ShouldNotBeNull();

            var otherDoc = readResult;
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

            writeResult.ShouldNotBeNull();

            var riakObjectId = new RiakObjectId(TestBucket, TestKey);
            var readResult = Client.Get(riakObjectId);

            var otherDoc = readResult;
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
            writeResult.ShouldNotBeNull();

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.ShouldNotBeNull();

            var loadedDoc = readResult;

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

            writeResult.All(r => r != null).ShouldBeTrue();

            var objectIds = keys.Select(k => new RiakObjectId(TestBucket, k)).ToList();
            var loadedDocs = Client.Get(objectIds).ToList();
            loadedDocs.All(d => d != null).ShouldBeTrue();

            var deletedObjectIds = Client.Delete(objectIds);

            deletedObjectIds.Count(x => x != null).ShouldEqual(objectIds.Count);
        }

        [Test]
        public void BulkInsertFetchDeleteWorksAsExpectedInBatch()
        {
            var keys = new[] { 1, 2, 3, 4, 5 }.Select(i => TestKey + i).ToList();
            var docs = keys.Select(k => new RiakObject(TestBucket, k, TestJson, RiakConstants.ContentTypes.ApplicationJson)).ToList();

            Client.Batch(batch =>
            {
                var writeResult = batch.Put(docs);

                writeResult.All(r => r != null).ShouldBeTrue();

                var objectIds = keys.Select(k => new RiakObjectId(TestBucket, k)).ToList();
                var loadedDocs = batch.Get(objectIds);
                loadedDocs.All(d => d != null).ShouldBeTrue();

                var deletedObjectIds = batch.Delete(objectIds);
                deletedObjectIds.Count(x => x != null).ShouldEqual(objectIds.Count);
            });
        }

        [Test]
        public void DeletingIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.Get(TestBucket, TestKey);
            result.ShouldNotBeNull();

            Client.Delete(doc.Bucket, doc.Key).ShouldNotBeNull();

            result = Client.Get(TestBucket, TestKey);
            result.ShouldBeNull();
        }

        [Test]
        public void DeletingIsSuccessfulInBatch()
        {
            Client.Batch(batch =>
            {
                var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
                batch.Put(doc).ShouldNotBeNull();

                // yup, just to be sure the data is there on the next node
                var result = batch.Get(TestBucket, TestKey);
                result.ShouldNotBeNull();

                batch.Delete(doc.Bucket, doc.Key).ShouldNotBeNull();
                result = batch.Get(TestBucket, TestKey);
                result.ShouldBeNull();
            });
        }

        [Test]
        public void MapReduceQueriesReturnData()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc).ShouldNotBeNull();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(bucket)
                .MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))
                .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(query);
            result.ShouldNotBeNull();

            var mrRes = result;
            mrRes.PhaseResults.ShouldNotBeNull();
            mrRes.PhaseResults.Count().ShouldEqual(2);

            mrRes.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
            mrRes.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

            //mrRes.PhaseResults.ElementAt(0).Values.ShouldBeNull();
            foreach (var v in mrRes.PhaseResults.ElementAt(0).Values)
            {
                v.ShouldBeNull();
            }
            mrRes.PhaseResults.ElementAt(1).Values.ShouldNotBeNull();

            var values = result.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
            //var values = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(result.Value.PhaseResults.ElementAt(1).Values.First().FromRiakString());
            values[0].ShouldEqual(10);
        }

        [Test]
        public void MapReduceQueriesReturnDataInBatch()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            Client.Batch(batch =>
            {
                for (var i = 1; i < 11; i++)
                {
                    var doc = new RiakObject(bucket, i.ToString(), new { value = i });
                    batch.Put(doc).ShouldNotBeNull();
                }

                var query = new RiakMapReduceQuery()
                    .Inputs(bucket)
                    .MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))
                    .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));

                var result = batch.MapReduce(query);
                result.ShouldNotBeNull();

                var mrRes = result;
                mrRes.PhaseResults.ShouldNotBeNull();
                mrRes.PhaseResults.Count().ShouldEqual(2);

                mrRes.PhaseResults.ElementAt(0).Phase.ShouldEqual(0u);
                mrRes.PhaseResults.ElementAt(1).Phase.ShouldEqual(1u);

                //mrRes.PhaseResults.ElementAt(0).Values.ShouldBeNull();
                foreach (var v in mrRes.PhaseResults.ElementAt(0).Values)
                {
                    v.ShouldBeNull();
                }
                mrRes.PhaseResults.ElementAt(1).Values.ShouldNotBeNull();

                var values = result.PhaseResults.ElementAt(1).GetObjects<int[]>().First();
                values[0].ShouldEqual(10);
            });
        }

        [Test]
        public void MultipleBatchesDoNotRunOutOfSockets()
        {
            for (var i = 0; i < 10; i++)
            {
                MapReduceQueriesReturnDataInBatch();
            }
        }

        [Test]
        public void ListBucketsIncludesTestBucket()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.ListBuckets();
            result.ShouldContain(TestBucket);
        }

        [Test]
        public void ListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.ListKeys(TestBucket);
            result.ShouldContain(TestKey);
        }

        [Test]
        public void StreamListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.StreamListKeys(TestBucket);
            result.ShouldContain(TestKey);
        }

        [Test]
        public void StreamListBucketsIncludesTestBucket()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.StreamListBuckets();
            result.ShouldContain(TestBucket);
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
            client.Delete(MultiBucket, MultiKey).ShouldNotBeNull();

            // Do this via the REST interface - will be substantially slower than PBC
            var props = new RiakBucketProperties().SetAllowMultiple(true).SetLastWriteWins(false);
            client.SetBucketProperties(MultiBucket, props).ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult1 = client.Put(doc);
            writeResult1.ShouldNotBeNull();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult2 = client.Put(doc);
            writeResult2.ShouldNotBeNull();
            writeResult2.Siblings.Count.ShouldBeGreaterThan(2);

            var result = client.Get(MultiBucket, MultiKey);
            result.ShouldNotBeNull();
            result.Siblings.Count.ShouldBeGreaterThan(2);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleVTags()
        {
            // Do this via the PBC - noticable quicker than REST
            var props = new RiakBucketProperties().SetAllowMultiple(true);
            Client.SetBucketProperties(MultiBucket, props).ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            var result = Client.Get(MultiBucket, MultiKey);

            result.VTags.ShouldNotBeNull();
            result.VTags.Count.IsAtLeast(2);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleVTagsInBatch()
        {
            Client.Batch(batch =>
            {
                // Do this via the PBC - noticable quicker than REST
                var props = new RiakBucketProperties().SetAllowMultiple(true);
                batch.SetBucketProperties(MultiBucket, props).ShouldBeTrue();

                var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
                batch.Put(doc);

                doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
                batch.Put(doc);

                var result = batch.Get(MultiBucket, MultiKey);

                result.VTags.ShouldNotBeNull();
                result.VTags.Count.IsAtLeast(2);
            });
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucketInBatch()
        {
            // add multiple keys
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            Client.Batch(batch =>
            {
                for (var i = 1; i < 11; i++)
                {
                    var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                    batch.Put(doc);
                }

                var keyList = batch.ListKeys(bucket);
                keyList.Count().ShouldEqual(10);

                var deletedObjectIds = batch.DeleteBucket(bucket);
                deletedObjectIds.Count().ShouldEqual(keyList.Count());

                // This might fail if you check straight away
                // because deleting takes time behind the scenes.
                // So wait in case (yup, you can shoot me if you like!)
                Thread.Sleep(4000);

                keyList = batch.ListKeys(bucket);
                keyList.Count().ShouldEqual(0);
                batch.ListBuckets().Contains(bucket).ShouldBeFalse();
            });
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucket()
        {
            // add multiple keys
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc);
            }

            var keyList = Client.ListKeys(bucket).ToList();
            keyList.Count().ShouldEqual(10);

            var deletedObjectIds = Client.DeleteBucket(bucket).ToList();
            deletedObjectIds.Count().ShouldEqual(keyList.Count());

            // This might fail if you check straight away
            // because deleting takes time behind the scenes.
            // So wait in case (yup, you can shoot me if you like!)
            Thread.Sleep(4000);

            keyList = Client.ListKeys(bucket).ToList();
            keyList.Count().ShouldEqual(0);

            Client.ListBuckets().Contains(bucket).ShouldBeFalse();
        }

        [Test]
        public void DeleteBucketDeletesAllKeysInABucketAsynchronously()
        {
            // add multiple keys
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                Client.Put(doc);
            }

            var keyList = Client.ListKeys(bucket).ToList();
            keyList.Count().ShouldEqual(10);

            var deletedObjectIds = Client.Async.DeleteBucket(bucket)
                .ToEnumerable()
                .ToList();
            deletedObjectIds.Count().ShouldEqual(keyList.Count());

            // This might fail if you check straight away
            // because deleting takes time behind the scenes.
            // So wait in case (yup, you can shoot me if you like!)
            Thread.Sleep(4000);

            keyList = Client.ListKeys(bucket).ToList();
            keyList.Count().ShouldEqual(0);
            Client.ListBuckets().Contains(bucket).ShouldBeFalse();
        }

        [Test]
        public void DeletingAnObjectDeletesAnObject()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            Client.Put(doc).ShouldNotBeNull();

            Client.Delete(doc.Bucket, doc.Key).ShouldNotBeNull();

            var getResult = Client.Get(doc.Bucket, doc.Key);
            getResult.ShouldBeNull();
        }

        [Test]
        public void DeletingAnObjectDeletesAnObjectInBatch()
        {
            Client.Batch(batch =>
            {
                var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
                batch.Put(doc).ShouldNotBeNull();

                batch.Delete(doc.Bucket, doc.Key).ShouldNotBeNull();

                var getResult = batch.Get(doc.Bucket, doc.Key);
                getResult.ShouldBeNull();
            });
        }

        [Test]
        public void AsyncListKeysReturnsTheCorrectNumberOfResults()
        {
            var bucket = string.Format("{0}_{1}", TestBucket, Guid.NewGuid());

            for (var i = 1; i < 11; i++)
            {
                var doc = new RiakObject(bucket, i.ToString(), new { value = i });

                var r = Client.Put(doc);
                r.ShouldNotBeNull();
            }

            var result = Client.Async.ListKeys(bucket)
                .ToEnumerable()
                .ToList();

            result.Count.ShouldEqual(10);
        }
    }
}
