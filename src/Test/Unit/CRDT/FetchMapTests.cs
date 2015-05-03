// <copyright file="FetchMapTests.cs" company="Basho Technologies, Inc.">
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
    using System;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;

    [TestFixture]
    public class FetchMapTests
    {
        private static readonly RiakString BucketType = "maps";
        private static readonly RiakString Bucket = "myBucket";
        private static readonly RiakString Key = "map_1";
        private static readonly byte[] Context = Encoding.UTF8.GetBytes("test-context");

        [Test]
        public void Should_Build_DtFetchReq_Correctly()
        {
            var fetch = new FetchMap.Builder()
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithR((Quorum)1)
                .WithPR((Quorum)2)
                .WithNotFoundOK(true)
                .WithBasicQuorum(true)
                .WithTimeout(TimeSpan.FromMilliseconds(20000))
                .Build();

            DtFetchReq protobuf = (DtFetchReq)fetch.ConstructPbRequest();

            Assert.AreEqual(BucketType, (RiakString)protobuf.type);
            Assert.AreEqual(Bucket, (RiakString)protobuf.bucket);
            Assert.AreEqual(Key, (RiakString)protobuf.key);
            Assert.AreEqual(1, protobuf.r);
            Assert.AreEqual(2, protobuf.pr);
            Assert.AreEqual(true, protobuf.notfound_ok);
            Assert.AreEqual(true, protobuf.basic_quorum);
            Assert.AreEqual(20000, protobuf.timeout);
        }
    }
}