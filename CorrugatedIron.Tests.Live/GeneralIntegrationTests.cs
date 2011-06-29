// Copyright (c) 2010 - OJ Reeves & Jeremiah Peschka
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
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Tests.Live.LiveRiakConnectionTests;
using CorrugatedIron.Util;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.GeneralIntegrationTests
{
    [TestFixture]
    public class WhenTalkingToRiak : LiveRiakConnectionTestBase
    {
        [Test]
        public void ServerInfoIsSuccessfullyExtracted()
        {
            var result = Client.GetServerInfo();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void PingRequstResultsInPingResponse()
        {
            var result = Client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void ReadingMissingValueDoesntBreak()
        {
            var readResult = Client.Get("nobucket", "novalue");
            readResult.IsSuccess.ShouldBeFalse();
            readResult.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void GetsReturnObjects()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult = Client.Put(doc);

            writeResult.IsSuccess.ShouldBeTrue();
            writeResult.Value.ShouldNotBeNull();

            var readResult = Client.Get(TestBucket, TestKey);
            readResult.IsSuccess.ShouldBeTrue();
            readResult.Value.ShouldNotBeNull();

            var otherDoc = readResult.Value;
            otherDoc.Bucket.ShouldEqual(doc.Bucket);
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
            loadedDoc.VectorClock.ShouldNotBeNullOrEmpty();
        }

        [Test]
        public void BulkInsertFetchDeleteWorksAsExpected()
        {
            var keys = new[] { 1, 2, 3, 4, 5 }.Select(i => TestKey + i);
            var docs = keys.Select(k => new RiakObject(TestBucket, k, TestJson, RiakConstants.ContentTypes.ApplicationJson)).ToList();

            var writeResult = Client.Put(docs);

            writeResult.All(r => r.IsSuccess).ShouldBeTrue();

            var objectIds = keys.Select(k => new RiakObjectId(TestBucket, k)).ToList();
            var loadedDocs = Client.Get(objectIds);
            loadedDocs.All(d => d.IsSuccess).ShouldBeTrue();
            loadedDocs.All(d => d.Value != null).ShouldBeTrue();

            var deleteResults = Client.Delete(objectIds);
            deleteResults.All(r => r.IsSuccess).ShouldBeTrue();
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
        public void MapReduceQueriesReturnData()
        {
            const string dummyData = "{{ value: {0} }}";
            var bucket = Guid.NewGuid().ToString();

            for (var i = 1; i < 11; i++)
            {
                var newData = string.Format(dummyData, i);
                var doc = new RiakObject(bucket, i.ToString(), newData, RiakConstants.ContentTypes.ApplicationJson);

                Client.Put(doc).IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(new RiakPhaseInputs(bucket))
                .MapJs(m => m.Source(@"function(o) {return [ 1 ];}"))
                .ReduceJs(r => r.Name(@"Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(query);
            result.IsSuccess.ShouldBeTrue();

            var mrRes = result.Value;
            mrRes.PhaseResults.ShouldNotBeNull();
            mrRes.PhaseResults.Count.ShouldEqual(2);

            mrRes.PhaseResults[0].Phase.ShouldEqual(0u);
            mrRes.PhaseResults[1].Phase.ShouldEqual(1u);

            mrRes.PhaseResults[0].Value.ShouldBeNull();
            mrRes.PhaseResults[1].Value.ShouldNotBeNull();

            var json = JArray.Parse(result.Value.PhaseResults[1].Value.FromRiakString());
            json[0].Value<int>().ShouldEqual(10);
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
            // delete first if something does exist
            Client.Delete(MultiBucket, MultiKey);

            // Do this via the REST interface - will be substantially slower than PBC
            var props = new RiakBucketProperties().SetAllowMultiple(true).SetLastWriteWins(false);
            props.CanUsePbc.ShouldBeFalse();
            Client.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult1 = Client.Put(doc);
            writeResult1.IsSuccess.ShouldBeTrue();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, RiakConstants.ContentTypes.ApplicationJson);
            var writeResult2 = Client.Put(doc);
            writeResult2.IsSuccess.ShouldBeTrue();
            writeResult2.Value.Siblings.Count.ShouldEqual(2);

            var result = Client.Get(MultiBucket, MultiKey);
            result.Value.Siblings.Count.ShouldEqual(2);
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
        public void DeleteBucketDeletesAllKeysInABucket()
        {
            // add multiple keys
            const string dummyData = "{{ value: {0} }}";
            var bucket = Guid.NewGuid().ToString();

            for (var i = 1; i < 11; i++)
            {
                var newData = string.Format(dummyData, i);
                var doc = new RiakObject(bucket, i.ToString(), newData, RiakConstants.ContentTypes.ApplicationJson);

                Client.Put(doc);
            }

            var keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(10);

            Client.DeleteBucket(bucket);
            
            keyList = Client.ListKeys(bucket);
            keyList.Value.Count().ShouldEqual(0);
            Client.ListBuckets().Value.Contains(bucket).ShouldBeFalse();
        }

        [Test]
        public void LastModifiedShouldChangeAfterAPutRequest()
        {
            const string dummyData = "{{ value: 1234; }}";
            const string changedData = "{{ value: 12345 }}";
            var o = new RiakObject(TestBucket, "1234", dummyData, RiakConstants.ContentTypes.ApplicationJson);

            var lm1 = o.LastModified;
            var lmu1 = o.LastModifiedUsec;

            o = Client.Put(o, new RiakPutOptions { ReturnBody = true}).Value;

            var lm2 = o.LastModified;
            var lmu2 = o.LastModifiedUsec;

            System.Threading.Thread.Sleep(1000);

            o = new RiakObject(TestBucket, "1234", changedData);
            o = Client.Put(o, new RiakPutOptions { ReturnBody = true }).Value;

            var lm3 = o.LastModified;
            var lmu3 = o.LastModifiedUsec;

            lm1.ShouldNotEqual(lm2);
            lm1.ShouldNotEqual(lm3);
            lmu1.ShouldNotEqual(lmu2);
            lmu1.ShouldNotEqual(lmu3);
            lm2.ShouldNotEqual(lm3);
            lmu2.ShouldNotEqual(lmu3);
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
    }
}
