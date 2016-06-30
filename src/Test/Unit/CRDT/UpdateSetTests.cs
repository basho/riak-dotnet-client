// <copyright file="UpdateSetTests.cs" company="Basho Technologies, Inc.">
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
    public class UpdateSetTests
    {
        private const string BucketType = "sets";
        private const string Bucket = "myBucket";
        private const string Key = "set_1";

        private static readonly ISet<byte[]> DefaultAdds = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("add_1"),
                Encoding.UTF8.GetBytes("add_2")
            };

        private static readonly ISet<byte[]> DefaultRemoves = new HashSet<byte[]>
            {
                Encoding.UTF8.GetBytes("remove_1"),
                Encoding.UTF8.GetBytes("remove_2")
            };

        private static readonly byte[] Context = Encoding.UTF8.GetBytes("1234");

        [Test]
        public void Should_Build_DtUpdateReq_Correctly()
        {
            var updateSetCommandBuilder = new UpdateSet.Builder(DefaultAdds, DefaultRemoves);

            var q1 = new Quorum(1);
            var q2 = new Quorum(2);
            var q3 = new Quorum(3);

            updateSetCommandBuilder
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .WithKey(Key)
                .WithW(q3)
                .WithPW(q1)
                .WithDW(q2)
                .WithReturnBody(true)
                .WithIncludeContext(false)
                .WithContext(Context)
                .WithTimeout(TimeSpan.FromSeconds(20));

            UpdateSet updateSetCommand = updateSetCommandBuilder.Build();

            DtUpdateReq protobuf = (DtUpdateReq)updateSetCommand.ConstructRequest(false);

            Assert.AreEqual(Encoding.UTF8.GetBytes(BucketType), protobuf.type);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Bucket), protobuf.bucket);
            Assert.AreEqual(Encoding.UTF8.GetBytes(Key), protobuf.key);
            Assert.AreEqual(q3, protobuf.w);
            Assert.AreEqual(q1, protobuf.pw);
            Assert.AreEqual(q2, protobuf.dw);
            Assert.IsTrue(protobuf.return_body);
            Assert.IsFalse(protobuf.include_context);
            Assert.AreEqual(20000, protobuf.timeout);

            SetOp setOpMsg = protobuf.op.set_op;

            Assert.AreEqual(DefaultAdds, setOpMsg.adds);
            Assert.AreEqual(DefaultRemoves, setOpMsg.removes);
        }

        [Test]
        public void Should_Construct_SetResponse_From_DtUpdateResp()
        {
            var key = new RiakString("riak_generated_key");

            var updateResp = new DtUpdateResp();
            updateResp.key = key;
            updateResp.context = Context;
            updateResp.set_value.AddRange(DefaultAdds);

            var update = new UpdateSet.Builder(DefaultAdds, null)
                .WithBucketType(BucketType)
                .WithBucket(Bucket)
                .Build();

            update.OnSuccess(updateResp);

            SetResponse response = update.Response;

            Assert.NotNull(response);
            Assert.AreEqual(key, response.Key);
            Assert.AreEqual(Context, response.Context);
            Assert.AreEqual(DefaultAdds, response.Value);
        }
    }
}