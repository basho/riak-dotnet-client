// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
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

using CorrugatedIron.Encoding;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Tests.Extensions;
using NUnit.Framework;

namespace CorrugatedIron.Tests.Encoding.MessageEncoderUnitTests
{
    [TestFixture]
    public class WhenEncodingProtocolBufferMessages
    {
        private IMessageEncoder _encoder;

        [SetUp]
        public void Setup()
        {
            _encoder = new MessageEncoder();
        }

        [Test]
        public void PingRequestIsSerialisedCorrectly()
        {
            var bytes = _encoder.Encode(new RpbPingReq());
            bytes.ContentsShouldEqual(new byte[] { 0, 0, 0, 1, 1 });
        }

        [Test]
        public void GetClientIdRequestIsSerialisedCorrectly()
        {
            var bytes = _encoder.Encode(new RpbGetClientIdReq());
            bytes.ContentsShouldEqual(new byte[] { 0, 0, 0, 1, 3 });
        }

        [Test]
        public void ListBucketsRequestIsSerialisedCorrectly()
        {
            var bytes = _encoder.Encode(new RpbListBucketsReq());
            bytes.ContentsShouldEqual(new byte[] { 0, 0, 0, 1, 15 });
        }

        [Test]
        public void ListKeysRequestIsSerialisedCorrectly()
        {
            var bytes = _encoder.Encode(new RpbListKeysReq
                {
                    Bucket = "test_bucket".ToRiakString()
                });

            // TODO: Not sure if this is right. Check the right encoding and verify
            // it's right against what is sent to the Riak server from the erlang
            // pb client.
            bytes.ContentsShouldEqual(new byte[] { 0, 0, 0, 14, 17, 10, 11, 116, 101, 115, 116, 95, 98, 117, 99, 107, 101, 116 });
        }
    }
}
