// <copyright file="FetchPreflistTests.cs" company="Basho Technologies, Inc.">
// Copyright 2015 - Basho Technologies, Inc.
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

namespace Test.Unit.CRDT
{
    using System.Linq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.KV;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class FetchPreflistTests
    {
        private static readonly RiakString BucketType = "myBucketType";
        private static readonly RiakString Bucket = "myBucket";
        private static readonly RiakString Key = "key_1";

        [Test]
        public void Should_Build_Req_Correctly()
        {
            var fetch = new FetchPreflist.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .Build();

            RpbGetBucketKeyPreflistReq protobuf = (RpbGetBucketKeyPreflistReq)fetch.ConstructRequest(false);

            Assert.AreEqual(BucketType, RiakString.FromBytes(protobuf.type));
            Assert.AreEqual(Bucket, RiakString.FromBytes(protobuf.bucket));
            Assert.AreEqual(Key, RiakString.FromBytes(protobuf.key));
        }

        [Test]
        public void Should_Construct_PreflistResponse_From_Resp()
        {
            string node_name = "node-foo";
            long partitionId = long.MaxValue;

            var preflistItem = new RpbBucketKeyPreflistItem();
            preflistItem.node = RiakString.ToBytes(node_name);
            preflistItem.partition = partitionId;
            preflistItem.primary = true;

            var fetchResp = new RpbGetBucketKeyPreflistResp();
            fetchResp.preflist.Add(preflistItem);

            var fetch = new FetchPreflist.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .Build();

            fetch.OnSuccess(fetchResp);

            Assert.AreEqual(1, fetch.Response.Value.Count());
        }
    }
}
