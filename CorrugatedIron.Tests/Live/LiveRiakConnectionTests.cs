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

using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LiveRiakConnectionTests
{
    public class LiveRiakConnectionTestBase
    {
        protected const int TestClientId = 42;
        protected readonly static byte[] ClientId;
        protected const string TestHost = "riak-test";
        protected const int TestHostPort = 8081;
        protected const string TestBucket = "test_bucket";
        protected const string TestKey = "test_json";
        protected const string TestJson = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}";
        protected const string MapReduceBucket = "map_reduce_bucket";
        protected const string TestMapReduce = @"{""inputs"":""map_reduce_bucket"",""query"":[{""map"":{""language"":""javascript"",""keep"":false,""source"":""function(o) {return [ 1 ];}""}},{""reduce"":{""language"":""javascript"",""keep"":true,""name"":""Riak.reduceSum""}}]}";

        protected IRiakConnectionManager ConnectionManager;
        protected IRiakConnectionConfiguration ConnectionConfig;
        protected IRiakClient Client;

        static LiveRiakConnectionTestBase()
        {
            ClientId = RiakConnection.ToClientId(TestClientId);
        }

        public LiveRiakConnectionTestBase(int poolSize = 1, int idleTimeout = 10000)
        {
            ConnectionConfig = new RiakConnectionManualConfiguration
            {
                HostAddress = TestHost,
                HostPort = TestHostPort,
                PoolSize = poolSize,
                AcquireTimeout = 4000,
                IdleTimeout = idleTimeout
            };

        }

        [SetUp]
        public void SetUp()
        {
            ConnectionManager = new RiakConnectionManager(ConnectionConfig);
            Client = new RiakClient(ConnectionManager);
        }

        [TearDown]
        public void TearDown()
        {
            ConnectionManager.Dispose();
        }
    }

    [TestFixture]
    public class WhenTalkingToRiak : LiveRiakConnectionTestBase
    {
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
        public void WritingThenReadingJsonIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);

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
        public void DeletingIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc);

            var result = Client.Get(TestBucket, TestKey);
            result.IsSuccess.ShouldBeTrue();

            Client.Delete(doc.Bucket, doc.Key);
            result = Client.Get(TestBucket, TestKey);
            result.IsSuccess.ShouldBeFalse();
            result.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void MapReduceQueriesReturnData()
        {
            const string dummyData = "{{ value: {0} }}";

            for (var i = 1; i < 11; i++)
            {
                var newData = string.Format(dummyData, i);
                var doc = new RiakObject(MapReduceBucket, i.ToString(), newData, Constants.ContentTypes.ApplicationJson);

                Client.Put(doc);
            }

            var result = Client.MapReduce(TestMapReduce);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Response.GetType().ShouldEqual(typeof(byte[]));
            result.Value.Response.FromRiakString().ShouldEqual("[10]");
        }

        [Test]
        public void ListBucketsIncludesTestBucket ()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc);

            var result = Client.ListBuckets();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestBucket);
        }

        [Test]
        public void ListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc);

            var result = Client.ListKeys(TestBucket);
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestKey);
        }
    }

    [TestFixture]
    public class WhenConnectionGoesIdle : LiveRiakConnectionTestBase
    {
        public WhenConnectionGoesIdle()
            : base(1, 1)
        {
        }

        private IRiakConnection GetIdleConnection()
        {
            var result = ConnectionManager.UseConnection(ClientId, RiakResult<IRiakConnection>.Success);
            System.Threading.Thread.Sleep(ConnectionConfig.IdleTimeout + 1000);
            return result.Value;
        }

        [Test]
        public void IsIdleFlagIsSet()
        {
            var conn = GetIdleConnection();
            conn.IsIdle.ShouldBeTrue();
        }

        [Test]
        public void ConnectionIsRestoredOnNextUse()
        {
            GetIdleConnection();
            var result = Client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void IdleFlagIsUnsetOnNextUse()
        {
            var conn = GetIdleConnection();
            Client.Ping();
            conn.IsIdle.ShouldBeFalse();
        }
    }
}
