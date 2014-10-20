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

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CorrugatedIron.Comms;
using CorrugatedIron.Containers;
using CorrugatedIron.Exceptions;
using CorrugatedIron.Extensions;
using CorrugatedIron.Messages;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace CorrugatedIron.Tests.RiakAsyncClientTests
{
    internal abstract class RiakAsyncClientTestBase<TResult>
    {
        protected Mock<IRiakEndPoint> EndPointMock;
        protected Mock<IRiakConnection> ConnectionMock;
        protected RiakAsyncClient AsyncClient;
        protected TResult Result;

        protected RiakAsyncClientTestBase()
        {
            EndPointMock = new Mock<IRiakEndPoint>();
            ConnectionMock = new Mock<IRiakConnection>();
            AsyncClient = new RiakAsyncClient(EndPointMock.Object, ConnectionMock.Object);
        }
    }

    [TestFixture]
    internal class WhenPingingServerAsync : RiakAsyncClientTestBase<Pong>
    {
        [SetUp]
        public void SetUp()
        {
            ConnectionMock.Setup(m => m.PbcWriteRead(It.IsAny<IRiakEndPoint>(), MessageCode.PingReq, MessageCode.PingResp)).Returns(Task.FromResult(false));
            Result = AsyncClient.Ping().Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ConnectionMock.Verify(m => m.PbcWriteRead(It.IsAny<IRiakEndPoint>(), MessageCode.PingReq, MessageCode.PingResp), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.ShouldBe<Pong>();
            Result.ResponseTime.Milliseconds.ShouldBeGreaterThan(0);
        }
    }

    [TestFixture]
    internal class WhenCallingGetWithObjectIdAsync : RiakAsyncClientTestBase<Either<RiakException, RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            var rpbContent = new RpbContent
            {
                value = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}".ToRiakString(),
            };

            var response = new RpbGetResp();
            response.content.Add(rpbContent);
            response.vclock = new byte[] { };

            ConnectionMock.Setup(
                m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()))
                .Returns(Task.FromResult(response));

            Task<Either<RiakException, RiakObject>> taskResults = AsyncClient.Get(new RiakObjectId("foo", "bar"));
            Either<RiakException, RiakObject> results = taskResults.Result;

            Result = results;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ConnectionMock.Verify(m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.ShouldBe<Either<RiakException, RiakObject>>();
            Result.IsLeft.ShouldBeFalse();
            Result.Right.ShouldNotBeNull();
            Result.Right.Bucket.ShouldEqual("foo");
            Result.Right.Key.ShouldEqual("bar");
            Result.Right.Value.FromRiakString().ShouldEqual("{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}");
        }
    }

    [TestFixture]
    internal class WhenCallingGetWithBucketKeyAsync : RiakAsyncClientTestBase<Either<RiakException, RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            var rpbContent = new RpbContent
            {
                value = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}".ToRiakString(),
            };

            var response = new RpbGetResp();
            response.content.Add(rpbContent);
            response.vclock = new byte[] { };

            ConnectionMock.Setup(
                m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()))
                .Returns(Task.FromResult(response));

            Task<Either<RiakException, RiakObject>> taskResults = AsyncClient.Get("foo", "bar");
            Either<RiakException, RiakObject> results = taskResults.Result;

            Result = results;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ConnectionMock.Verify(m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.ShouldBe<Either<RiakException, RiakObject>>();
            Result.IsLeft.ShouldBeFalse();
            Result.Right.ShouldNotBeNull();
            Result.Right.Bucket.ShouldEqual("foo");
            Result.Right.Key.ShouldEqual("bar");
            Result.Right.Value.FromRiakString().ShouldEqual("{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}");
        }
    }

    [TestFixture]
    internal class WhenCallingGetManyAsync : RiakAsyncClientTestBase<List<Either<RiakException, RiakObject>>>
    {
        [SetUp]
        public void SetUp()
        {
            var rpbContent = new RpbContent
            {
                value = "{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}".ToRiakString(),
            };

            var response = new RpbGetResp();
            response.content.Add(rpbContent);
            response.vclock = new byte[] { };

            ConnectionMock.Setup(
                m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()))
                .Returns(Task.FromResult(response));

            IObservable<Either<RiakException, RiakObject>> observableResults = AsyncClient.Get(new List<RiakObjectId>
            {
                new RiakObjectId("foo", "bar")
            });

            List<Either<RiakException, RiakObject>> results = observableResults
                .ToEnumerable()
                .ToList();

            Result = results;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ConnectionMock.Verify(m => m.PbcWriteRead<RpbGetReq, RpbGetResp>(It.IsAny<IRiakEndPoint>(), It.IsAny<RpbGetReq>()), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.Count().ShouldEqual(1);
            Result[0].ShouldNotBeNull();
            Result[0].ShouldBe<Either<RiakException, RiakObject>>();
            Result[0].IsLeft.ShouldBeFalse();
            Result[0].Right.ShouldNotBeNull();
            Result[0].Right.Bucket.ShouldEqual("foo");
            Result[0].Right.Key.ShouldEqual("bar");
            Result[0].Right.Value.FromRiakString().ShouldEqual("{\"string\":\"value\",\"int\":100,\"float\":2.34,\"array\":[1,2,3],\"dict\":{\"foo\":\"bar\"}}");
        }
    }
}
