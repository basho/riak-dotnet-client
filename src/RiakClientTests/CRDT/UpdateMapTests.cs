// <copyright file="UpdateMapTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2015 - Basho Technologies, Inc.
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

namespace RiakClientTests.CRDT
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using RiakClient.Commands.CRDT;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture]
    public class UpdateMapTests
    {
        [Test]
        public void UpdateMap_Should_Build_A_DtUpdateReq_Correctly()
        {
            const string bucketType = "maps";
            const string bucket = "myBucket";
            const string key = "map_1";

            byte[] context = Encoding.UTF8.GetBytes("test-context");

            var mapOp = new UpdateMap.MapOperation();
            mapOp.IncrementCounter("counter_1", 50);

            /*
                .removeCounter("counter_2")
                .addToSet("set_1", "set_value_1")
                .removeFromSet("set_2", "set_value_2")
                .removeSet("set_3")
                .setRegister("register_1", new Buffer("register_value_1"))
                .removeRegister("register_2")
                .setFlag("flag_1", true)
                .removeFlag("flag_2")
                .removeMap("map_3");

            mapOp.map("map_2").incrementCounter("counter_1", 50)
                .removeCounter("counter_2")
                .addToSet("set_1", new Buffer("set_value_1"))
                .removeFromSet("set_2", new Buffer("set_value_2"))
                .removeSet("set_3")
                .setRegister("register_1", new Buffer("register_value_1"))
                .removeRegister("register_2")
                .setFlag("flag_1", true)
                .removeFlag("flag_2")
                .removeMap("map_3")
                .map("map_2");
             */


            var updateMapCommandBuilder = new UpdateMap.Builder();

            var q1 = new Quorum(1);
            var q2 = new Quorum(2);
            var q3 = new Quorum(3);

            updateMapCommandBuilder
                .WithBucketType(bucketType)
                .WithBucket(bucket)
                .WithKey(key)
                .WithMapOperation(mapOp)
                .WithContext(context)
                .WithW(q3)
                .WithPW(q1)
                .WithDW(q2)
                .WithReturnBody(true)
                .WithIncludeContext(false)
                .WithTimeout(TimeSpan.FromSeconds(20));

            UpdateMap updateMapCommand = updateMapCommandBuilder.Build();

            DtUpdateReq pbReq = updateMapCommand.ConstructPbRequest();
            Assert.AreEqual(Encoding.UTF8.GetBytes(bucketType), pbReq.type);
            Assert.AreEqual(Encoding.UTF8.GetBytes(bucket), pbReq.bucket);
            Assert.AreEqual(Encoding.UTF8.GetBytes(key), pbReq.key);
            Assert.AreEqual(q3, pbReq.w);
            Assert.AreEqual(q1, pbReq.pw);
            Assert.AreEqual(q2, pbReq.dw);
            Assert.True(pbReq.return_body);
            Assert.False(pbReq.include_context);
            Assert.AreEqual(20000, pbReq.timeout);
            Assert.AreEqual(context, pbReq.context);
        }
    }
}