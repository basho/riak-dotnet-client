namespace RiakClientTests.Models.Search
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using RiakClient.Models.Search;

    [TestFixture, UnitTest]
    public class RiakSearchResultTests
    {
        [Test]
        public void TestPublicConstructor()
        {
            var result = new RiakSearchResult(100.0f, 42, new List<RiakSearchResultDocument>());
            result.MaxScore.ShouldEqual(100.0f);
            result.NumFound.ShouldEqual(42);
            result.Documents.ShouldNotBeNull();
            result.Documents.Count.ShouldEqual(0);
        }

        [Test]
        public void TestPublicConstructorThrowsExceptionWhenDocumentsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new RiakSearchResult(100.0f, 42, null));
        }
    }
}
