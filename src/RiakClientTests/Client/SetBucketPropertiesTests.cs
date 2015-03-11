// <copyright file="SetBucketPropertiesTests.cs" company="Basho Technologies, Inc.">
// Copyright (c) 2011 - OJ Reeves & Jeremiah Peschka
// Copyright (c) 2014 - Basho Technologies, Inc.
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

namespace RiakClientTests.Client
{
    using Moq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Messages;
    using RiakClient.Models;

    [TestFixture]
    public class WhenSettingBucketPropertiesWithoutExtendedProperties : ClientTestBase
    {
        protected RiakResult Response;
        [SetUp]
        public void SetUp()
        {
            var result = RiakResult.Success();
            Cluster.ConnectionMock.Setup(m => m.PbcWriteRead(It.IsAny<RpbSetBucketReq>(), MessageCode.RpbSetBucketResp)).Returns(result);

            Response = Client.SetBucketProperties("foo", new RiakBucketProperties().SetAllowMultiple(true));
        }

        [Test]
        public void PbcInterfaceIsInvokedWithAppropriateValues()
        {
            Cluster.ConnectionMock.Verify(m => m.PbcWriteRead(It.Is<RpbSetBucketReq>(r => r.props.allow_mult), MessageCode.RpbSetBucketResp), Times.Once());
        }
    }
}

