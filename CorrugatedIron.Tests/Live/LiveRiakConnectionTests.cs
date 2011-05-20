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
using CorrugatedIron.Comms;
using CorrugatedIron.Config;
using CorrugatedIron.Models;
using CorrugatedIron.Extensions;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using CorrugatedIron.Messages;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LiveRiakConnectionTests
{
    [TestFixture]
    public class WhenTalkingToRiak
    {
        private const string TestBucket = "test_bucket";
        private const string TestKey = "test_json";
        private const string TestJson = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}";
		private const string MapReduceBucket = "map_reduce_bucket";
		private const string TestMapReduce = @"{""inputs"":""map_reduce_bucket"",""query"":[{""map"":{""language"":""javascript"",""keep"":false,""source"":""function(o) {return [ 1 ];}""}},{""reduce"":{""language"":""javascript"",""keep"":true,""name"":""Riak.reduceSum""}}]}";
        private IRiakConnectionManager _connectionManager;
        private IRiakConnectionConfiguration _connectionConfig;

        [SetUp]
        public void SetUp()
        {
            _connectionConfig = new RiakConnectionConfiguration
            {
                HostAddress = "127.0.0.1",
                HostPort = 8081,
                PoolSize = 1
            };

            _connectionManager = new RiakConnectionManager(_connectionConfig);
        }

        [TearDown]
        public void TearDown()
        {
            _connectionManager.Dispose();
        }

        [Test]
        public void PingRequstResultsInPingResponse()
        {
            var result = _connectionManager.UseConnection(conn => conn.Ping());
            result.IsError.ShouldBeFalse();
        }

        [Test]
        public void WritingThenReadingJsonIsSuccessful()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);

            var writeResult = _connectionManager.UseConnection(conn => conn.Put(doc));
            writeResult.IsError.ShouldBeFalse();

            var readResult = _connectionManager.UseConnection(conn => conn.Get(TestBucket, TestKey));
            readResult.IsError.ShouldBeFalse();

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
			
			_connectionManager.UseConnection(conn => conn.Put(doc));
			_connectionManager.UseConnection(conn => conn.Delete(TestBucket, TestKey, Constants.Defaults.RVal));
			
			var result = _connectionManager.UseConnection(conn => conn.Get(TestBucket, TestKey));

		    result.Value.ShouldEqual(null);
		}
		
		[Test]
		public void SettingTheClientIdIsSuccessful()
		{
			var writeResult = _connectionManager.UseConnection(conn => conn.SetClientId("test"));
			writeResult.IsError.ShouldBeFalse();
		}
		
		[Test]
		public void MapReduceQueriesReturnData()
		{
			string dummydata = "{{ value: {0}; }}";
			
			for (int i = 1; i < 11; i++) {
				var newdata = string.Format(dummydata, i);
				var doc = new RiakObject(MapReduceBucket, i.ToString(), newdata, Constants.ContentTypes.ApplicationJson);
				
				_connectionManager.UseConnection(conn => conn.Put(doc));
			}
			
			var results = _connectionManager.UseConnection(conn => conn.MapReduce(TestMapReduce, Constants.ContentTypes.ApplicationJson));
			results.IsError.ShouldBeFalse();
			(results.Value as RpbMapRedResp).Response.GetType().ShouldEqual(typeof(byte[]));
			(results.Value as RpbMapRedResp).Response.FromRiakString().ShouldEqual("[10]");
		}
    }
}
