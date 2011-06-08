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
using CorrugatedIron.Models.CommitHook;
using CorrugatedIron.Models.MapReduce;
using CorrugatedIron.Models.MapReduce.Inputs;
using CorrugatedIron.Tests.Extensions;
using CorrugatedIron.Util;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Live.LiveRiakConnectionTests
{
    public class LiveRiakConnectionTestBase
    {
        protected const int TestClientId = 42;
        protected readonly static byte[] ClientId;
        protected const string TestHost = "riak-test";
        protected const int TestPbcPort = 8081;
        protected const int TestHttpPort = 8091;
        protected const string TestBucket = "test_bucket";
        protected const string TestKey = "test_json";
        protected const string TestJson = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}";
        protected const string MapReduceBucket = "map_reduce_bucket";
        protected const string MultiBucket = "test_multi_bucket";
        protected const string MultiKey = "test_multi_key";
        protected const string MultiBodyOne = @"{""dishes"": 9}";
        protected const string MultiBodyTwo = @"{""dishes"": 11}";
        protected const string PropertiesTestBucket = @"propertiestestbucket";

        protected IRiakCluster Cluster;
        protected IRiakClient Client;
        protected IRiakClusterConfiguration ClusterConfig;

        static LiveRiakConnectionTestBase()
        {
            ClientId = RiakConnection.ToClientId(TestClientId);
        }

        public LiveRiakConnectionTestBase(string section = "riak3NodeConfiguration")
        {
            ClusterConfig = RiakClusterConfiguration.LoadFromConfig(section);
        }

        [SetUp]
        public void SetUp()
        {
            Cluster = new RiakCluster(ClusterConfig, new RiakNodeFactory(), new RiakConnectionFactory());
            Client = new RiakClient(Cluster);
        }

        [TearDown]
        public void TearDown()
        {
            Cluster.Dispose();
        }
    }

    [TestFixture]
    public class WhenDealingWithBucketProperties : LiveRiakConnectionTestBase
    {
        // use the one node configuration here because we might run the risk
        // of hitting different nodes in the configuration before the props
        // are replicated to other nodes.
        public WhenDealingWithBucketProperties()
            :base("riak1NodeConfiguration")
        {
        }

        [Test]
        public void GettingWithoutExtendedFlagDoesNotReturnExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket);
            result.IsSuccess.ShouldBeTrue();
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.HasValue.ShouldBeTrue();
            result.Value.LastWriteWins.HasValue.ShouldBeFalse();
            result.Value.RVal.ShouldBeNull();
            result.Value.RwVal.ShouldBeNull();
            result.Value.DwVal.ShouldBeNull();
            result.Value.WVal.ShouldBeNull();
        }

        [Test]
        public void GettingWithExtendedFlagReturnsExtraProperties()
        {
            var result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue();
            result.Value.AllowMultiple.HasValue.ShouldBeTrue();
            result.Value.NVal.HasValue.ShouldBeTrue();
            result.Value.LastWriteWins.HasValue.ShouldBeTrue();
            result.Value.RVal.ShouldNotBeNull();
            result.Value.RwVal.ShouldNotBeNull();
            result.Value.DwVal.ShouldNotBeNull();
            result.Value.WVal.ShouldNotBeNull();
        }

        [Test]
        public void CommitHooksAreStoredAndLoadedProperly()
        {
            // make sure we're all clear first
            var result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue();
            var props = result.Value;
            props.ClearPostCommitHooks().ClearPreCommitHooks();
            Client.SetBucketProperties(PropertiesTestBucket, props).IsSuccess.ShouldBeTrue();

            // when we load, the commit hook lists should be null
            result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue();
            props = result.Value;
            props.PreCommitHooks.ShouldBeNull();
            props.PostCommitHooks.ShouldBeNull();

            // we then store something in each
            props.AddPreCommitHook(new RiakJavascriptCommitHook("Foo.doBar"))
                .AddPreCommitHook(new RiakErlangCommitHook("my_mod", "do_fun"))
                .AddPostCommitHook(new RiakErlangCommitHook("my_other_mod", "do_more"));
            Client.SetBucketProperties(PropertiesTestBucket, props).IsSuccess.ShouldBeTrue();

            // load them out again and make sure they got loaded up
            result = Client.GetBucketProperties(PropertiesTestBucket, true);
            result.IsSuccess.ShouldBeTrue();
            props = result.Value;

            props.PreCommitHooks.ShouldNotBeNull();
            props.PreCommitHooks.Count.ShouldEqual(2);
            props.PostCommitHooks.ShouldNotBeNull();
            props.PostCommitHooks.Count.ShouldEqual(1);
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

            for (var i = 1; i < 11; i++)
            {
                var newData = string.Format(dummyData, i);
                var doc = new RiakObject(MapReduceBucket, i.ToString(), newData, Constants.ContentTypes.ApplicationJson);

                Client.Put(doc).IsSuccess.ShouldBeTrue();
            }

            var query = new RiakMapReduceQuery()
                .Inputs(new RiakPhaseInputs(MapReduceBucket))
                .Map(m => m.Source(@"function(o) {return [ 1 ];}"))
                .Reduce(r => r.Name(@"Riak.reduceSum").Keep(true));

            var result = Client.MapReduce(query);
            result.IsSuccess.ShouldBeTrue();
            result.Value.Response.ShouldNotBeNull();
            result.Value.Response.GetType().ShouldEqual(typeof(byte[]));

            var json = JArray.Parse(result.Value.Response.FromRiakString());
            json[0].Value<int>().ShouldEqual(10);
        }

        [Test]
        public void ListBucketsIncludesTestBucket()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.ListBuckets();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestBucket);
        }

        [Test]
        public void ListKeysIncludesTestKey()
        {
            var doc = new RiakObject(TestBucket, TestKey, TestJson, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.ListKeys(TestBucket);
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldContain(TestKey);
        }

        [Test]
        public void WritesWithAllowMultProducesMultiple()
        {
            // Do this via the REST interface - will be substantially slower than PBC
            var props = new RiakBucketProperties().SetAllowMultiple(true).SetLastWriteWins(false);
            props.CanUsePbc.ShouldBeFalse();
            Client.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.Get(MultiBucket, MultiKey);
            result.Value.Siblings.Count.IsAtLeast(2);
        }

        [Test]
        public void WritesWithAllowMultProducesMultipleVTags()
        {
            // Do this via the PBC - noticable quicker than REST
            var props = new RiakBucketProperties().SetAllowMultiple(true);
            props.CanUsePbc.ShouldBeTrue();
            Client.SetBucketProperties(MultiBucket, props).IsSuccess.ShouldBeTrue();

            var doc = new RiakObject(MultiBucket, MultiKey, MultiBodyOne, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            doc = new RiakObject(MultiBucket, MultiKey, MultiBodyTwo, Constants.ContentTypes.ApplicationJson);
            Client.Put(doc).IsSuccess.ShouldBeTrue();

            var result = Client.Get(MultiBucket, MultiKey);

            result.Value.VTags.ShouldNotBeNull();
            result.Value.VTags.Count.IsAtLeast(2);
        }
    }

    [TestFixture]
    public class WhenConnectionGoesIdle : LiveRiakConnectionTestBase
    {
        public WhenConnectionGoesIdle()
            : base("riak1NodeConfiguration")
        {
        }

        private IRiakConnection GetIdleConnection()
        {
            var result = Cluster.UseConnection(ClientId, RiakResult<IRiakConnection>.Success);
            System.Threading.Thread.Sleep(ClusterConfig.RiakNodes[0].IdleTimeout + 1000);
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
