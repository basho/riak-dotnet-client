// <copyright file="UpdateHllTests.cs" company="Basho Technologies, Inc.">
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
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;

    [TestFixture, UnitTest]
    public class UpdateHllTests
    {
        private const string BucketType = "hlls";
        private const string Bucket = "myBucket";
        private const string Key = "hll_1";

        private static readonly ISet<byte[]> DefaultAdds = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("add_1"),
                Encoding.UTF8.GetBytes("add_2")
            };

        [Test]
        public void Should_Build_DtUpdateReq_Correctly()
        {
            var updateHllCommandBuilder = new UpdateHll.Builder(DefaultAdds);

            var q1 = new Quorum(1);
            var q2 = new Quorum(2);
            var q3 = new Quorum(3);

            updateHllCommandBuilder
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithW(q3)
                .WithPW(q1)
                .WithDW(q2)
                .WithReturnBody(true)
                .WithTimeout(TimeSpan.FromSeconds(20));

            UpdateHll updateHllCommand = updateHllCommandBuilder.Build();

            DtUpdateReq protobuf = (DtUpdateReq)updateHllCommand.ConstructRequest(false);

            Assert.AreEqual(Encoding.UTF8.GetBytes(BucketType), protobuf.type);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Bucket), protobuf.bucket);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Key), protobuf.key);
            Assert.AreEqual(q3, protobuf.w);
            Assert.AreEqual(q1, protobuf.pw);
            Assert.AreEqual(q2, protobuf.dw);
            Assert.IsTrue(protobuf.return_body);
            Assert.IsFalse(protobuf.include_context);
            Assert.AreEqual(20000, protobuf.timeout);

            HllOp hllOpMsg = protobuf.op.hll_op;

            Assert.AreEqual(DefaultAdds, hllOpMsg.adds);
        }

        [Test]
        public void Should_Construct_HllResponse_From_DtUpdateResp()
        {
            var key = new RiakString("riak_generated_key");

            var updateResp = new DtUpdateResp();
            updateResp.key = key;
            updateResp.hll_value = 42;

            var update = new UpdateHll.Builder(DefaultAdds)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .Build();

            update.OnSuccess(updateResp);

            HllResponse response = update.Response;

            Assert.NotNull(response);
            Assert.AreEqual(key, response.Key);
            Assert.AreEqual(42, response.Value);
        }
    }
}