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
using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using Moq;
using NUnit.Framework;

namespace CorrugatedIron.Tests.RiakAsyncClientTests
{
    public abstract class RiakAsyncClientTestBase<TResult>
    {
        protected Mock<IRiakClient> ClientMock;
        protected RiakAsyncClient AsyncClient;
        protected AsyncMethodTester<TResult> Tester;
        protected TResult Result;

        protected RiakAsyncClientTestBase()
        {
            ClientMock = new Mock<IRiakClient>();
            AsyncClient = new RiakAsyncClient(ClientMock.Object);
            Tester = new AsyncMethodTester<TResult>();
        }
    }

    [TestFixture]
    public class WhenPingingServerAsync : RiakAsyncClientTestBase<RiakResult>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Ping()).Returns(RiakResult.Success());
            AsyncClient.Ping(Tester.HandleResult);
            Result = Tester.Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Ping(), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.IsSuccess.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class WhenCallingGetWithObjectIdAsync : RiakAsyncClientTestBase<RiakResult<RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), 2)).Returns(RiakResult<RiakObject>.Success(new RiakObject("foo", "bar", "baz")));
            AsyncClient.Get(new RiakObjectId("foo", "bar"), Tester.HandleResult);
            Result = Tester.Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), 2), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.IsSuccess.ShouldBeTrue();
            Result.Value.ShouldNotBeNull();
            Result.Value.Bucket.ShouldEqual("foo");
            Result.Value.Key.ShouldEqual("bar");
            Result.Value.Value.FromRiakString().ShouldEqual("baz");
        }
    }

    [TestFixture]
    public class WhenCallingGetWithBucketKeyAsync : RiakAsyncClientTestBase<RiakResult<RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), 2)).Returns(RiakResult<RiakObject>.Success(new RiakObject("foo", "bar", "baz")));
            AsyncClient.Get("foo", "bar", Tester.HandleResult);
            Result = Tester.Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), 2), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.IsSuccess.ShouldBeTrue();
            Result.Value.ShouldNotBeNull();
            Result.Value.Bucket.ShouldEqual("foo");
            Result.Value.Key.ShouldEqual("bar");
            Result.Value.Value.FromRiakString().ShouldEqual("baz");
        }
    }

    [TestFixture]
    public class WhenCallingGetManyAsync : RiakAsyncClientTestBase<IEnumerable<RiakResult<RiakObject>>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<IEnumerable<RiakObjectId>>(), 2)).Returns(new List<RiakResult<RiakObject>>());
            AsyncClient.Get(new List<RiakObjectId>(), Tester.HandleResult, 2);
            Result = Tester.Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<IEnumerable<RiakObjectId>>(), 2), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.ShouldBe<List<RiakResult<RiakObject>>>();
            Result.Count().ShouldEqual(0);
        }
    }
}
