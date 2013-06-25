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

using CorrugatedIron.Extensions;
using CorrugatedIron.Models;
using CorrugatedIron.Tests.Extensions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using CorrugatedIron.Util;

namespace CorrugatedIron.Tests.RiakAsyncClientTests
{
    internal abstract class RiakAsyncClientTestBase<TResult>
    {
        protected Mock<IRiakClient> ClientMock;
        protected RiakAsyncClient AsyncClient;
        protected TResult Result;

        protected RiakAsyncClientTestBase()
        {
            ClientMock = new Mock<IRiakClient>();
            AsyncClient = new RiakAsyncClient(ClientMock.Object);
        }
    }

    [TestFixture]
    internal class WhenPingingServerAsync : RiakAsyncClientTestBase<RiakResult>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Ping()).Returns(RiakResult.Success());
            Result = AsyncClient.Ping().Result;
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
    internal class WhenCallingGetWithObjectIdAsync : RiakAsyncClientTestBase<RiakResult<RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RiakGetOptions>())).Returns(RiakResult<RiakObject>.Success(new RiakObject("foo", "bar", "baz")));
            Result = AsyncClient.Get(new RiakObjectId("foo", "bar")).Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RiakGetOptions>()), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.IsSuccess.ShouldBeTrue(Result.ErrorMessage);
            Result.Value.ShouldNotBeNull();
            Result.Value.Bucket.ShouldEqual("foo");
            Result.Value.Key.ShouldEqual("bar");
            Result.Value.Value.FromRiakString().ShouldEqual("baz");
        }
    }

    [TestFixture]
    internal class WhenCallingGetWithBucketKeyAsync : RiakAsyncClientTestBase<RiakResult<RiakObject>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RiakGetOptions>())).Returns(RiakResult<RiakObject>.Success(new RiakObject("foo", "bar", "baz")));
            Result = AsyncClient.Get("foo", "bar").Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RiakGetOptions>()), Times.Once());
        }

        [Test]
        public void AsyncClientReturnsCorrectResult()
        {
            Result.ShouldNotBeNull();
            Result.IsSuccess.ShouldBeTrue(Result.ErrorMessage);
            Result.Value.ShouldNotBeNull();
            Result.Value.Bucket.ShouldEqual("foo");
            Result.Value.Key.ShouldEqual("bar");
            Result.Value.Value.FromRiakString().ShouldEqual("baz");
        }
    }

    [TestFixture]
    internal class WhenCallingGetManyAsync : RiakAsyncClientTestBase<IEnumerable<RiakResult<RiakObject>>>
    {
        [SetUp]
        public void SetUp()
        {
            ClientMock.Setup(m => m.Get(It.IsAny<IEnumerable<RiakObjectId>>(), null)).Returns(new List<RiakResult<RiakObject>>());
            Result = AsyncClient.Get(new List<RiakObjectId>()).Result;
        }

        [Test]
        public void AsyncClientInvokesCorrectClientFunction()
        {
            ClientMock.Verify(m => m.Get(It.IsAny<IEnumerable<RiakObjectId>>(), It.IsAny<RiakGetOptions>()), Times.Once());
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
