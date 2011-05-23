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
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LiveRiakConnectionTests
{
    public class LiveRiakConnectionTestBase
    {
        protected const string TestHost = "riak-test";
        protected const int TestHostPort = 8081;
        protected const string TestBucket = "test_bucket";
        protected const string TestKey = "test_json";
        protected const string TestJson = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}";

        protected IRiakConnectionManager _connectionManager;
        protected IRiakConnectionConfiguration _connectionConfig;
        protected IRiakClient _client;

        public LiveRiakConnectionTestBase(int poolSize = 1, int idleTimeout = 10000)
        {
            _connectionConfig = new RiakConnectionManualConfiguration
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
            _connectionManager = new RiakConnectionManager(_connectionConfig);
            _client = new RiakClient(_connectionManager);
        }

        [TearDown]
        public void TearDown()
        {
            _connectionManager.Dispose();
        }

    }

    [TestFixture]
    public class WhenTalkingToRiak : LiveRiakConnectionTestBase
    {
        [Test]
        public void PingRequstResultsInPingResponse()
        {
            var result = _client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void ReadingMissingValueDoesntBreak()
        {
            var readResult = _client.Get("nobucket", "novalue");
            readResult.IsSuccess.ShouldBeFalse();
            readResult.ResultCode.ShouldEqual(ResultCode.NotFound);
        }

        [Test]
        public void WritingThenReadingJsonIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);

            var writeResult = _client.Put(doc);
            writeResult.IsSuccess.ShouldBeTrue();

            var readResult = _client.Get(TestBucket, TestKey);
            readResult.IsSuccess.ShouldBeTrue();

            var loadedDoc = readResult.Value;

            loadedDoc.Bucket.ShouldEqual(doc.Bucket);
            loadedDoc.Key.ShouldEqual(doc.Key);
            loadedDoc.Value.ShouldEqual(doc.Value);
            loadedDoc.VectorClock.ShouldNotBeNullOrEmpty();
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
            var result = _connectionManager.UseConnection(RiakResult<IRiakConnection>.Success);
            System.Threading.Thread.Sleep(_connectionConfig.IdleTimeout + 1000);
            return result.Value;
        }

        [Test]
        public void IsIdleFlagIsSet()
        {
            var conn = GetIdleConnection();
            conn.IsIdle.ShouldBeTrue();
        }

        [Test]
        [Ignore("Please see function comments")]
        public void ConnectionToRiakIsTerminated()
        {
            // TODO: should we do this? It would require
            // diving into private members. I think this
            // is overkill given that the IsIdle flag
            // is based on the connection status anyway.
            // Thoughts?
        }

        [Test]
        public void ConnectionIsRestoredOnNextUse()
        {
            GetIdleConnection();
            var result = _client.Ping();
            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void IdleFlagIsUnsetOnNextUse()
        {
            var conn = GetIdleConnection();
            _client.Ping();
            conn.IsIdle.ShouldBeFalse();
        }
    }
}
