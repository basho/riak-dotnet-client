namespace RiakClientTests.RiakAsyncClientTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using RiakClient;
    using RiakClient.Extensions;
    using RiakClient.Models;

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

    [TestFixture, UnitTest]
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

    [TestFixture, UnitTest]
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

    [TestFixture, UnitTest]
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

    [TestFixture, UnitTest]
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
